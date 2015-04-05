using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class CancellationRefundService 
  {  
    public static async Task<decimal> ExecuteAsync(int bookingid)
    {
      EntityManager mgr = new AccoBookingEntities();
      
      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.CancellationRefund, bookingid);
      return (decimal) result;

    }

  }
}
