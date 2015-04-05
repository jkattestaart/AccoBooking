using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemGroupSummaryViewModel : BaseScreen<SystemGroup>
  {
    [ImportingConstructor]
    public SystemGroupSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                       IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
    }

    protected override IRepository<SystemGroup> Repository()
    {
      return UnitOfWork.SystemGroups;
    }

  }
}