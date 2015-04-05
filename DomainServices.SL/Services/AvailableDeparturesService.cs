using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class AvailableDeparturesService
  {
    public static async Task<IEnumerable<AvailableDepartureListItem>> ExecuteAsync(int accoId, DateTime arrivalOn)
    {
      EntityManager mgr = new AccoBookingEntities();

      //IEnumerable<AvailableDepartureListItem> AvailableDepartures
      var result = await mgr.InvokeServerMethodAsync(Library.Acco, Method.AvailableDepartures, accoId, arrivalOn);

      return result as IEnumerable<AvailableDepartureListItem>;
    }
  }
}
