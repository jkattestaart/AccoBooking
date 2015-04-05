// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{

  public class BookingSearchService : IBookingSearchService
  {
    private readonly IRepository<Booking> _repository;

    public BookingSearchService(IRepository<Booking> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingListItem>> FindBookingsAsync(CancellationToken cancellationToken)
    {
      Expression<Func<Booking, bool>> filter = null;
    
      filter = booking => booking.Acco.AccoOwnerId == SessionManager.CurrentOwner.AccoOwnerId && !booking.IsBlock;

      var bookings = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingListItem
        {
          Id = x.BookingId,
          Accommodation = x.Acco.Description,
          Arrival = x.Arrival,
          Departure = x.Departure,
          //Nights zit in de projections als calculated value!
          Rent = x.Rent,
          Deposit = x.Deposit,
          Additions = x.Additions,
          GuestName = x.Booker,
          Status = x.Status,
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Arrival));

      foreach (var booking in bookings)
      {
        booking.Label = booking.Arrival.ToShortDateString() + "-" + booking.Departure.ToShortDateString();
        booking.Status = SystemCodeService.Description(booking.Status, SystemGroupName.BookingStatus);
      }

      return bookings;
    }

    public async Task<IEnumerable<BookingListItem>> FindBookingsAsync(
      string guestName, bool includeClosed, bool includeExpired, DateTime from, DateTime to,
      CancellationToken cancellationToken)
    {
      Expression<Func<Booking, bool>> filter = null;
      filter =
        booking =>
          (string.IsNullOrWhiteSpace(guestName) || booking.Booker.Contains(guestName))
          && !booking.IsBlock
          && booking.Acco.AccoOwnerId == SessionManager.CurrentOwner.AccoOwnerId
          && (includeClosed || booking.Status != BookingStatus.Closed)
          && (includeExpired || booking.Status != BookingStatus.Expired)
          && (from.Date != new DateTime(1, 1, 1) || booking.Arrival >= from.Date)
          && (to.Date != new DateTime(1, 1, 1) || booking.Arrival <= to.Date);

      var bookings = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingListItem
        {
          Id = x.BookingId,
          Accommodation = x.Acco.Description,
          Arrival = x.Arrival,
          Departure = x.Departure,
          //Nights zit in de projections als calculated value!
          Rent = x.Rent,
          Deposit = x.Deposit,
          Additions = x.Additions,
          GuestName = x.Booker,
          Status = x.Status,
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Arrival));

      foreach (var booking in bookings)
      {
        booking.Label = booking.Arrival.ToShortDateString() + "-" + booking.Departure.ToShortDateString();
        booking.Status = SystemCodeService.Description(booking.Status, SystemGroupName.BookingStatus);
      }

      return bookings;
    }

    public async Task<IEnumerable<BlockListItem>> FindBlocksAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Booking, bool>> filter = null;
      filter = booking => booking.IsBlock && booking.AccoId == accoid;
      
      var bookings = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BlockListItem
        {
          Id = x.BookingId,
          Accommodation = x.Acco.Description,
          Arrival = x.Arrival,
          Departure = x.Departure,
          Notes = x.Notes  
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Arrival));

      foreach (var booking in bookings)
      {
        booking.Label = booking.Arrival.ToShortDateString() + "-" + booking.Departure.ToShortDateString();
      }

      return bookings;
    }
  }


  public class BookingReminderSearchService : IBookingReminderSearchService
  {
    private readonly IRepository<BookingReminder> _repository;

    public BookingReminderSearchService(IRepository<BookingReminder> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingReminderListItem>> FindBookingRemindersAsync(
      int bookingid, CancellationToken cancellationToken)
    {
      Expression<Func<BookingReminder, bool>> filter = null;
      filter = x => x.BookingId == bookingid;

      var reminders = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingReminderListItem
        {
          Id = x.BookingReminderId,
          Description = x.Description,
          Milestone = x.Milestone,
          Offset = x.Offset,
          DisplaySequence = x.DisplaySequence,
          IsDue = x.IsDue,
          Due = x.Due,
          IsDone = x.IsDone,
          Done = x.Done.Value,

          Label = x.Description
        }),

        cancellationToken, filter, null);


      foreach (var reminder in reminders)
      {
        if (!string.IsNullOrEmpty(reminder.Milestone))
          reminder.Milestone = SystemCodeService.Description(reminder.Milestone, SystemGroupName.Milestone);
        reminder.DisplaySequence = SystemCodeService.DisplaySequence(reminder.Milestone, SystemGroupName.Milestone);
      }

      return reminders.OrderBy(i=>i.DisplaySequence).ThenBy(i=>i.Offset);
    }
  }

  public class BookingAdditionSearchService : IBookingAdditionSearchService
  {
    private readonly IRepository<BookingAddition> _repository;

    public BookingAdditionSearchService(IRepository<BookingAddition> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingAdditionListItem>> FindBookingAdditionsAsync(
      int bookingid, CancellationToken cancellationToken)
    {
      Expression<Func<BookingAddition, bool>> filter = null;
      filter = x => x.BookingId == bookingid;

      var additions = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingAdditionListItem
        {
          Id = x.BookingAdditionId,
          Description = x.Description,
          Price = x.Price,
          Arrival = x.Booking.Arrival,
          Departure = x.Booking.Departure,
          Adults = x.Booking.Adults,
          Children = x.Booking.Children,
          Pets = x.Booking.Pets,
          // Amount = x.Amount,  @@@ JKT zelfde code als in domain voor projection
          KWHUsage= x.Booking.DepartureElectricity - x.Booking.ArrivalElectricity,
          GASM3Usage = x.Booking.DepartureGas - x.Booking.ArrivalGas,
          WATERM3Usage = x.Booking.DepartureWater - x.Booking.ArrivalWater,
          Unit = x.Unit,
          SystemCodeUnit = x.Unit,
          IsPaidFromDeposit =  x.IsPaidFromDeposit,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));

      foreach (var addition in additions)
        addition.Unit = SystemCodeService.Description(addition.Unit, SystemGroupName.Unit);

      return additions;
    }
  }


  public class BookingGuestSearchService : IBookingGuestSearchService
  {
    private readonly IRepository<BookingGuest> _repository;

    public BookingGuestSearchService(IRepository<BookingGuest> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingGuestListItem>> FindBookingGuestsAsync(
      int bookingid, CancellationToken cancellationToken)
    {
      Expression<Func<BookingGuest, bool>> filter = null;
      filter = x => x.BookingId == bookingid;

      var guests = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingGuestListItem
          {
            Id = x.BookingGuestId,
            Name = x.Name,
            Email = x.Email,
            Phone = x.Phone,
            Address1 = x.Address1,
            Address2 = x.Address2,
            Address3 = x.Address3,
            Gender = x.Gender,
            BirthDate = x.BirthDate,
            IdentityDocument = x.IdentityDocument,


            Label = x.Name
          }),

        cancellationToken, filter, q => q.OrderBy(i => i.Name));

      foreach (var guest in guests)
        guest.Gender = SystemCodeService.Description(guest.Gender, SystemGroupName.Gender);

      return guests;

    }
  }

  public class BookingPaymentSearchService : IBookingPaymentSearchService
  {
    private readonly IRepository<BookingPayment> _repository;

    public BookingPaymentSearchService(IRepository<BookingPayment> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingPaymentListItem>> FindBookingPaymentsAsync(
      int bookingid, CancellationToken cancellationToken)
    {
      Expression<Func<BookingPayment, bool>> filter = null;
      filter = x => x.BookingId == bookingid;

      var payments = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingPaymentListItem
          {
            Id = x.BookingPaymentId,
            //Description = x.Description,
            //Milestone = x.Milestone,
            //Offset = x.Offset.Value,
            //DisplaySequence = x.DisplaySequence.Value,
            //IsDue = x.IsDue.Value,
            //Due = x.Due.Value,
            //IsDone = x.IsDone.Value,
            //Done = x.Done.Value

          }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));

      foreach (var payment in payments)
        payment.Label = payment.Id.ToString();

      return payments;
    }
  }


  public class BookingCancelConditionSearchService : IBookingCancelConditionSearchService
  {
    private readonly IRepository<BookingCancelCondition> _repository;

    public BookingCancelConditionSearchService(IRepository<BookingCancelCondition> repository)
    {
      _repository = repository;
    }

    public async Task<IEnumerable<BookingCancelConditionListItem>> FindBookingCancelConditionsAsync(
      int bookingid, CancellationToken cancellationToken)
    {
      Expression<Func<BookingCancelCondition, bool>> filter = null;
      filter = x => x.BookingId == bookingid;

      var conditions = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingCancelConditionListItem
          {
            Id = x.BookingCancelConditionId,
            //Description = x.Description,
            //Milestone = x.Milestone,
            //Offset = x.Offset.Value,
            //DisplaySequence = x.DisplaySequence.Value,
            //IsDue = x.IsDue.Value,
            //Due = x.Due.Value,
            //IsDone = x.IsDone.Value,
            //Done = x.Done.Value

          }),

        cancellationToken, filter, q => q.OrderBy(i => i.DaysBeforeArrival));

      foreach (var condition in conditions)
        condition.Label = condition.Id.ToString();

      return conditions;
    }

  }

  public class BookingPaymentToDoSearchService : IBookingPaymentToDoSearchService
  {
    private readonly IRepository<BookingPayment> _repository;

    public BookingPaymentToDoSearchService(IRepository<BookingPayment> repository)
    {
      _repository = repository;
    }


    public Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(14);

      Expression<Func<BookingPayment, bool>> filter = null;
      filter = x => x.Booking.Acco.AccoOwnerId == SessionManager.CurrentAcco.AccoOwnerId &&
                    x.Booking.Status != BookingStatus.Paid && x.Booking.Status != BookingStatus.Closed &&
                    x.Booking.Status != BookingStatus.Expired && x.Booking.Status != BookingStatus.Cancelled &&
                    x.IsScheduledPayment && // @@@ JKT vervalllen  x.IsClosed == false &&
                    !x.IsPaid              &&      //TODO omzetten naar not nullable
                    x.Due.CompareTo(offset) < 0;

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingToDoListItem
          {
            Id = x.Booking.BookingId,
            Accommodation= x.Booking.Acco.Description,
            Description = (x.Due.CompareTo(DateTime.Now) < 0)
                            ? (x.IsPaymentByGuest 
                                 ? AccoResource.lab_GUEST_PAYMENT_TOO_LATE : AccoResource.lab_OWNER_PAYMENT_TOO_LATE
                              )
                            : (x.IsPaymentByGuest 
                                 ? AccoResource.lab_GUEST_PAYMENT_SCHEDULED : AccoResource.lab_OWNER_PAYMENT_SCHEDULED
                              ),
            Context = ReminderContext.Payment,
            Guest = x.Booking.Booker,
            ExpirationDate = x.Due,
            DaysToExpire = 0
          }),

        cancellationToken, filter, null);
    }

  }


  public class BookingReminderToDoSearchService : IBookingReminderToDoSearchService
  {
    private readonly IRepository<BookingReminder> _repository;

    public BookingReminderToDoSearchService(IRepository<BookingReminder> repository
      )
    {
      _repository = repository;
    }


    public Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(14);

      Expression<Func<BookingReminder, bool>> filter = null;
      filter = x => x.Booking.Acco.AccoOwnerId == SessionManager.CurrentAcco.AccoOwnerId &&
                    x.Booking.Status != BookingStatus.Expired && x.Booking.Status != BookingStatus.Cancelled &&
                    x.IsDue && x.IsDone == false &&
                    x.Due.Value.CompareTo(offset) < 0;

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingToDoListItem
          {
            Id = x.Booking.BookingId,
            Accommodation = x.Booking.Acco.Description,
            Description = x.Description
                          + ((x.Due.Value.CompareTo(DateTime.Now) < 0) ? 
                              AccoResource.lab_TOO_LATE : AccoResource.lab_SCHEDULED
                            ),
            Context = ReminderContext.Reminder,
            Guest = x.Booking.Booker,
            ExpirationDate = x.Due.Value,
            DaysToExpire = 0
          }),

        cancellationToken, filter, null);
    }
  }


  public class BookingExpiredToDoSearchService : IBookingExpiredToDoSearchService
  {
    private readonly IRepository<Booking> _repository;

    public BookingExpiredToDoSearchService(IRepository<Booking> repository)
    {
      _repository = repository;
    }

    public Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Booking, bool>> filter = null;
      filter = x => x.Acco.AccoOwnerId == SessionManager.CurrentAcco.AccoOwnerId &&
                    x.Status != BookingStatus.Expired && x.Status != BookingStatus.Cancelled;

      // @@@ JKT named query wordt niet meer automatisch herkend
      var em = new AccoBookingEntities();
      var q = em.ExpiredBookings.Where(filter);

      return em.ExecuteQueryAsync(q.Select(x => new BookingToDoListItem
      {
        Id = x.BookingId,
        Accommodation = x.Acco.Description,
        Context = ReminderContext.Booking,
        Description = AccoResource.lab_BOOKING_EXPIRED,
        ExpirationDate = x.Booked,
        DaysToExpire = x.Acco.DaysToExpire,
        Guest = x.Booker
      }));


    }
  }

  public class PayPatternToDoSearchService : IPayPatternToDoSearchService
  {
    private readonly IRepository<Booking> _repository;
    private List<BookingToDoListItem> _items = new List<BookingToDoListItem>();

    public PayPatternToDoSearchService(IRepository<Booking> repository)
    {
      _repository = repository;
    }

    public Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Booking, bool>> filter = null;
      filter = x => x.Acco.AccoOwnerId == SessionManager.CurrentAcco.AccoOwnerId &&
               !x.IsBlock &&
               x.Status != BookingStatus.Expired && x.Status != BookingStatus.Cancelled &&
               !x.IsAmountExactlyScheduled;

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingToDoListItem
        {
          Id = x.BookingId,
          Accommodation = x.Acco.Description,
          Context = ReminderContext.Booking,
          Description = AccoResource.lab_NOT_EXACTLY_SCHEDULED,
          ExpirationDate = DateTime.Now,
          DaysToExpire = 0,
          Guest = x.Booker
        }),

        cancellationToken, filter, null);
    }
  }

  public class LicenseExpiredToDoSearchService : ILicenseExpiredToDoSearchService
  {
    private readonly IRepository<Acco> _repository;

    public LicenseExpiredToDoSearchService(IRepository<Acco> repository)
    {
      _repository = repository;
    }

    public Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      var offset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddMonths(-1);

      Expression<Func<Acco, bool>> filter = null;
      filter = x => x.AccoOwnerId == SessionManager.CurrentAcco.AccoOwnerId &&
                    x.LicenceExpiration.Value.CompareTo(offset) < 0; 

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new BookingToDoListItem
        {
          Id = x.AccoId,
          Accommodation = x.Description,
          Context = ReminderContext.License,
          Description = AccoResource.lab_LICENSE_EXPIRES,
          ExpirationDate = x.LicenceExpiration.Value,
          DaysToExpire = 0
        }),

        cancellationToken, filter, null);
    }
  }

}
