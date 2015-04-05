using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class UpdateBookingService 
  {

    public static async Task ExecuteAsync(int bookingid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.UpdateBooking, bookingid);

      var message = (string) result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));
    }
  }
}
