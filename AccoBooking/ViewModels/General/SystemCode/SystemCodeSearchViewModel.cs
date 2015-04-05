using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemCodeSearchViewModel : BaseSearchViewModel<SystemCode, SystemCodeListItem>
  {

    [ImportingConstructor]
    public SystemCodeSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    }

    protected override Task<IEnumerable<SystemCodeListItem>> ExecuteQuery()
    {
        return UnitOfWork.SystemCodeSearchService.FindSystemCodesAsync(_parentid, SearchText, CancellationToken.None);
    }

  }



}
