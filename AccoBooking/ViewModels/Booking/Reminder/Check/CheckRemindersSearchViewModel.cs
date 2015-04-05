using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class CheckRemindersSearchViewModel : BaseSearchViewModel<DomainModel.Booking, AccoNotificationListItem>
  {

    [ImportingConstructor]
    public CheckRemindersSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    
    }


    protected override async Task<IEnumerable<AccoNotificationListItem>> ExecuteQuery()
    {
      await CheckRemminderService.ExecuteAsync(SessionManager.CurrentOwner.AccoOwnerId);

      var reminders = await UnitOfWork.AccoNotificationSearchService.FindAccoNotificationsAsync(SessionManager.CurrentAcco.AccoOwnerId, CancellationToken.None);
      return reminders.OrderBy(r => r.ExpirationDate);
    } 

  }
}
