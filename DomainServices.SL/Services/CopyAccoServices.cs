using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CopyAccoService
  {
    public static async Task ExecuteAsync(int fromaccoid, int toaccoid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Acco, Method.CopyAcco, fromaccoid, toaccoid);

      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));
    }
  }
}
