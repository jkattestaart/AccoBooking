using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CheckRemminderService
  {



    public static async Task ExecuteAsync(int ownerid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.CheckReminders, ownerid);

      if (!string.IsNullOrEmpty((string) result))
        throw (new Exception((string) result));
    }
  }
}
