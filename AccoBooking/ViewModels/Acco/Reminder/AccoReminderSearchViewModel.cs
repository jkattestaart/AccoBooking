using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoReminderSearchViewModel : BaseSearchViewModel<AccoReminder, AccoReminderListItem>
  {
    [ImportingConstructor]
    public AccoReminderSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
      
    }

    protected async override Task<IEnumerable<AccoReminderListItem>> ExecuteQuery()
    {
      var items = await UnitOfWork.AccoReminderSearchService.FindAccoRemindersAsync(_parentid, CancellationToken.None);
      return items.OrderBy(x => x.DisplaySequence);
      
    }

  }


}
