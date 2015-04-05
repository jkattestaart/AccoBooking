using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CopyAccoRentService
  {
    public static async Task ExecuteAsync(int fromaccoid, int fromyear, int toaccoid, int toyear)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Acco, Method.CopyRent, fromaccoid, fromyear, toaccoid, toyear);

      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));

    }
  }
}
