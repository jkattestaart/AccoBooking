using System;
using System.Data.Objects.SqlClient;
using System.Linq;
using IdeaBlade.Core.DomainServices;
using IdeaBlade.EntityModel;

namespace DomainModel
{
  [EnableClientAccess]
  public class NamedQueryServiceProvider
  {
    public IQueryable<Booking> GetExpiredBookings()
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

      //@@@ jkt sqlfunctions gebruiken mbv named query
      //x.Created.Value,AddDays(x.Acco.DaysToExpire).CompareTo(DateTime.Now) < 0));     
      return new EntityQuery<Booking>()
        .Where(x => !x.IsConfirmed && x.Status != "CANCELLED" && x.Status != "EXPIRED" &&
                    SqlFunctions.DateDiff("Day", SqlFunctions.DateAdd("Day", x.Acco.DaysToExpire, x.Booked),
                                          offset) < 0
        );
    }

  }
}