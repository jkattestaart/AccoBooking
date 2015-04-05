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
  public class AccoSeasonSearchViewModel : BaseSearchViewModel<AccoSeason, AccoSeasonListItem>
  {
    [ImportingConstructor]
    public AccoSeasonSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
     
    }

    protected override Task<IEnumerable<AccoSeasonListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoSeasonSearchService.FindAccoSeasonsAsync(_parentid, CancellationToken.None);
    }

  }
}
