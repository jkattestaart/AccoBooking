using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemCodeManagementViewModel : BaseManagementViewModel<SystemCodeDetailViewModel, SystemCodeListItem, DomainModel.SystemCode>
  {

  
    [ImportingConstructor]
    public SystemCodeManagementViewModel(SystemCodeSearchViewModel searchPane,
                                         ExportFactory<SystemCodeDetailViewModel> detailFactory,
                                         //IPartFactory<SystemCodeEditorViewModel> systemCodeEditorFactory,
                                         IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                         IDialogManager dialogManager,
                                         ToolbarViewModel toolbar)
      : base(searchPane, detailFactory, unitOfWorkManager, dialogManager, toolbar, null)
    {

    }

    protected override IRepository<SystemCode> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.SystemCodes;
    }
  
  }
}