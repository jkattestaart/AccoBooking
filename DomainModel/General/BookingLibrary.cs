using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace DomainModel
{
  /// <summary>
  /// Library routines booking
  /// </summary>
  public class BookingLibrary
  {
    /// <summary>
    /// Create a booking addition
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static BookingAddition CreateBookingAddition(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.BookingAdditionId);

      var addition = new BookingAddition();
      em.AddEntity(addition);
      addition.BookingAdditionId = sequence;

      return addition;
    }

    /// <summary>
    /// Create a booking cancelcondition
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static BookingCancelCondition CreateBookingCancelCondition(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.BookingCancelConditionId);

      var condition = new BookingCancelCondition();
      em.AddEntity(condition);
      condition.BookingCancelConditionId = sequence;

      return condition;
    }

    /// <summary>
    /// Create a booking payment
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static BookingPayment CreateBookingPayment(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.BookingPaymentId);

      var payment = new BookingPayment();
      em.AddEntity(payment);
      payment.BookingPaymentId = sequence;
      payment.IsPaid = false;

      return payment;
    }
    
    /// <summary>
    /// Create a booking reminder
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static BookingReminder CreateBookingReminder(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.BookingReminderId);

      var reminder = new BookingReminder();
      em.AddEntity(reminder);
      reminder.BookingReminderId = sequence;

      return reminder;
    }

    /// <summary>
    /// Create a booking, using the acco definition as defaults
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string CreateBooking(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      try
      {
        var em = (AccoBookingEntities) entitymanager;

        var booking = em.Bookings.FirstOrDefault(b => b.BookingId == (int) args[0]);
        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == booking.AccoId);
        var language = em.Languages.FirstOrDefault(l => l.LanguageId == booking.BookerLanguageId);

        var additions =
          em.AccoAdditions.Where(a => a.AccoId == acco.AccoId).Include(a => a.AccoAdditionDescriptions).ToList();
        var conditions = em.AccoCancelConditions.Where(c => c.AccoId == acco.AccoId).ToList();
        var reminders = em.AccoReminders.Where(c => c.AccoId == acco.AccoId).ToList();

        booking.CancelAdministrationCosts = acco.CancelAdministrationCosts;
        booking.DaysToPayDepositBackAfterDeparture = acco.DaysToPayDepositBackAfterDeparture;
        booking.Deposit = acco.Deposit;
        booking.Additions = 0;

        foreach (var addition in additions.Where(a => a.IsDefaultBooked))
        {

          var bookingAddition = CreateBookingAddition(em);

          var description =
            addition.AccoAdditionDescriptions.FirstOrDefault(ad => ad.LanguageId == language.LanguageId);
          if (description != null) bookingAddition.Description = description.Description;

          bookingAddition.AccoAdditionId = addition.AccoAdditionId;
          bookingAddition.Price = addition.Price;
          bookingAddition.Unit = addition.Unit;
          bookingAddition.DisplaySequence = addition.DisplaySequence;
          bookingAddition.IsPaidFromDeposit = addition.IsPaidFromDeposit;

          bookingAddition.Booking = booking;
          booking.Additions = booking.Additions + bookingAddition.Amount;
        }

        foreach (var reminder in reminders)
        {
          var bookingReminder = CreateBookingReminder(em);

          bookingReminder.Description = reminder.Description;
          bookingReminder.Milestone = reminder.Milestone;
          bookingReminder.Offset = reminder.Offset;
          bookingReminder.DisplaySequence = reminder.DisplaySequence;
          //rest wordt verwerkt bij het opslaan

          bookingReminder.Booking = booking;
        }

        foreach (var condition in conditions)
        {
          var bookingCondition = CreateBookingCancelCondition(em);

          bookingCondition.DaysBeforArrival = condition.DaysBeforeArrival;
          bookingCondition.RentPercentageToPay = condition.RentPercentageToPay;
          //rest wordt verwerkt bij het opslaan

          //Foreign keys
          bookingCondition.Booking = booking;
        }

        var days = (booking.Arrival.Date - DateTime.Now.Date).TotalDays;

        var pattern =
          acco.AccoPayPatterns.Where(p => p.DaysBeforeArrival.HasValue && p.DaysBeforeArrival.Value >= days)
            .OrderBy(p => p.DaysBeforeArrival).FirstOrDefault();
        var patternid = pattern != null ? pattern.AccoPayPatternId : acco.DefaultPayPatternId.Value;

        var msg = ApplyPayPatternToBooking(em, patternid, booking.BookingId);
        if (!string.IsNullOrEmpty(msg))
          return msg;

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }

    }

    /// <summary>
    /// Calculate the rent to be paid for the booking on cancellation
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static decimal CancelRent(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      //bepaal aantal dag voor arrival => cancellationcost     bijv: 10 dagen = 75%
      //bepaal rest ahv percentage van huur:                   bijv: 75% van 500 = 350
      //bepaal wat gast betaald heeft en annuleringskosten     bijv: 850 en aanulering = 15
      //terugbetaald is dan: betaald - rest huur - kosten      bijv: 850 - 350 - 15 = 485

      var em = (AccoBookingEntities) entitymanager;

      var booking = em.Bookings.FirstOrDefault(b => b.BookingId == (int) args[0]);
      decimal perc = 0;
      foreach (
        var condition in
          em.BookingCancelConditions.Where(c => c.BookingId == booking.BookingId)
            .OrderByDescending(c => c.DaysBeforArrival))
      {
        if (condition.DaysBeforArrival > (booking.Arrival - DateTime.Now).Days)
          perc = condition.RentPercentageToPay;
      }

      return booking.Rent*perc/100;

    }

    /// <summary>
    /// Calculate the refund for the guest for a cancelled booking
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static decimal CancellationRefund(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      //bepaal aantal dag voor arrival => cancellationcost     bijv: 10 dagen = 75%
      //bepaal rest ahv percentage van huur:                   bijv: 75% van 500 = 350
      //bepaal wat gast betaald heeft en annuleringskosten     bijv: 850 en aanulering = 15
      //terugbetaald is dan: betaald - rest huur - kosten      bijv: 850 - 350 - 15 = 485

      var em = (AccoBookingEntities) entitymanager;

      var booking = em.Bookings.FirstOrDefault(b => b.BookingId == (int) args[0]);

      decimal rent = CancelRent(principal, entitymanager, args);

      decimal paid = 0;
      foreach (var payment in em.BookingPayments.Where(p => p.BookingId == booking.BookingId &&
                                                            p.IsPaymentByGuest && !p.IsScheduledPayment))
        paid = paid + payment.Amount;

      return paid - rent - booking.CancelAdministrationCosts;
    }

    /// <summary>
    /// Update related data for a booking
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string UpdateBooking(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      var em = (AccoBookingEntities) entitymanager;

      try
      {
        var booking = em.Bookings.FirstOrDefault(b => b.BookingId == (int) args[0]);

        if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Expired)
        {
          //RemoveBookingFromAvailability(em, booking);
          em.SaveChanges();
          return "";
        }

        //Eerste keer naam overnemen van booker
        var guestTODO = booking.BookingGuests.FirstOrDefault(g => g.Name == "TODO");
        if (guestTODO != null)
        {
          if (booking.BookingGuests.Count > 1)
            guestTODO.EntityAspect.Delete();
          else
          {
            guestTODO.Name = booking.Booker;
            guestTODO.Email = booking.BookerEmail;
            guestTODO.Phone = booking.BookerPhone;
            guestTODO.Address1 = booking.BookerAddress1;
            guestTODO.Address2 = booking.BookerAddress2;
            guestTODO.Address3 = booking.BookerAddress3;
          }
        }

        //foreach (var period in em.AccoAvailablePeriods
        //  .Where(a => a.AccoId == booking.AccoId &&
        //              ((a.Arrival <= booking.Arrival && a.Departure > booking.Arrival) ||
        //               (a.Arrival < booking.Departure && a.Departure >= booking.Departure) ||
        //               (a.Arrival >= booking.Arrival && a.Departure <= booking.Departure)
        //                )
        //  )
        //  )
        //  period.IsBooked = true;

        var language = em.Languages.FirstOrDefault(l => l.LanguageId == booking.BookerLanguageId);
        var bookingAdditions = em.BookingAdditions.Where(a => a.BookingId == booking.BookingId);
        var accoAdditions =
          em.AccoAdditions.Where(a => a.AccoId == booking.AccoId).Include(d => d.AccoAdditionDescriptions);

        booking.Additions = 0;
        booking.Usage = 0;
        foreach (var bookingAddition in bookingAdditions)
        {
          booking.Additions = booking.Additions + bookingAddition.Amount;
          if (bookingAddition.IsPaidFromDeposit)
            booking.Usage = booking.Usage + bookingAddition.Amount;

          var addition = accoAdditions.FirstOrDefault(a => a.AccoAdditionId == bookingAddition.AccoAdditionId);
          if (addition != null)
            if (bookingAddition.Description == null)
              bookingAddition.Description = addition.AccoAdditionDescriptions
                .FirstOrDefault(d => d.LanguageId == language.LanguageId)
                .Description;
            else
            {
              foreach (var descr in addition.AccoAdditionDescriptions)
              {
                var de = descr.Description;
                var bd = bookingAddition.Description;
                var gelijk = (de == bd);
                if (descr.Description == bookingAddition.Description)
                {
                  bookingAddition.Description = addition.AccoAdditionDescriptions
                    .FirstOrDefault(d => d.LanguageId == language.LanguageId)
                    .Description;
                }
              }
            }

          //Todo: 
          //accoAddition = accoAdditions.FirstOrDefault(x => x.AccoAdditionId == addition.AccoAdditionId);

          // AccoAdditionDescription description = null;

          //if (accoAddition != null)
          //  description = em.AccoAdditionDescriptions.FirstOrDefault(ad => ad.AccoAdditionId == accoAddition.AccoAdditionId && ad.AccoLanguageId == language.AccoLanguageId);
          //if (description != null) addition.Description = description.Description;

        }

        //Status bijwerken indien paid
        if (booking.Status != BookingStatus.Closed)
        {
          // Bijwerken of amount correct scheduled is
          decimal scheduled = 0;
          foreach (var payment in em.BookingPayments.Where(p => p.BookingId == booking.BookingId &&
                                                                p.IsPaymentByGuest && p.IsScheduledPayment))
            scheduled += payment.Amount;

          booking.IsAmountExactlyScheduled = (booking.Total == scheduled + booking.Usage);


          decimal paid = 0;

          foreach (var payment in em.BookingPayments.Where(p => p.BookingId == booking.BookingId &&
                                                                p.IsPaymentByGuest && !p.IsScheduledPayment))
            paid += payment.Amount;


          if (paid >= booking.Rent + booking.Deposit + booking.Additions - booking.Usage)
            SetBookingStatus(booking, BookingStatus.Paid);
          else
          {
            if (paid < booking.Rent + booking.Deposit + booking.Additions - booking.Usage)
            {
              if (booking.IsConfirmed)
                SetBookingStatus(booking, BookingStatus.Confirmed);
              else
                SetBookingStatus(booking, BookingStatus.Reserved);
            }
          }

          scheduled = 0;
          foreach (var payment in em.BookingPayments.Where(p => p.BookingId == booking.BookingId &&
                                                                p.IsPaymentByGuest && p.IsScheduledPayment
            ).OrderBy(p => p.Due))
          {
            scheduled += payment.Amount;
            payment.IsPaid = (scheduled <= paid);
          }

        }

        if (booking.Status == BookingStatus.Paid)
        {
          //controleer of eigenaar alles betaald heeft
            decimal ownerScheduled = 0;
            decimal ownerPaid = 0;
          foreach (var payment in em.BookingPayments.Where(p => p.BookingId == booking.BookingId &&
                                                               !p.IsPaymentByGuest))
          {
            if (payment.IsScheduledPayment)
              ownerScheduled = ownerScheduled + payment.Amount;
            else
              ownerPaid = ownerPaid + payment.Amount;
          }
          if (ownerPaid >= ownerScheduled)
            SetBookingStatus(booking, BookingStatus.Closed);
        }

        var reminders = em.BookingReminders.Where(r => r.BookingId == booking.BookingId);
        var payments = em.BookingPayments.Where(r => r.BookingId == booking.BookingId);

        foreach (var reminder in reminders)
        {

          switch (reminder.Milestone)
          {
            case MileStone.Arrival:
            {
              reminder.IsDue = true;
              reminder.Due = booking.Arrival.AddDays(reminder.Offset);

              break;
            }
            case MileStone.Departure:
            {
              reminder.IsDue = true;
              reminder.Due = booking.Departure.AddDays(reminder.Offset);

              break;
            }
            case MileStone.FirstPayment:
            {
              var payment =
                payments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest).OrderBy(p => p.Due).FirstOrDefault();
              if (payment != null)
              {
                reminder.IsDue = true;
                reminder.Due = payment.Due.AddDays(reminder.Offset);
              }
              else
              {
                reminder.IsDue = false;
                reminder.Due = null;
              }

              break;
            }
            case MileStone.LastPayment:
            {
              {
                if (booking.Status == BookingStatus.Paid)
                {
                  BookingPayment payment = null;
                  // de boeking is volledig betaald. Zoek de laatste payment om Due voor de reminder te bepalen. 
                  foreach (
                    var last in payments.Where(p => !p.IsScheduledPayment && p.IsPaymentByGuest).OrderBy(p => p.Due))
                    payment = last;

                  if (payment != null)
                  {
                    reminder.IsDue = true;
                    reminder.Due = payment.Due.AddDays(reminder.Offset);
                  }
                  else
                  {
                    // hier komt het programma nooit, betaald is immers groter gelijk boekingsbedrag.
                    // voor veiligheid toch maar laten staan
                    reminder.IsDue = false;
                    reminder.Due = null;
                  }
                }
                else
                {
                  reminder.IsDue = false;
                  reminder.Due = null;
                }
              }
              break;
            }
          }

        }
        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Set the status for the booking
    /// </summary>
    /// <param name="booking"></param>
    /// <param name="status"></param>
    private static void SetBookingStatus(Booking booking, string status)
    {
      if (booking.Status == status)
        return;

      booking.Status = status;
      booking.StatusUpdate = DateTime.Now;
    }

    /// <summary>
    /// Apply the selected paypattern for the booking
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string ApplyPayPattern(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      var em = (AccoBookingEntities) entitymanager;
      var bookingid = (int) args[0];
      var patternid = (int) args[1];

      var msg = ApplyPayPatternToBooking(em, patternid, bookingid);
      if (!string.IsNullOrEmpty(msg))
        return msg;

      return "";

    }

    /// <summary>
    /// Apply a paypattern to the booking
    /// </summary>
    /// <param name="em"></param>
    /// <param name="patternid"></param>
    /// <param name="bookingid"></param>
    /// <returns></returns>
    private static string ApplyPayPatternToBooking(AccoBookingEntities em, int patternid, int bookingid)
    {
      try
      {
        var booking = em.Bookings.FirstOrDefault(b => b.BookingId == bookingid);
        var pattern = em.AccoPayPatterns.FirstOrDefault(p => p.AccoPayPatternId == patternid);
        var sequence = em.Sequences.FirstOrDefault(s => s.Name == SequenceName.BookingPaymentId);

        // apply pay pattern for booking

        // delete current scheduled payments
        var scheduledPayments = (from bookingPayment in booking.BookingPayments
          where bookingPayment.IsScheduledPayment
          select bookingPayment).ToArray(); //prevent collection changed exception

        foreach (var scheduledPayment in scheduledPayments)
          scheduledPayment.EntityAspect.Delete();

        var total = booking.Rent;

        // spread additions over calculated payments (via total) or additions in last payment?
        if (!pattern.IsAdditionInLastPayment)
          total += booking.Additions;

        // spread deposit over calculated payments (via total) or deposit in last payment?
        if (!pattern.IsDepositInLastPayment)
          total += booking.Deposit -booking.Usage;

        // create scheduled payments for guest for fixed amounts
        var fixedGuestPayments = pattern.AccoPayPatternPayments.Where(p => p.PaymentAmount > 0).ToList() ;

        for (int i = 0; i < fixedGuestPayments.Count; i++) 
        {
          var paymentFg = BookingPayment.Create(++sequence.CurrentId);
          paymentFg.IsPaymentByGuest = true;
          paymentFg.IsScheduledPayment = true;
          paymentFg.Amount = fixedGuestPayments[i].PaymentAmount;
          paymentFg.IsPaid = false;
          
          if (fixedGuestPayments[i].DaysToPayAfterBooking > 0)
            paymentFg.Due = booking.Booked.AddDays(fixedGuestPayments[i].DaysToPayAfterBooking);
          else paymentFg.Due = booking.Arrival.AddDays(-1*fixedGuestPayments[i].DaysToPayBeforeArrival);

          em.AddEntity(paymentFg);
          booking.BookingPayments.Add(paymentFg);

          total -= paymentFg.Amount;
        }

        booking.IsAmountExactlyScheduled = true;

        // create calculated amounts
        // total contains the amount yet to be paid
        // the sum of the percentages is per definition 100
        // there is always at least one calculated guest payment

        var calculatedGuestPayments = pattern.AccoPayPatternPayments.Where(p => p.PaymentPercentage > 0).ToList();

        for (int i = 0; i < calculatedGuestPayments.Count; i++)
        {
          var paymentCg = BookingPayment.Create(++sequence.CurrentId);
          paymentCg.IsPaymentByGuest = true;
          paymentCg.IsScheduledPayment = true;
          paymentCg.Amount = total*calculatedGuestPayments[i].PaymentPercentage/100;
          paymentCg.IsPaid = false;

          // add addition and or deposit to last payment
          if (i == calculatedGuestPayments.Count - 1)
          {
            if (pattern.IsAdditionInLastPayment)
              paymentCg.Amount += booking.Additions;

            if (pattern.IsDepositInLastPayment)
              paymentCg.Amount += booking.Deposit - booking.Usage;
          }

          if (calculatedGuestPayments[i].DaysToPayAfterBooking > 0)
            paymentCg.Due = booking.Booked.AddDays(calculatedGuestPayments[i].DaysToPayAfterBooking);
          else paymentCg.Due = booking.Arrival.AddDays(-1*calculatedGuestPayments[i].DaysToPayBeforeArrival);

          em.AddEntity(paymentCg);
          booking.BookingPayments.Add(paymentCg);

        }

        // create scheduled payment for owner (only one fixed amount
        var payment = BookingPayment.Create(++sequence.CurrentId);
        payment.IsPaymentByGuest = false;
        payment.IsScheduledPayment = true;
        payment.Amount = booking.Deposit - booking.Usage;
        payment.IsPaid = false;
        payment.Due = booking.Departure.AddDays(booking.DaysToPayDepositBackAfterDeparture);

        em.AddEntity(payment);
        booking.BookingPayments.Add(payment);

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Move abooking to another acco
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string MoveBookingToAcco(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      var em = (AccoBookingEntities) entitymanager;
      var bookingid = (int) args[0];
      var accoid = (int) args[1];

      try
      {
        var booking = em.Bookings.FirstOrDefault(b => b.BookingId == (int) args[0]);
        var acco = em.Accoes.FirstOrDefault(a => a.AccoId == booking.AccoId);
        var toAcco = em.Accoes.FirstOrDefault(a => a.AccoId == accoid);

        booking.AccoId = accoid;
        //Wat te doen met additions, vertalingen

        em.SaveChanges(); //Fingers crossed!

        return "";

      }
      catch (Exception ex)
      {
        return ex.Message;
      }

    }

    /// <summary>
    /// Check the reminders for all or a selected owner
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="entitymanager"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    [AllowRpc]
    public static string CheckReminders(IPrincipal principal, EntityManager entitymanager, params object[] args)
    {
      var items = new List<BookingToDoListItem>();
      var em = (AccoBookingEntities) entitymanager;
      var owner = 0;
      if (args.Length > 0)
        owner = (int) args[0];

      if (owner != 0)
      {
        var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == owner);
        if (accoOwner.Language.Description == "Dutch")
          AccoResource.Culture = new CultureInfo("nl");
      }

      try
      {
        foreach (var notification in em.AccoNotifications.Where(n=> (n.AccoOwnerId == owner && owner != 0 ) || owner == 0))
        {
          notification.EntityAspect.Delete();
        }
        em.SaveChanges();

        CheckRemindersBookingPayments(em, owner);
        CheckRemindersBookings(em, owner);
        CheckRemindersBookingExpired(em, owner);
        CheckRemindersPayPatterns(em, owner);
        CheckRemindersLicenseExpired(em, owner);

        em.SaveChanges(); //Fingers crossed!

        if (owner != 0)
          return "";

        foreach (var group in em.AccoNotifications.GroupBy(s => s.AccoOwnerId))
        {

          if (group.Any(n => n.Acco.SendWeeklyReminders))
          {
            var not = group.FirstOrDefault();

            if (not.AccoOwner.Language.Description == "Dutch")
              AccoResource.Culture = new CultureInfo("nl");

            string fileName = @"c:\temp\owner" + group.Key + ".csv";
            var sw = new StreamWriter(fileName);
            
            sw.Write(AccoResource.email_NOTIFICATION_FROM);
            sw.Write(";");
            sw.Write("info@accobooking.nl");
            sw.WriteLine();

            sw.Write(AccoResource.email_NOTIFICATION_TO);
            sw.Write(";");
            sw.Write(not.AccoOwner.Email);
            sw.WriteLine();

            sw.Write(AccoResource.email_NOTIFICATION_OWNER_NAME);
            sw.Write(";");
            sw.Write(not.AccoOwner.Name);
            sw.WriteLine();

            sw.Write(AccoResource.email_NOTIFICATION_CREATED_ON);
            sw.Write(";");
            sw.Write(DateTime.Now.ToShortDateString());
            sw.WriteLine();

            sw.WriteLine();

            sw.Write(AccoResource.lab_ACCO);
            sw.Write(";");
            sw.Write(AccoResource.lab_DESCRIPTION);
            sw.Write(";");
            sw.Write(AccoResource.lab_BOOKER);
            sw.Write(";");
            sw.Write(AccoResource.lab_DUE);
            sw.WriteLine();

            foreach (var notification in group.Where(n => n.Acco.SendWeeklyReminders).OrderBy(n => n.Acco.Description))
            {
              sw.Write(notification.Acco.Description);
              sw.Write(";");
              sw.Write(notification.Description);
              sw.Write(";");
              sw.Write(notification.Booking.Booker);
              sw.Write(";");
              sw.Write(notification.ExpirationDate);
              sw.WriteLine();
            }

            sw.Close();
            SendMail("info@accobooking.nl",
              "jkattestaart@gmail.com",                                     //TODO: not.AccoOwner.Email,
              AccoResource.email_NOTIFICATION_SUBJECT,
              AccoResource.email_NOTIFICATION_BODY,
              fileName);
          }
        }
        return "";

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return ex.Message;
      }
    }

    /// <summary>
    /// Send the mail with the reminders to an owner
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="attach"></param>
    /// <returns></returns>
    private static bool SendMail(string from, string to, string subject, string body, string attach)
    {
      bool isEmailSendSuccessfully = false;

      try
      {
        var mailMessage = new MailMessage(from, to);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.Attachments.Add(new Attachment(attach));

        var smtp = new SmtpClient
        {
          Host = "localhost",
          Port = 25,
          //Host = "smtp.live.com",
          //Host = "smtp.gmail.com",
          //Port = 587,
          //EnableSsl = true,
          //DeliveryMethod = SmtpDeliveryMethod.Network,
          //UseDefaultCredentials = false,
          //Credentials = new NetworkCredential("jkattestaart", "???????????????????")
          //Credentials = new NetworkCredential("jkattestaart@stiphout-it.nl", "??????????????")
        };

        smtp.Send(mailMessage);
        isEmailSendSuccessfully = true;

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      return isEmailSendSuccessfully;
    }

    /// <summary>
    /// Create a notification for the reminder
    /// </summary>
    /// <param name="em"></param>
    /// <returns></returns>
    private static AccoNotification CreateNotification(AccoBookingEntities em)
    {
      var sequence = GeneralLibrary.NextValue(em, SequenceName.AccoNotificationId);

      var notification = new AccoNotification();
      em.AddEntity(notification);
      notification.AccoNotificationId = sequence;
      return notification;
    }

    /// <summary>
    /// Check the reminders for the booking payments
    /// </summary>
    /// <param name="em"></param>
    /// <param name="owner"></param>
    private static void CheckRemindersBookingPayments(AccoBookingEntities em, int owner)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(14);

      Expression<Func<BookingPayment, bool>> filter = null;
      filter = x => ((x.Booking.Acco.AccoOwnerId == owner && owner != 0) || owner == 0)
                    && x.Booking.Status != BookingStatus.Closed
                    && x.Booking.Status != BookingStatus.Expired 
                    && x.Booking.Status != BookingStatus.Cancelled 
                    && x.IsScheduledPayment  
                    && !x.IsPaid             
                    && x.Due.CompareTo(offset) <= 0;

      foreach (var payment in em.BookingPayments.Where(filter))
      {
        if (owner == 0)
        {
          var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == payment.Booking.Acco.AccoOwnerId);
          if (accoOwner.Language.Description == "Dutch")
            AccoResource.Culture = new CultureInfo("nl");
        }

        var notification = CreateNotification(em);

        notification.AccoId = payment.Booking.AccoId;
        notification.AccoOwnerId = payment.Booking.Acco.AccoOwnerId;
        notification.BookingId = payment.BookingId;
        notification.Description = (payment.Due.CompareTo(DateTime.Now) < 0)
          ? (payment.IsPaymentByGuest
            ? AccoResource.lab_GUEST_PAYMENT_TOO_LATE
            : AccoResource.lab_OWNER_PAYMENT_TOO_LATE
            )
          : (payment.IsPaymentByGuest
            ? AccoResource.lab_GUEST_PAYMENT_SCHEDULED
            : AccoResource.lab_OWNER_PAYMENT_SCHEDULED
            );
        notification.NoticationType = ReminderContext.Payment;
        notification.NotificationDate = DateTime.Now;
        notification.ExpirationDate = payment.Due;
        notification.DaystoExpire = 0;
      }

      em.SaveChanges();
    }

    /// <summary>
    /// Check the reminders for the bookings
    /// </summary>
    /// <param name="em"></param>
    /// <param name="owner"></param>
    private static void CheckRemindersBookings(AccoBookingEntities em, int owner)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(14);

      Expression<Func<BookingReminder, bool>> filter = null;
      filter = x => ((x.Booking.Acco.AccoOwnerId == owner && owner != 0) || owner == 0)
                    &&  x.Booking.Status != BookingStatus.Expired 
                    && x.Booking.Status != BookingStatus.Cancelled 
                    && x.IsDue && x.IsDone == false 
                    && x.Due.Value.CompareTo(offset) <= 0;

      foreach (var reminder in em.BookingReminders.Where(filter))
      {
        var notification = CreateNotification(em);

        if (owner == 0)
        {
          var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == reminder.Booking.Acco.AccoOwnerId);
          if (accoOwner.Language.Description == "Dutch")
            AccoResource.Culture = new CultureInfo("nl");
        }

        notification.AccoId = reminder.Booking.AccoId;
        notification.AccoOwnerId = reminder.Booking.Acco.AccoOwnerId;
        notification.BookingId = reminder.BookingId;
        notification.Description = reminder.Description
                                   + ((reminder.Due.Value.CompareTo(DateTime.Now) < 0)
                                     ? AccoResource.lab_TOO_LATE
                                     : AccoResource.lab_SCHEDULED
                                     );
        notification.NoticationType = ReminderContext.Reminder;
        notification.NotificationDate = DateTime.Now;
        notification.ExpirationDate = reminder.Due.Value;
        notification.DaystoExpire = 0;
      }
      em.SaveChanges();
    }

    /// <summary>
    /// The reminders for expired bookings
    /// </summary>
    /// <param name="em"></param>
    /// <param name="owner"></param>
    private static void CheckRemindersBookingExpired(AccoBookingEntities em, int owner)
    {
      Expression<Func<Booking, bool>> filter = null;
      filter = x => ((x.Acco.AccoOwnerId == owner && owner != 0) || owner == 0)
                    && !x.IsBlock && x.Status != BookingStatus.Expired
                    && x.Status != BookingStatus.Cancelled;

      // @@@ JKT named query wordt niet meer automatisch herkend
      foreach (var booking in em.ExpiredBookings.Where(filter))
      {
        var notification = CreateNotification(em);

        if (owner == 0)
        {
          var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == booking.Acco.AccoOwnerId);
          if (accoOwner.Language.Description == "Dutch")
            AccoResource.Culture = new CultureInfo("nl");
        }

        notification.AccoId = booking.AccoId;
        notification.AccoOwnerId = booking.Acco.AccoOwnerId;
        notification.BookingId = booking.BookingId;
        notification.Description = AccoResource.lab_BOOKING_EXPIRED;
        notification.NoticationType = ReminderContext.Booking;
        notification.NotificationDate = DateTime.Now;
        notification.ExpirationDate = booking.Booked;
        notification.DaystoExpire = booking.Acco.DaysToExpire;
      }
      em.SaveChanges();
    }

    /// <summary>
    /// Check reminders for paypatterns with not scheduled payments
    /// </summary>
    /// <param name="em"></param>
    /// <param name="owner"></param>
    private static void CheckRemindersPayPatterns(AccoBookingEntities em, int owner)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(14);

      Expression<Func<Booking, bool>> filter = null;
      filter = x => ((x.Acco.AccoOwnerId == owner && owner != 0) || owner == 0) 
                    && !x.IsBlock 
                    && !x.IsAmountExactlyScheduled 
                    && x.Status != BookingStatus.Expired 
                    && x.Status != BookingStatus.Cancelled;
      
      foreach (var booking in em.Bookings.Where(filter))
      {
        var notification = CreateNotification(em);

        if (owner == 0)
        {
          var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == booking.Acco.AccoOwnerId);
          if (accoOwner.Language.Description == "Dutch")
            AccoResource.Culture = new CultureInfo("nl");
        }
        
        notification.AccoId = booking.AccoId;
        notification.AccoOwnerId = booking.Acco.AccoOwnerId;
        notification.BookingId = booking.BookingId;
        notification.Description = AccoResource.lab_NOT_EXACTLY_SCHEDULED;
                                   
        notification.NoticationType = ReminderContext.Booking;
        notification.NotificationDate = DateTime.Now;
        notification.ExpirationDate = DateTime.Now;
        notification.DaystoExpire = 0;
      }
      em.SaveChanges();
    }

    /// <summary>
    /// Check reminders for expired licenses
    /// </summary>
    /// <param name="em"></param>
    /// <param name="owner"></param>
    private static void CheckRemindersLicenseExpired(AccoBookingEntities em, int owner)
    {
       var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddMonths(-1);

      Expression<Func<Acco, bool>> filter = null;
      filter = x => ((x.AccoOwnerId == owner && owner != 0) || owner == 0) &&
                      x.LicenceExpiration.Value.CompareTo(offset) <= 0; 

      foreach (var acco in em.Accoes.Where(filter))
      {
        var notification = CreateNotification(em);

        if (owner == 0)
        {
          var accoOwner = em.AccoOwners.FirstOrDefault(o => o.AccoOwnerId == acco.AccoOwnerId);
          if (accoOwner.Language.Description == "Dutch")
            AccoResource.Culture = new CultureInfo("nl");
        }
        
        notification.AccoId = acco.AccoId;
        notification.AccoOwnerId = acco.AccoOwnerId;
        notification.BookingId = null;
        notification.Description = AccoResource.lab_LICENSE_EXPIRES;
                                   
        notification.NoticationType = ReminderContext.License;
        notification.NotificationDate = DateTime.Now;
        notification.ExpirationDate = acco.LicenceExpiration.Value;
        notification.DaystoExpire = 0;
      }
      em.SaveChanges();
    }


  }
}
