using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class NewBookingFromDepartureViewModel : BaseScreen<AccoRent>
  {
    private ShellViewModel _shellViewModel;

    public static DateTime Day;

    [ImportingConstructor]
    public NewBookingFromDepartureViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                            DepartureManagementViewModel departure,
                                            ShellViewModel shellViewModel,
                                            IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)

    {
      Departure = departure;
      _shellViewModel = shellViewModel;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      Start(SessionManager.CurrentAcco.AccoId);
    }

    public DepartureManagementViewModel Departure { get; set; }
    
    public async void Cancel()
    {
      //if (Parent.GetType() == typeof (AccoAvailablePeriodCalenderViewModel))
      //{
      //  (Parent as AccoAvailablePeriodCalenderViewModel).Start();
      //  TryClose();
      //}
      //else
      //{
      await _shellViewModel.NavigateToAvailability();
      //}
    }

    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

    public override BaseScreen<AccoRent> Start(int entityid)
    {
      //_entityid = entityid;
      Departure.Parent = this;
      DepartureSearchViewModel.IsDaySelected = false;
      ((IActivate)Departure).Activate();
      Departure.Start(entityid);

      return this;
    }

  }

}
