using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class GeoCodingService
  {
    public static async Task<string> ExecuteAsync(string address)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.General, Method.GeoCode, address);
      return result as string;
    }
  }
}
