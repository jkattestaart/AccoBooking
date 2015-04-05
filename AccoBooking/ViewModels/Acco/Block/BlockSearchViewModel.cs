using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BlockSearchViewModel : BaseSearchViewModel<DomainModel.Booking, BlockListItem>
  {
    [ImportingConstructor]
    public BlockSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
     
    }

    protected override Task<IEnumerable<BlockListItem>> ExecuteQuery()
    {
      return UnitOfWork.BookingSearchService.FindBlocksAsync(SessionManager.CurrentAcco.AccoId, CancellationToken.None);
    }

  }
}
