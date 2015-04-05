using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTemplateSearchViewModel : BaseSearchViewModel<MailTemplate, MailTemplateListItem>
  {
   
    [ImportingConstructor]
    public MailTemplateSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
     
    }

    protected override Task<IEnumerable<MailTemplateListItem>> ExecuteQuery()
    {
      return UnitOfWork.MailTemplateSearchService.FindMailTemplatesAsync(_parentid, CancellationToken.None);
    } 

  }

}
