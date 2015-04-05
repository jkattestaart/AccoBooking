using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoSearchViewModel : BaseSearchViewModel<DomainModel.Acco, AccoListItem>
  {

    [ImportingConstructor]
    public AccoSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
      Search();
    }

    // NIETS DOEN
    public override void Search(int selectedid)
    {
      //base.Search(selectedid);
    }

    protected override Task<IEnumerable<AccoListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoSearchService.FindAccoesAsync(SearchText, CancellationToken.None);
    } 

  }
}
