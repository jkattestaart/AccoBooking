using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using DomainModel;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class SelectDepartureViewModel : BaseScreen<AccoRent>
  {
    public static DateTime Day;

    [ImportingConstructor]
    public SelectDepartureViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                    DepartureManagementViewModel departure,
                                    IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)

    {
      Departure = departure;
    }

    public DepartureManagementViewModel Departure { get; set; }
    
    public void Cancel()
    {
      if (Parent.GetType() == typeof(AccoAvailablePeriodCalenderViewModel))
        (Parent as AccoAvailablePeriodCalenderViewModel).Start();
      TryClose();
    }

    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

    public override BaseScreen<AccoRent> Start(int entityid)
    {
      //return base.Start(entityid);
      return this;
    }

    public BaseScreen<AccoRent> Start(DateTime day)
    {
      //_entityid = entityid;
      Departure.Parent = this;
      DepartureSearchViewModel.IsDaySelected = true;
      DepartureSearchViewModel.SelectedDay = day;    //zetten van de waarde voor de search
      ((IActivate)Departure).Activate();
      Departure.Start(SessionManager.CurrentAcco.AccoId);
     
      return this;
    }      

  }

}
