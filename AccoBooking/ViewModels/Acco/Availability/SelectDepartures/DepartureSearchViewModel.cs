using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class DepartureSearchViewModel : BaseSearchViewModel<AccoRent, AvailableDepartureListItem>
  {
    public static DateTime SelectedDay = new DateTime(1,1,1);
    public static bool IsDaySelected = false;

    private DateTime _arrival = DateTime.Now;
    private bool _searchFirstArrival;

    [ImportingConstructor]
    public DepartureSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                    SelectDepartureListViewModel calender)
      : base(unitOfWorkManager)
    {
      Calender = calender;
      Calender.NewArrivalSelected += CalenderNewArrivalSelected;
    }

    void CalenderNewArrivalSelected(object sender, NewArrivalSelectedEventArgs e)
    {
      IsDaySelected = true;
      Arrival = e.Arrival;
      NotifyOfPropertyChange(() => Arrival);
      Start();
    }

    public SelectDepartureListViewModel Calender { get; set; }

    public DateTime Arrival
    {
      get { return _arrival; }
      set { _arrival = value; }
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (!IsDaySelected)
      {
        Arrival = DateTime.Now;
        _searchFirstArrival = true;
      }
      else
      {
        Arrival = SelectedDay;
        _searchFirstArrival = false;
      }
      NotifyOfPropertyChange(() => Arrival);
      Start();   //deze start?
    }

    public override BaseSearchViewModel<AccoRent, AvailableDepartureListItem> Start(int parentid)
    {
      if (IsDaySelected)
      {
        Arrival = SelectedDay;
        _searchFirstArrival = false;
      }
      NotifyOfPropertyChange(() => Arrival);
      Start(); //of deze?
      return base.Start(parentid);
    }

    protected override Task<IEnumerable<AvailableDepartureListItem>> ExecuteQuery()
    {
      return AvailableDeparturesService.ExecuteAsync(SessionManager.CurrentAcco.AccoId, Arrival);
    }

    public override void PostSearch()
    {
      base.PostSearch();

      foreach (var item in Items)
      {
        item.ArrivalWeekDay = SystemCodeService.Description(item.ArrivalWeekDay, SystemGroupName.Weekday);
        item.DepartureWeekDay = SystemCodeService.Description(item.DepartureWeekDay, SystemGroupName.Weekday);
        item.PeriodUnit = SystemCodeService.Description(item.PeriodUnit, SystemGroupName.PeriodUnit);
      }
    }

    public override BaseSearchViewModel<AccoRent, AvailableDepartureListItem> Start()
    {
      //Calender.StartPeriod = Arrival.AddMonths(-1);
      //Calender.EndPeriod = Arrival.AddMonths(2);

      Calender.Parent = this; //Caliburn zet deze nu niet 
      Calender.BeginCalendar = Arrival.Date;
      Calender.Arrival = Arrival.Date;
      ((IActivate)Calender).Activate();
      Calender.Start(SessionManager.CurrentAcco.AccoId);  //waarom deze niet?
      if (_searchFirstArrival)
      {
        _searchFirstArrival = false;
        var firstArrival = Calender.FindFirstArrival();
        if (firstArrival.Date != DateTime.Now.Date)
          CalenderNewArrivalSelected(this, new NewArrivalSelectedEventArgs(firstArrival));
      }
      return base.Start();
    }

    protected override void OnDeactivate(bool close)
    {
      Calender.NewArrivalSelected -= CalenderNewArrivalSelected;
      base.OnDeactivate(close);

    }
  }

 
}
