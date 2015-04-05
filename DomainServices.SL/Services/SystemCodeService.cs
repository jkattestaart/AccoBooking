using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public static class SystemCodeService
  {
    public static IEnumerable<SystemCode> SystemCodeList;

    public static async Task LoadSystemCodesAsync()
    {
      var entityManagerProvider = new EntityManagerProvider<EntityManager>();
      var unitOfWork = new UnitOfWork<SystemCode>(entityManagerProvider);

      SystemCodeList = await unitOfWork.Entities.FindInDataSourceAsync(s => true, q => q.OrderBy(s => s.Code), options => options.Include(x => x.SystemGroup));


      foreach (var systemCode in SystemCodeList)
      {
        string code = systemCode.SystemGroup.Name + "_" + systemCode.Code.Replace('-', '_');
        var translated = SessionManager.GetString(code);
        if (translated != code)
          systemCode.Description = translated;
      }
    }

    public static string Description(string code,  string group)
    {
      if (code == null)
        return "";

      var systemCode = SystemCodeList.FirstOrDefault(t => t.Code == code.ToUpper() && t.SystemGroup.Name == group);
      return ((systemCode != null )? systemCode.Description : "!!:" + code);  
    }

    public static int DisplaySequence(string code, string group)
    {
      if (code == null)
        return 0;

      var systemCode = SystemCodeList.FirstOrDefault(t => t.Code == code.ToUpper() && t.SystemGroup.Name == group);
      return ((systemCode != null) ? 
                (systemCode.DisplaySequence.HasValue? 
                   systemCode.DisplaySequence.Value : 0) : 0);
    }

  }
}
