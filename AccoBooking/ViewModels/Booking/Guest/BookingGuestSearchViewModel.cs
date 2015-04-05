using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingGuestSearchViewModel : BaseSearchViewModel<BookingGuest, BookingGuestListItem>
  {

    [ImportingConstructor]
    public BookingGuestSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<System.Collections.Generic.IEnumerable<BookingGuestListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingGuestSearchService.FindBookingGuestsAsync(_parentid, CancellationToken.None);
    } 

  }

  
}
