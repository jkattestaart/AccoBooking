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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cocktail;
using DomainModel;
using DomainServices.Services;
using IdeaBlade.EntityModel;

namespace DomainServices.Factories
{

  public class BookingFactory : Factory<Booking>
  {
    private readonly IRepository<Language> _languages;
    private readonly IRepository<Country> _countries;

    public BookingFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _languages = new Repository<Language>(entityManagerProvider);
      _countries = new Repository<Country>(entityManagerProvider);
    }

    public override async Task<Booking> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var acco = SessionManager.CurrentAcco;
     
      var language = await _languages.WithIdFromDataSourceAsync(SessionManager.CurrentOwner.LanguageId, cancellationToken);
      var country = await _countries.WithIdFromDataSourceAsync(SessionManager.CurrentOwner.CountryId, cancellationToken);

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingId, cancellationToken);

      var booking = new Booking();
      
      EntityManager.AddEntity(booking);
      booking.BookingId = sequence.CurrentId;
      booking.Status = "RESERVED";
      booking.Booked = DateTime.Now.Date;
      booking.IsBookedOnline = false;
      booking.BookingColor = acco.ColorOwner ?? Colors.Red.ToString();
      booking.Adults = 2;

      sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingGuestId, cancellationToken);
      var bookingGuest = new BookingGuest();
      EntityManager.AddEntity(bookingGuest);
      bookingGuest.BookingGuestId = sequence.CurrentId;
      bookingGuest.BookingId = booking.BookingId;
      bookingGuest.Name = "TODO";

      booking.BookerLanguageId = language.LanguageId;
      booking.BookerCountryId = country.CountryId;

      booking.Deposit = acco.Deposit;
      booking.Additions = 0;
      booking.IsAmountExactlyScheduled = true;
      booking.IsConfirmed = false;

      
      return booking;
    }
  }

  public class BookingGuestFactory : Factory<BookingGuest>, IAccoBookingFactory<BookingGuest>
  {
    public BookingGuestFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<BookingGuest> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingGuestId, cancellationToken);

      var bookingGuest = new BookingGuest();
      EntityManager.AddEntity(bookingGuest);
      bookingGuest.BookingGuestId = sequence.CurrentId;

      return bookingGuest;
    }

    public async Task<BookingGuest> CreateCopyAsync(CancellationToken cancellationToken, int entityid)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var key = new EntityKey(typeof(BookingGuest), entityid);
      var guest = EntityManager.FindEntity(key) as BookingGuest;

      var bookingGuest = await CreateAsync(cancellationToken);
      AccoBookingFactory.Clone(guest, bookingGuest);
      
      return bookingGuest;
    }
  }

  public class BookingReminderFactory : Factory<BookingReminder>
  {
    public BookingReminderFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<BookingReminder> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingReminderId, cancellationToken);

      var bookingReminder = new BookingReminder();
      EntityManager.AddEntity(bookingReminder);

      bookingReminder.BookingReminderId = sequence.CurrentId;
      bookingReminder.Milestone = "";
      bookingReminder.Offset = 0;
      bookingReminder.DisplaySequence = 10; //TODO Jan: zou laatste +10 moeten zijn
      bookingReminder.IsDue = false;
      bookingReminder.Due = null;
      bookingReminder.IsDone = false;
      bookingReminder.Done = null;

      return bookingReminder;
    }
  }

  public class BookingAdditionFactory : Factory<BookingAddition>
  {
    public BookingAdditionFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<BookingAddition> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingAdditionId, cancellationToken);

      var bookingAddition = new BookingAddition();
      EntityManager.AddEntity(bookingAddition);

      bookingAddition.BookingAdditionId = sequence.CurrentId;
      bookingAddition.Unit = UnitName.Booking;
      bookingAddition.Price = 0;
      bookingAddition.DisplaySequence = 10; //TODO Jan: zou laatste +10 moeten zijn

      return bookingAddition;
    }
  }

  public class BookingPaymentFactory : Factory<BookingPayment>
  {
    public BookingPaymentFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<BookingPayment> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingPaymentId, cancellationToken);

      var bookingPayment = new BookingPayment();
      EntityManager.AddEntity(bookingPayment);
      bookingPayment.BookingPaymentId = sequence.CurrentId;
      bookingPayment.IsPaid = false;

      return bookingPayment;
    }
  }

  public class BookingCancelConditionFactory : Factory<BookingCancelCondition>
  {
    public BookingCancelConditionFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<BookingCancelCondition> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.BookingCancelConditionId, cancellationToken);

      var bookingCancelCondition = new BookingCancelCondition();
      EntityManager.AddEntity(bookingCancelCondition);
      bookingCancelCondition.BookingCancelConditionId = sequence.CurrentId;

      return bookingCancelCondition;
    }
  }


}