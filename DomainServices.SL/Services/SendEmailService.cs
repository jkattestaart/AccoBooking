using System;
using System.Threading.Tasks;
using DomainModel;

using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public class SendEmailService
  {
    public static async Task ExecuteAsync(string to, string from, string subject, string body, string attach, string user,
      string password, string provider, string name)
    {
      EntityManager mgr = new AccoBookingEntities();

      var result = await mgr.InvokeServerMethodAsync(Library.General, Method.SendMail,
        to,
        from,
        subject,
        body,
        attach,
        user,
        password,
        provider,
        name
        );
      var ok = (bool) result;

      if (!ok)
        throw (new Exception("Error in SendMail"));

    }
  }
}
