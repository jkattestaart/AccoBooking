using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CreateBookingService 
  {
    public static async Task ExecuteAsync(int bookingid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.CreateBooking, bookingid);
      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));

    }

  }
}
