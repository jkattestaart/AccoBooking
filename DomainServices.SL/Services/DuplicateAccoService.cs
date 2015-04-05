using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class DuplicateAccoService
  {



    public static async Task ExecuteAsync(int fromaccoid, int toaccoid, string language)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Acco, Method.DuplicateAcco, fromaccoid, toaccoid, language);

      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));
    }
  }
}
