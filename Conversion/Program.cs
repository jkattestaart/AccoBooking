using System;
using System.Security.Cryptography;
using DomainModel;
using IdeaBlade.EntityModel.Security;

namespace Conversion
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      try
      {

        var em = new AccoBookingEntities();

        // Login will null credentials.  The LoginManager will decide what to do ...
        Authenticator.Instance.DefaultAuthenticationContext = Authenticator.Instance.Login();

        foreach (var acco in em.Accoes)
        {
          if (string.IsNullOrEmpty(acco.Address1))
          {
            acco.Address1 = "";
            acco.Address2 = "";
            acco.Address3 = "";
          }
          if (acco.Latitude == null)
            acco.Latitude = 0;
          if (acco.Longitude == null)
            acco.Longitude = 0;
        }

        foreach (var booking in em.Bookings)
        {
          if (booking.BookerLatitude == null)
            booking.BookerLatitude = 0;
          if (booking.BookerLongitude == null)
            booking.BookerLongitude = 0;
        }

        em.SaveChanges();
        
      }

      catch (Exception ex)
      {

        throw ex;
      }

    }
  }
}
