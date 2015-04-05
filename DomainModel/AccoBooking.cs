using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IdeaBlade.EntityModel;
using IdeaBlade.Validation;

namespace DomainModel
{
  public static class BookingStatus
  {
    public const string Reserved = "RESERVED";
    public const string Expired = "EXPIRED";
    public const string Confirmed = "CONFIRMED";
    public const string Paid = "PAID";
    public const string Cancelled = "CANCELLED";
    public const string Closed = "CLOSED";
  }

  public static class SystemGroupName
  {
    public const string Acco = "ACCO";
    public const string Period = "PERIOD";
    public const string Booking = "BOOKING";
    public const string PeriodUnit = "PERIODUNIT";
    public const string Weekday = "WEEKDAY";
    public const string Unit = "UNIT";
    public const string Milestone = "MILESTONE";
    public const string BookingStatus = "BOOKINGSTATUS";
    public const string Country = "COUNTRY";
    public const string Language = "LANGUAGE";
    public const string MailContext = "MAILCONTEXT";
    public const string Gender = "GENDER";
    public const string MailTemplate = "MAILTEMPLATE";
    public const string Currency = "CURRENCY";
    public const string AccoType = "ACCOTYPE";
  }

  public static class UnitName
  {
    public const string Booking = "BOOKING";
    public const string Night = "NIGHT";
    public const string Person = "PERSON";
    public const string PersonPerNight = "PERSON-PER-NIGHT";
    public const string Adult = "ADULT";
    public const string AdultPerNight = "ADULT-PER-NIGHT";
    public const string Child = "CHILD";
    public const string ChildPerNight = "CHILD-PER-NIGHT";
    public const string Pet = "PET";
    public const string PetPerNight = "PET-PER-NIGHT";
    public const string GasM3 = "GASM3";
    public const string WaterM3 = "WATERM3";
    public const string KWh = "KWH";
  }

  public static class PeriodUnitName
  {
    public const string Week = "WEEK";
    public const string Weekend = "WEEKEND";
    public const string MidWeek = "MIDWEEK";
    public const string Night = "NIGHT";
  }

  public static class ReminderContext
  {
    public const string Booking = "BOOKING";
    public const string Payment = "PAYMENT";
    public const string Reminder = "REMINDER";
    public const string License = "LICENSE";
  }

  public static class RentType
  {
    public const string Night = "NIGHT";
    public const string Weekend = "WEEKEND"; 
    public const string Week = "WEEK";
  }

  public static class MailTemplateType
  {
    public const string PeriodAvailable = "PERIODAVAILABLE";
    public const string Reservation = "RESERVATION";
    public const string Confirmation = "CONFIRMATION";
    public const string PaymentReminder = "PAYMENTREMINDER";
    public const string PaymentReceived = "PAYMENTRECEIVED";
    public const string Arrival = "ARRIVAL";
    public const string Departure = "DEPARTURE";
    public const string Cancellation = "CANCELLATION";
    public const string Expiration = "EXPIRATION";
  }

  public static class MileStone
  {
    public const string Arrival = "ARRIVAL";
    public const string FirstPayment = "FIRST-PAYMENT";
    public const string LastPayment = "LAST-PAYMENT";
    public const string Departure = "DEPARTURE";
  }

  public static class SequenceName
  {
    public const string AccoOwnerId = "AccoOwnerId";
    public const string AccoId = "AccoId";
    public const string AccoDescriptionId = "AccoDescriptionId";
    public const string AccoAdditionId = "AccoAdditionId";
    public const string AccoAdditionDescriptionId = "AccoAdditionDescriptionId";
    public const string AccoReminderId = "AccoReminderId";
    public const string AccoNotificationId = "AccoNotificationId";
    public const string AccoRentId = "AccoRentId";
    public const string AccoSeasonId = "AccoSeasonId";
    public const string AccoPayPatternId = "AccoPayPatternId";
    public const string AccoPayPatternPaymentId = "AccoPayPatternPaymentId";
    public const string AccoSpecialOfferId = "AccoSpecialOfferId";
    public const string AccoCancelConditionId = "AccoCancelConditionId";
    public const string AccoTrusteeId = "AccoTrusteeId";

    public const string BookingId = "BookingId";
    public const string BookingGuestId = "BookingGuestId";
    public const string BookingAdditionId = "BookingAdditionId";
    public const string BookingReminderId ="BookingReminderId";
    public const string BookingPaymentId = "BookingPaymentId";
    public const string BookingCancelConditionId = "BookingCancelConditionId";

    public const string MailTemplateId = "MailTemplateId";
    public const string SystemCodeId = "SystemCodeId";
    public const string SystemGroupId = "SystemGroupId";
    public const string CurrencyId = "CurrencyId";
    public const string CountryId = "CountryId";
    public const string LanguageId = "LanguageId";

    //obsolete
    public const string AccoAvailablePeriodId = "AccoAvailablePeriodId";
    public const string AccoDefaultRentId = "AccoDefaultRentId";
    public const string MailTemplateContentId = "MailTemplateContentId";
    public const string PeriodUnitId = "PeriodUnitId"; 
  }

  public static class Library
  {
    public const string Acco = "DomainModel.AccoLibrary,DomainModel";
    public const string Booking = "DomainModel.BookingLibrary,DomainModel";
    public const string General = "DomainModel.GeneralLibrary,DomainModel";
  }

  public static class Method
  {
    public const string Subscribe = "Subscribe";
    public const string ApplyPayPattern = "ApplyPayPattern";
    public const string AvailableDepartures = "AvailableDepartures";
    public const string BuildMailTemplates = "BuildMailTemplates";
    public const string CancellationRefund = "CancellationRefund";
    public const string CancelRent = "CancelRent";
    public const string CheckReminders = "CheckReminders";
    public const string CopyRent = "CopyRent";
    public const string CopyAcco = "CopyAcco";
    public const string CreateBooking = "CreateBooking";
    public const string DuplicateAcco = "DuplicateAcco";
    public const string SendMail = "SendMail";
    public const string UpdateBooking = "UpdateBooking";
    public const string GeoCode = "GeoCode";
  }

  public partial class AccoBookingEntities
  {
    //Named queries
    public EntityQuery<Booking> ExpiredBookings
    {
      get { return new EntityQuery<Booking>("ExpiredBookings", this); }
    }

  }


  public partial class Acco
  {
    [NotMapped]
    public List<AccoDescription> TypeDescriptions
    {
      get { return AccoDescriptions.Where(ad => ad.PropertyName == "AccoType").ToList(); }
    }

    [NotMapped]
    public List<AccoDescription> ArriveAfterDescriptions
    {
      get { return AccoDescriptions.Where(ad => ad.PropertyName == "ArriveAfter").ToList(); }
    }

    /// <summary>
    /// 
    /// </summary>
    [NotMapped]
    public List<AccoDescription> DepartureBeforeDescriptions
    {
      get { return AccoDescriptions.Where(ad => ad.PropertyName == "DepartureBefore").ToList(); }
    }

    [NotMapped]
    public string CountryDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Country + "_" + Country.Description.ToUpper()); }
    }
    
    [NotMapped]
    public string CurrencyDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Currency + "_" + Currency.ToUpper()); }
    }
     
    public string TypeDescription(int languageid)
    {
      {
        return
          AccoDescriptions.FirstOrDefault(
            d => d.PropertyName == "AccoType" && d.LanguageId == languageid).Description;
      }
    }

    public string ArriveAfterDescription(int languageid)
    {
      {
        return
          AccoDescriptions.FirstOrDefault(
            d => d.PropertyName == "ArriveAfter" && d.LanguageId == languageid).Description;
      }
    }

    public string DepartureBeforeDescription(int languageid)
    {
      {
        return
          AccoDescriptions.FirstOrDefault(
            d => d.PropertyName == "DepartureBefore" && d.LanguageId == languageid).Description;
      }
    }

    [NotMapped]
    public int Zoom { get; set; }

    [NotMapped]
    public string Map
    {
      get
      {
        return string.Format(
          "<img src=\"https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&amp;zoom={2}&amp;size=270x270&amp;markers=color:red%7Clabel:Acco%7C{0},{1}\">",
         Latitude, Longitude, Zoom);
      }
    }


  }

  //// extras per entity
  //public partial class AccoAvailablePeriod
  //{
  //  public string ArrivalWeekDay
  //  {
  //    get { return SessionManager.GetString(SystemGroupName.Weekday + "_" + Arrival.DayOfWeek.ToString().ToUpper()); }
  //  }

  //  public string DepartureWeekDay
  //  {
  //    get { return SessionManager.GetString(SystemGroupName.Weekday + "_" + Departure.DayOfWeek.ToString().ToUpper()); }
  //  }

  //  public decimal RentPerNight
  //  {
  //    get
  //    {
  //      if ((Rent > 0) && (Nights > 0))
  //        return Rent / Nights;

  //      return 0;
  //    }
  //  }

  //  public string PeriodUnitDescription
  //  {
  //    get { return SessionManager.GetString(SystemGroupName.PeriodUnit + "_" + PeriodUnit.Description.ToUpper()); }
  //  }

  //  public override void Validate(VerifierResultCollection validationErrors)
  //  {
  //    if (Departure <= DateTime.Now)
  //      validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
  //                                              SessionManager.GetString("AccoAvailablePeriod_Departure_Future"),
  //                                              "AccoAvailablePeriod"));
  //  }
  //}

  //public partial class AccoDefaultRent
  //{
  //  public string PeriodUnitDescription
  //  {
  //    get { return SessionManager.GetString(SystemGroupName.PeriodUnit + "_" + PeriodUnit.Description.ToUpper()); }
  //  }

  //  public override void Validate(VerifierResultCollection validationErrors)
  //  {
  //    if (PeriodUnit.IsWeekBased && ArrivalWeekday == "")
  //      validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
  //                                              SessionManager.GetString("Period_ArrivalWeekDay_Required"),
  //                                              "AccoDefaultRent"));
  //  }
  //}

  public partial class AccoSeason
  {
    public override void Validate(VerifierResultCollection validationErrors)
    {
      if (AccoRentId == 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
          SessionManager.GetString("AccoSeason_AccoRent_Required"),   
          "AccoSeason"));

      if (SeasonStart.Date > SeasonEnd.Date)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
          SessionManager.GetString("AccoSeason_SeasonStart_SeasonEnd"),
          "AccoSeason"));

      if (Acco.AccoSeasons.Any(s => s.SeasonStart.Date >= SeasonStart.Date && s.SeasonStart.Date <= SeasonEnd.Date && s.AccoSeasonId != AccoSeasonId) ||
          Acco.AccoSeasons.Any(s => s.SeasonEnd.Date >= SeasonStart.Date && s.SeasonEnd.Date <= SeasonEnd.Date && s.AccoSeasonId != AccoSeasonId))
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
          SessionManager.GetString("AccoSeason_SeasonOverlap"),
          "AccoSeason"));

      //Geen check meer op jaar
      //if (SeasonStart.Date < new DateTime(SeasonYear,1,1).Date || SeasonEnd.Date > new DateTime(SeasonYear,12,31).Date)
      //  validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
      //    SessionManager.GetString("AccoSeason_Season_Year"),
      //    "AccoSeason"));

      //zit nu in combo-box
      //if (SeasonYear != AccoRent.RentYear)
      //  validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
      //    SessionManager.GetString("AccoSeason_SeasonYear_RentYear"),
      //    "AccoRent"));
    }
  }

  public partial class AccoSpecialOffer
  {
    [NotMapped]
    public string LanguageDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Language + "_" + Language.Description.ToUpper()); }
    }
  }

  public partial class Booking
  {
    [NotMapped]
    public bool IsValid { get { return Status != BookingStatus.Expired && Status != BookingStatus.Cancelled; } }

    [NotMapped]
    public string CountryDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Country + "_" + Country.Description.ToUpper()); }
    }

    public override void Validate(VerifierResultCollection validationErrors)
    {
      // Email or Phone must have a value to enable the owner to contact the guest.
      if (String.IsNullOrEmpty(BookerEmail) && String.IsNullOrEmpty(BookerPhone))
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("Booker_EmailPhone_Required"), "Email"));

      if (!String.IsNullOrEmpty(BookerEmail))
      {
        Regex r = new Regex(@"^[a-z0-9][\w\.-]*[a-z0-9]@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

        // Check e-mail format
        // @@@ JKT OKWarnings lijken niet afgevangen te worden in cocktail. Voorlopig error hanteren
        if (!r.Match(BookerEmail).Success)
          validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("Booker_Email_Format"), "Email"));
      }

      if (Adults < 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                SessionManager.GetString("Booking_Adults"),
                                                "Booking"));

      if (Children < 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                SessionManager.GetString("Booking_Children"),
                                                "Booking"));

      if (Pets < 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                SessionManager.GetString("Booking_Pets"),
                                                "Booking"));
      
      // Validate only active bookings
      if (Status == BookingStatus.Reserved || Status == BookingStatus.Confirmed || Status == BookingStatus.Paid)
      {
        // The arrival must be before the departure.
        if (Arrival >= Departure)
          validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                  SessionManager.GetString("Booking_Arrival"),
                                                  "Booking"));

        // The deparure must be in the future
        if (Status == BookingStatus.Reserved && Departure < DateTime.Now)
          validationErrors.Add(new VerifierResult(VerifierResultCode.OkWarning,
                                                  SessionManager.GetString("Booking_Departure_Future"),
                                                  "Booking"));

        // The booking may not overlap with an existing booking
        var booking2 = Acco.Bookings.FirstOrDefault(b => b.BookingId != BookingId && b.IsValid &&
                                                         b.Departure > Arrival && b.Arrival < Departure);

        var a = Thread.CurrentThread.CurrentCulture;
        if (booking2 != null)
          validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                  SessionManager.GetString("Booking_Overlap"),
                                                  "Booking"));
      }
    }

    public BookingPayment AddBookingPayment(int bookingpaymentid, bool isPaymentByGuest, bool isScheduledPayment)
    {
      BookingPayment payment = BookingPayment.Create(bookingpaymentid);
      payment.IsPaymentByGuest = isPaymentByGuest;
      payment.IsScheduledPayment = isScheduledPayment;
      BookingPayments.Add(payment);

      return payment;
    }

    public void DeleteBookingPayment(BookingPayment payment)
    {
      payment.EntityAspect.Delete();
    }

    public int Nights
    {
      get
      {
        return Departure.Subtract(Arrival).Days;
      }

    }

    public decimal SubTotal
    {
      get { return Rent + Additions; }     // rent plus additions, the income for the owner
    }

    public decimal Total
    {
      get { return SubTotal + Deposit; }  // total amount to be paid by guest
    }

    public DateTime Due
    {
      get { return Booked.AddDays(Acco.DaysToExpire); }
    }

  }

  public partial class BookingPayment
  {
    public static BookingPayment Create(int bookingpaymentid)
    {
      return new BookingPayment { BookingPaymentId = bookingpaymentid };
    }
  }

  public partial class AccoDescription
  {
    [NotMapped]
    public string LanguageDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Language + "_" + Language.Description.ToUpper()); }
    }

  }

  [MetadataType(typeof(AccoAdditionMetaData))]
  public partial class AccoAddition
  {
    public override void Validate(VerifierResultCollection validationErrors)
    {

      //@@@ JKT omdat de unit via een combo-box control-template op het scherm staat werkt de validatie niet meer zo lekker
      //Nakijken of dit via de combobox kan onderstaande geeft dubbele melding
      //if (Unit == "")
      //{
      //  validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
      //    SessionManager.GetString("AccoAddition_Unit_Required"), "Unit"));
      //}
    }

    public string Description
    {
      get
      {
        return
          AccoAdditionDescriptions.FirstOrDefault(
            d => d.LanguageId == SessionManager.BookingLanguageId).Description;
      }
    }

    public AccoAdditionDescription AddDescription(Language language)
    {
      AccoAdditionDescription description = AccoAdditionDescription.Create(language);
      AccoAdditionDescriptions.Add(description);

      return description;
    }
  }


  public partial class AccoAdditionDescription
  {

    [NotMapped]
    public string LanguageDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Language + "_" + Language.Description.ToUpper()); }
    }

    public static AccoAdditionDescription Create(Language language)
    {
      return new AccoAdditionDescription { Language = language };
    }
  }

  //public partial class MailTemplate
  //{
  //  public MailTemplateContent AddContent(Language language)
  //  {
  //    MailTemplateContent content = MailTemplateContent.Create(language);
  //    MailTemplateContents.Add(content);

  //    return content;
  //  }

  //}

  //public partial class MailTemplateContent
  //{
  //  public static MailTemplateContent Create(Language language)
  //  {
  //    return new MailTemplateContent { Language = language };
  //  }

  //  [NotMapped]
  //  public string LanguageDescription
  //  {
  //    get { return SessionManager.GetString(SystemGroupName.Language + "_" + Language.Description.ToUpper()); }
  //  }

  //}

  // @@@ JKT werkt niet, nog uitzoeken
  //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
  //public class MyRequiredVerifierAttribute : PropertyValueVerifierAttribute
  //{
  //  protected override Verifier BuildVerifierCore(Type pType, String propertyName)
  //  {
  //    return new MyRequiredVerifier(pType, propertyName);
  //  }
  //}
  //public class MyRequiredVerifier : PropertyValueVerifier
  //{

  //  public MyRequiredVerifier(Type entityType,
  //                            String propertyName,
  //                            String displayName = null,
  //                            bool? shouldTreatEmptyStringAsNull = null
  //    )
  //    : base(new PropertyValueVerifierArgs(entityType, propertyName, false,
  //                                         displayName, shouldTreatEmptyStringAsNull))
  //  {
  //  }

  //  protected override VerifierResult VerifyValue(object itemToVerify, object valueToVerify,
  //                                                TriggerContext triggerContext, VerifierContext verifierContext)
  //  {
  //    if (itemToVerify is string)
  //      return new VerifierResult(!String.IsNullOrEmpty((string)valueToVerify));

  //    if (itemToVerify is int)
  //      return new VerifierResult((int)valueToVerify != 0);

  //    return new VerifierResult(true);
  //  }
  //}


  [MetadataType(typeof(AccoOwnerMetaData))]
  public partial class AccoOwner
  {

    [NotMapped]
    public string LanguageDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Language + "_" + Language.Description.ToUpper()); }
    }

    [NotMapped]
    public string CountryDescription
    {
      get { return SessionManager.GetString(SystemGroupName.Country + "_" + Country.Description.ToUpper()); }
    }

    public override void Validate(VerifierResultCollection validationErrors)
    {
      Regex r = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-zA-Z]).*$", RegexOptions.IgnoreCase);

      // Match the regular expression pattern against a text string.
      if (!r.Match(Password).Success)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, AccoResource.Acco_Password, "Password"));
    
    }
  }


  public class AccoAdditionMetaData
  {
    [RequiredValueVerifier(ErrorMessageResourceName = "AccoAddition_Unit_Required", ErrorMessageResourceType = typeof(AccoResource))]
    public static string Unit;

  }

  public class AccoOwnerMetaData
  {
    [RequiredValueVerifier(ErrorMessageResourceName = "Acco_Login_Required", ErrorMessageResourceType = typeof(AccoResource))]
    public static string Login;

    [RequiredValueVerifier(ErrorMessageResourceName = "Acco_Password_Required", ErrorMessageResourceType = typeof(AccoResource))]
    public static string Password;
  }

  [MetadataType(typeof(AccoTrusteeMetaData))]
  public partial class AccoTrustee
  {
    public override void Validate(VerifierResultCollection validationErrors)
    {
      Regex r = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-zA-Z]).*$", RegexOptions.IgnoreCase);

      // Match the regular expression pattern against a text string.
      if (!r.Match(Password).Success)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, AccoResource.Acco_Password, "Password"));
    }
  }

  public class AccoTrusteeMetaData
  {
    [RequiredValueVerifier(ErrorMessageResourceName = "Acco_Login_Required", ErrorMessageResourceType = typeof(AccoResource))]
    public static string Login;

    [RequiredValueVerifier(ErrorMessageResourceName = "Acco_Password_Required", ErrorMessageResourceType = typeof(AccoResource))]
    public static string Password;
  }


  public partial class AccoPayPattern
  {
    public override void Validate(VerifierResultCollection validationErrors)
    {
      decimal total = 0;

      if (EntityAspect.EntityState.IsModified())
      {
        foreach (var payment in AccoPayPatternPayments)
          total += payment.PaymentPercentage;

        if (AccoPayPatternPayments.Count > 0 && decimal.Compare(total, 100) != 0)
          validationErrors.Add(new VerifierResult(VerifierResultCode.Error,
                                                  SessionManager.GetString("AccoPayPatternPayment_Total100"), "Payment"));
      }
    }
  }

  public partial class AccoPayPatternPayment
  {
    public override void Validate(VerifierResultCollection validationErrors)
    {
      // Percentage or amount must be filled
      if (PaymentPercentage <= 0 && PaymentAmount <= 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoPayPatternPayment_Percentage_Required"), "Percentage"));

      // If percentage is filled, amount must be 0
      if (PaymentPercentage > 0 && PaymentAmount > 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoPayPatternPayment_Amount"), "Percentage"));

      // Days after booking or days before arrival must be filled
      if (DaysToPayAfterBooking <= 0 && DaysToPayBeforeArrival <= 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoPayPatternPayment_Days_Required"), "Days"));

      // If days after booking  is filled, days before arrival must be 0
      if (DaysToPayAfterBooking > 0 && DaysToPayBeforeArrival > 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoPayPatternPayment_Days"), "Days"));
    }
  }

  public partial class AccoRent
  {
    [NotMapped]
    public string RentDescription
    {
      get { return Description + "(" + RentYear.ToString() + ")"; }
    }

    public override void Validate(VerifierResultCollection validationErrors)
    {
      if (IsAvailablePerWeek && RentPerWeek == 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoRent_RentPerWeek_Required"), "RentPerWeek"));

      if (IsAvailablePerWeek && string.IsNullOrEmpty(WeekExchangeDay))
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoRent_WeekExchangeDay_Required"), "WeekExchangeDay"));

      if (IsAvailablePerWeekend && RentPerWeekend == 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoRent_RentPerWeekend_Required"), "RentPerWeekend"));

      if (IsAvailablePerWeekend && RentPerMidweek == 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoRent_RentPerMidweek_Required"), "RentPerMidweek"));
    
      if (IsAvailablePerNight && RentPerNight == 0)
        validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("AccoRent_RentPerNight_Required"), "RentPerNight"));   
    }
  }

  public partial class BookingGuest
  {

    public override void Validate(VerifierResultCollection validationErrors)
    {
      if (!String.IsNullOrEmpty(Email))
      {
        Regex r = new Regex(@"^[a-z0-9][\w\.-]*[a-z0-9]@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

        // Check e-mail format
        // @@@ JKT OKWarnings lijken niet afgevangen te worden in cocktail. Voorlopig error hanteren
        if (!r.Match(Email).Success)
          validationErrors.Add(new VerifierResult(VerifierResultCode.Error, SessionManager.GetString("Guest_Email_Format"), "Email"));
      }

    }
  }


  public partial class BookingAddition
  {
    public decimal Amount
    {
      get
      {
        switch (Unit)
        {
          case UnitName.Booking:
            return Price;

          case UnitName.Night:
            return Booking.Nights * Price;

          case UnitName.Person:
            return (Booking.Adults + Booking.Children) * Price;

          case UnitName.PersonPerNight:
            return (Booking.Adults + Booking.Children) * Booking.Nights * Price;

          case UnitName.Adult:
            return Booking.Adults * Price;

          case UnitName.AdultPerNight:
            return Booking.Adults * Booking.Nights * Price;

          case UnitName.Child:
            return Booking.Children * Price;

          case UnitName.ChildPerNight:
            return Booking.Children * Booking.Nights * Price;

          case UnitName.Pet:
            return Booking.Pets * Price;

          case UnitName.PetPerNight:
            return Booking.Pets * Booking.Nights * Price;

          case UnitName.GasM3:
            return (Booking.DepartureGas - Booking.ArrivalGas) * Price;

          case UnitName.WaterM3:
            return (Booking.DepartureWater - Booking.ArrivalWater) * Price;

          case UnitName.KWh:
            return (Booking.DepartureElectricity - Booking.ArrivalElectricity) * Price;

          default:
            return 0;
        }

      }

    }

  }
}