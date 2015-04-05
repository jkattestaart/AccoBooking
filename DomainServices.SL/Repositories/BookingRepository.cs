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

using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
  public class BookingRepository : Repository<Booking>
  {
    public BookingRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      // booking payments altijd laden
      var ers = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      var navProp = Booking.PropertyMetadata.BookingPayments;
      navProp.ReferenceStrategy = ers;

      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.Bookings
        .Where(c => c.BookingId == (int) keyValues[0])
        .Include(c => c.Acco)
        .Include(c => c.BookingGuests)
        .Include(c => c.BookingAdditions)
        .Include(c => c.BookingReminders)
        .Include(c => c.BookingPayments);
    }
  }


  //@@@ JKT ivm named query aparte repository
  public class ExpiredBookingRepository : BaseRepository<Booking>
  {
    public ExpiredBookingRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    public override IEntityQuery<Booking> GetQuery()
    {
      return EntityManager.ExpiredBookings;
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return GetQuery().Where(c => c.BookingId == (int)keyValues[0])
        .Include(c => c.Acco)
        .Include(c => c.BookingGuests)
        .Include(c => c.BookingAdditions)
        .Include(c => c.BookingReminders)
        .Include(c => c.BookingPayments);
    }

  }

  public class BookingGuestRepository : Repository<BookingGuest>
  {
    public BookingGuestRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.BookingGuests
        .Where(c => c.BookingGuestId == (int) keyValues[0])
        .Include(c => c.Booking)
        .Include(c => c.Booking.Acco)
        .Include(c => c.Booking.BookingAdditions)
        .Include(c => c.Booking.BookingReminders)
        .Include(c => c.Booking.BookingPayments);
    }
  }


  public class BookingReminderRepository : Repository<BookingReminder>
  {
    public BookingReminderRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.BookingReminders
        .Where(c => c.BookingReminderId == (int) keyValues[0])
        .Include(c => c.Booking)
        .Include(c => c.Booking.Acco)
        .Include(c => c.Booking.BookingPayments);
    }
  }


  public class BookingAdditionRepository : Repository<BookingAddition>
  {
    public BookingAdditionRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.BookingAdditions
        .Where(c => c.BookingAdditionId == (int) keyValues[0])
        .Include(c => c.Booking)
        .Include(c => c.Booking.Acco)
        .Include(c => c.AccoAddition)
        .Include(c => c.Booking.BookingAdditions);
    }
  }


  public class BookingCancelConditionRepository : Repository<BookingCancelCondition>
  {
    public BookingCancelConditionRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.BookingCancelConditions
        .Where(c => c.BookingCancelConditionId == (int) keyValues[0])
        .Include(c => c.Booking)
        .Include(c => c.Booking.Acco);
    }
  }

  public class BookingPaymentRepository : Repository<BookingPayment>
  {
    public BookingPaymentRepository(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public new AccoBookingEntities EntityManager
    {
      get { return (AccoBookingEntities) base.EntityManager; }
    }

    protected override IEntityQuery GetKeyQuery(params object[] keyValues)
    {
      //EntityManager.DefaultEntityReferenceStrategy = new EntityReferenceStrategy(EntityReferenceLoadStrategy.Load, MergeStrategy.PreserveChanges);
      return EntityManager.BookingPayments
        .Where(c => c.BookingPaymentId == (int) keyValues[0])
        .Include(c => c.Booking)
        .Include(c => c.Booking.Acco);
    }
  }



}