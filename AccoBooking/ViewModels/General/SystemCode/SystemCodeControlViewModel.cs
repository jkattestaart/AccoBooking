using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemCodeControlViewModel : BaseMasterViewModel<SystemCodeSearchViewModel,SystemCodeDetailViewModel, SystemCodeListItem, SystemCode>
  {

  
    [ImportingConstructor]
    public SystemCodeControlViewModel(ExportFactory<SystemCodeSearchViewModel> searchFactory,
                                      ExportFactory<SystemCodeDetailViewModel> detailFactory,
                                      IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                      IDialogManager dialogManager,
                                      ToolbarViewModel toolbar,
                                      ToolbarViewModel bottomToolbar
                                     )
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {

    }

    protected override IRepository<SystemCode> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.SystemCodes;
    }
  
  }
}