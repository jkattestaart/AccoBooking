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
  public class AccoPayPatternSearchViewModel : BaseSearchViewModel<AccoPayPattern, AccoPayPatternListItem>
  {

    [ImportingConstructor]
    public AccoPayPatternSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<AccoPayPatternListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoPayPatternSearchService.FindAccoPayPatternsAsync(_parentid, CancellationToken.None);
    }

  }

}
