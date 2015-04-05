using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemCodeSummaryViewModel : BaseScreen<SystemCode>
  {
    [ImportingConstructor]
    public SystemCodeSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                      IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
    }

    protected override IRepository<SystemCode> Repository()
    {
      return UnitOfWork.SystemCodes;
    }
 
  }
}