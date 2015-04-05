using System.ComponentModel.Composition;
using AccoBooking.ViewModels.Acco;
using Caliburn.Micro;
using Cocktail;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class CreateBookingViewModel : BaseScreen<DomainModel.Booking>
  {
    public static AvailableDepartureListItem Departure;

    [ImportingConstructor]
    public CreateBookingViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   BookingManagementViewModel bookingManagement,
                                   IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      CreateBookingMaster = bookingManagement;
    }

    public BookingManagementViewModel CreateBookingMaster { get; set; }


    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    public override BaseScreen<DomainModel.Booking> Start(int entityid)
    {
      _entityid = entityid;
      CreateBookingMaster.UseSearch = false;
      CreateBookingMaster.Parent = this;
      ((IActivate)CreateBookingMaster).Activate();
      CreateBookingMaster.StartCreate(entityid);
      return this;
    }

    public void Cancel()
    {
      if (Parent.GetType() == typeof(DepartureManagementViewModel))
        (Parent as DepartureManagementViewModel).Cancel();
      TryClose();
    }

  }
}
