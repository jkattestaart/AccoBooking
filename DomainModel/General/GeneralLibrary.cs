using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using Geocoding;
using Geocoding.Google;
using IdeaBlade.EntityModel;

namespace DomainModel
{
  /// <summary>
  /// Library routines general 
  /// </summary>
  public static class GeneralLibrary
  {
    public static string CurrentUpload;

    public static int NextValue(AccoBookingEntities em, string seq)
    {
      var sequence = em.Sequences.FirstOrDefault(s => s.Name == seq);
      if (sequence != null)
        sequence.CurrentId += 1;
      return sequence.CurrentId;
    }

    /// <summary>
    /// Send email to receipient
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="em"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static bool SendMail(IPrincipal principal, EntityManager em, params object[] args)
    {
      //var emailMessage = args[0] as EmailMessage;
      bool isEmailSendSuccessfully = false;

      try
      {
        var from = new MailAddress(args[1] as string, args[8] as string);
        var to = new MailAddress(args[0] as string);
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = args[2] as string;
        mailMessage.Body = args[3] as string;
        //MailMessage mailMessage = new MailMessage("accobooking@mail.nl", "jkattestaart@gmail.com");
        //mailMessage.Subject = "TEST";
        //mailMessage.Body = "Dit is een test";

        if (!string.IsNullOrEmpty(args[4] as string))
        {
          var attachment = new Attachment(CurrentUpload);
          //var attachment = new Attachment(@"temp\" + args[4]);
          mailMessage.Attachments.Add(attachment);
        }


        
        mailMessage.IsBodyHtml = true;

        var smtp = CreateSmtpClient(args[7] as string, args[5] as string, args[6] as string);
   

        smtp.Send(mailMessage);
        isEmailSendSuccessfully = true;

      }
      catch (Exception ex)
      {
        isEmailSendSuccessfully = false;
      }

      return isEmailSendSuccessfully;
    }

    /// <summary>
    /// Create a smtp client to send email
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private static SmtpClient CreateSmtpClient(string provider, string user, string password)
    {
      var smtp = new SmtpClient();

      switch (provider)
      {
        case "GMAIL":
          smtp.Host = "smtp.gmail.com";
          smtp.Port = 587;
          smtp.EnableSsl = true;
          smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
          smtp.UseDefaultCredentials = false;
          //Credentials = new NetworkCredential("jkattestaart", "D9vQAxGosAumaLP78F2Q")
          smtp.Credentials = new NetworkCredential(user, password);
          break;

        case "LIVE":
          smtp.Host = "smtp.live.com";
          smtp.Port = 587;
          smtp.EnableSsl = true;
          smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
          smtp.UseDefaultCredentials = false;
          smtp.Credentials = new NetworkCredential(user, password);
          break;

        case "YAHOO":
          smtp.Host = "smtp.mail.yahoo.com";
          smtp.Port = 587;
          smtp.EnableSsl = true;
          smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
          smtp.UseDefaultCredentials = false;
          smtp.Credentials = new NetworkCredential(user, password);
          break;

        //TODO other providers: UPC, ZIGGO, ...

      }
      return smtp;
    }

    [AllowRpc]
    public static string GeoCode(IPrincipal principal, EntityManager em, params object[] args)
    {
      IGeocoder geocoder = new GoogleGeocoder() {ApiKey = "AIzaSyBzqa3PHsaptqCmUFJle9VUAz8k69ukKYY"};
      //IEnumerable<Address> addresses = geocoder.Geocode("1600 pennsylvania ave washington dc");
      //Console.WriteLine("Formatted: " + addresses.First().FormattedAddress);
      //Formatted: 1600 Pennslyvania Avenue Northwest, Presiden'ts Park, Washington, DC 20500, USA
      //Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude); //Coordinates: 38.8978378, -77.0365123
      
      var addresses = geocoder.Geocode(args[0] as string);
      if (addresses.Count() > 0)
      {
        var address = addresses.First();
        return address.Coordinates.Latitude + ";" + address.Coordinates.Longitude;
      }
      return"";
    }
  }
}
