using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using IdeaBlade.EntityModel;
using System.Threading.Tasks;

namespace DomainModel
{
  public static class SessionManager
  {
    public static string UserName { get; set; }

    public static int BookingAccoId { get; set; }

    public static Acco BookingAcco { get; set; }
    
    public static Acco CurrentAcco { get; set; }

    public static AccoOwner CurrentOwner { get; set; }

    public static AccoTrustee CurrentTrustee { get; set; }

    public static bool IsTrustee { get; set; }
    public static int BookingLanguageId { get; set; }

    public static CultureInfo Culture { get; set; }

    public static ResourceManager Resource { get; set; }

    public static string GetString(string text)
    {
      if (Resource == null)
      {
        Culture = CultureInfo.CurrentCulture; //new CultureInfo("en-US");
        Resource = new ResourceManager("DomainModel.AccoResource", Assembly.GetExecutingAssembly());
      }

      string translated = Resource.GetString(text, Culture);
      if (!string.IsNullOrEmpty(translated))
        return translated;
      return text;
    }

    public static async Task InitializeAsync(string username)
    {

      UserName = username;

      if (username == "guest")
      {
        CurrentOwner = null;
        CurrentAcco = null;
      }
      else
      {
        var em = new AccoBookingEntities();
        
        CurrentOwner = await em.AccoOwners.AsScalarAsync().FirstOrDefault(a => a.Login == username);
        CurrentTrustee = null;
        if (CurrentOwner == null)
        {
          CurrentTrustee = await em.AccoTrustees.AsScalarAsync().FirstOrDefault(a => a.Login == username);
          CurrentOwner = await em.AccoOwners.AsScalarAsync().FirstOrDefault(a => a.AccoOwnerId== CurrentTrustee.AccoOwnerId);
          CurrentAcco = await em.Accoes.AsScalarAsync().FirstOrDefault(a => a.AccoOwnerId == CurrentTrustee.AccoOwnerId);
        }
        CurrentAcco = await em.Accoes.AsScalarAsync().FirstOrDefault(a => a.AccoOwnerId == CurrentOwner.AccoOwnerId);      
      }
      IsTrustee = CurrentTrustee != null;
    }

    public static async Task BookingLanguage(int languageid)
    {
      var em = new AccoBookingEntities();
      var lang = await em.Languages.AsScalarAsync().FirstOrDefault(l => l.LanguageId == languageid);
      if (lang != null)
      {
        BookingLanguageId = lang.LanguageId;
      }

    }

    public static void Clone(Entity source, Entity destination)
    {
      Clone(source, destination, new string[] { });
    }

    public static void Clone(Entity source, Entity destination, string[] properties)
    {
      //clone the entity (except the primary key)
      foreach (var p in source.EntityAspect.EntityMetadata.DataProperties.Where(x => !x.IsPartOfKey))
      {
        if (properties.ToList().Contains(p.Name))
          continue;
        var dp = destination.EntityAspect.EntityMetadata.DataProperties.FirstOrDefault(e => e.Name == p.Name);
        if (dp.IsForeignKeyProperty)
          continue;

        dp.SetValue(destination, p.GetValue(source, EntityVersion.Current));
      }
    }
  }
}
