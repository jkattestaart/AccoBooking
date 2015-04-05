using System.ComponentModel.Composition;

using Cocktail;
using DomainModel;
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class DepartureDetailViewModel : BaseDetailViewModel<AccoRent>
  {
    [ImportingConstructor]
    public DepartureDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                              DepartureSummaryViewModel availablePeriodSummary,
                                              IDialogManager dialogManager)
      : base(unitOfWorkManager, null, availablePeriodSummary, dialogManager)
    {

    }

    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

    protected override IFactory<AccoRent> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoRentFactory;
    }

    //JKT parent moet gezet worden bij create
    protected override void OnCreateEntity(AccoRent entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);
      entity.AccoId = parentid;
    }
 
  }
}
