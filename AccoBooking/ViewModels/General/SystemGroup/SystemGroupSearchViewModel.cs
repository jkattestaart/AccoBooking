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
  public class SystemGroupSearchViewModel :  BaseSearchViewModel<SystemGroup, SystemGroupListItem>
  {
    [ImportingConstructor]
    public SystemGroupSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkmanager)
      : base(unitOfWorkmanager)
    {
      
    }

    protected override Task<IEnumerable<SystemGroupListItem>> ExecuteQuery()
    {
      return UnitOfWork.SystemGroupSearchService.FindSystemGroupsAsync(SearchText,CancellationToken.None);
    }


  }


}
