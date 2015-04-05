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
  public class AccoRentSearchViewModel : BaseSearchViewModel<AccoRent, AccoRentListItem>
  {
    [ImportingConstructor]
    public AccoRentSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
     
    }

    protected override Task<IEnumerable<AccoRentListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoRentSearchService.FindAccoRentsAsync(_parentid, CancellationToken.None);
    }

  }
}
