using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingReminderSearchViewModel : BaseSearchViewModel<BookingReminder, BookingReminderListItem>
  {

    [ImportingConstructor]
    public BookingReminderSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<System.Collections.Generic.IEnumerable<BookingReminderListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingReminderSearchService.FindBookingRemindersAsync(_parentid,CancellationToken.None);
    } 

  }

  
}
