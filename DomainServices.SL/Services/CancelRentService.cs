using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CancelRentService 
  {

    public static async Task<decimal> ExecuteAsync(int bookingid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.CancelRent, bookingid);
      return (decimal) result;

    }

  }
}
