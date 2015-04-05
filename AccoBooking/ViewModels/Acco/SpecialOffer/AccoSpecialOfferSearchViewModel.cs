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
  public class AccoSpecialOfferSearchViewModel : BaseSearchViewModel<AccoSpecialOffer, AccoSpecialOfferListItem>
  {

    [ImportingConstructor]
    public AccoSpecialOfferSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<AccoSpecialOfferListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoSpecialOfferSearchService.FindAccoSpecialOffersAsync(_parentid, CancellationToken.None);
    }

    
  }
}
