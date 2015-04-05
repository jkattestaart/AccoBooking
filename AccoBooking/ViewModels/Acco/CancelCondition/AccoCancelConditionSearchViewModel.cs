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
  public class AccoCancelConditionSearchViewModel : BaseSearchViewModel<AccoCancelCondition, AccoCancelConditionListItem>
  {

    [ImportingConstructor]
    public AccoCancelConditionSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<AccoCancelConditionListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoCancelConditionSearchService.FindAccoCancelConditionsAsync(_parentid, CancellationToken.None);
    }

    
  }
}
