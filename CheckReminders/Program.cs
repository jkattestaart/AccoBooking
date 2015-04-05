using System;
using DomainModel;
using IdeaBlade.EntityModel.Security;

namespace CheckReminders
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {

      var em = new AccoBookingEntities();

      // Login will null credentials.  The LoginManager will decide what to do ...
      Authenticator.Instance.DefaultAuthenticationContext = Authenticator.Instance.Login();
      BookingLibrary.CheckReminders(em.AuthenticationContext.Principal, em, new object[] { });

      }
      catch (Exception ex)
      {

        throw ex;
      }

    }
  }
}
