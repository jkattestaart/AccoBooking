using System.ComponentModel.Composition;
using AccoBooking.ViewModels.Acco;
using Caliburn.Micro;
using Cocktail;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SearchBookingViewModel : BaseScreen<DomainModel.Booking>
  {
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public SearchBookingViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                  BookingManagementViewModel bookingManagement,
                                  ShellViewModel shellViewModel,
                                  IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      _shellViewModel = shellViewModel;
      SearchBookingMaster = bookingManagement;
    }

    public BookingManagementViewModel SearchBookingMaster { get; set; }


    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      _shellViewModel.BuildMenu("");
      Start(0);
    }

    public override BaseScreen<DomainModel.Booking> Start(int entityid)
    {
      _entityid = entityid;
      SearchBookingMaster.UseSearch = true;
      SearchBookingMaster.Parent = this;
      ((IActivate)SearchBookingMaster).Activate();
      SearchBookingMaster.StartSearch();
      return this;
    }

    public void Cancel()
    {
      if (Parent.GetType() == typeof(AccoAvailablePeriodCalenderViewModel))
        (Parent as AccoAvailablePeriodCalenderViewModel).Start();
      TryClose();
    }

  }


}
