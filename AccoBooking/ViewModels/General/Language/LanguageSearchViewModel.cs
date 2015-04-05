using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class LanguageSearchViewModel : BaseSearchViewModel<Language, LanguageListItem>
  {

    [ImportingConstructor]
    public LanguageSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<System.Collections.Generic.IEnumerable<LanguageListItem>> ExecuteQuery()
    {
      return UnitOfWork.LanguageSearchService.FindLanguagesAsync(_parentid, CancellationToken.None);
    } 

  }

}
