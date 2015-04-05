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
  public class AccoSpecialOfferSearchTop2ViewModel : BaseSearchViewModel<AccoSpecialOffer, AccoSpecialOfferListItem>
  {

    [ImportingConstructor]
    public AccoSpecialOfferSearchTop2ViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<AccoSpecialOfferListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoSpecialOfferSearchService.FindAccoSpecialOffersAsync(_parentid, SessionManager.CurrentAcco.AccoOwner.LanguageId, 2, CancellationToken.None);
    }

    
  }
}
