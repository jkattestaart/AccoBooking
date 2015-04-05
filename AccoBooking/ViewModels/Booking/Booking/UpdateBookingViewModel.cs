using System.ComponentModel.Composition;
using AccoBooking.ViewModels.Acco;
using Caliburn.Micro;
using Cocktail;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class UpdateBookingViewModel : BaseScreen<DomainModel.Booking>
  {
    [ImportingConstructor]
    public UpdateBookingViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   BookingManagementViewModel bookingManagement,
                                   IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      UpdateBookingMaster = bookingManagement;
    }

    public BookingManagementViewModel UpdateBookingMaster { get; set; }


    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    //protected override void OnActivate()
    //{
    //  base.OnActivate();

    //}

    public override BaseScreen<DomainModel.Booking> Start(int entityid)
    {
      _entityid = entityid;
      UpdateBookingMaster.UseSearch = false;
      UpdateBookingMaster.Parent = this;
      ((IActivate)UpdateBookingMaster).Activate();
      UpdateBookingMaster.StartDetail(entityid);
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
