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
  public class AccoTrusteeSearchViewModel : BaseSearchViewModel<AccoTrustee, AccoTrusteeListItem>
  {

    [ImportingConstructor]
    public AccoTrusteeSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<AccoTrusteeListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoTrusteeSearchService.FindAccoTrusteesAsync(SessionManager.CurrentAcco.AccoOwnerId, CancellationToken.None);
    }

    
  }
}
