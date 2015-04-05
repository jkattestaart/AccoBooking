using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingAdditionSearchViewModel : BaseSearchViewModel<BookingAddition, BookingAdditionListItem>
  {

    [ImportingConstructor]
    public BookingAdditionSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<System.Collections.Generic.IEnumerable<BookingAdditionListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingAdditionSearchService.FindBookingAdditionsAsync(_parentid,CancellationToken.None);
    } 

  }

}
