using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class ApplyPayPatternService 
  {
    public static async Task ExecuteAsync(int bookingid, int patternid)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.Booking, Method.ApplyPayPattern, bookingid, patternid);

      var message = (string)result;

      if (!String.IsNullOrEmpty(message))
        throw (new Exception(message));
    }
 
  }
}
