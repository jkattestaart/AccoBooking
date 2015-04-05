using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Cocktail;
using Common;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SelectDepartureListViewModel : BaseScreen<DomainModel.Acco>
  {
    private string _startWeekday;
    
    public static int BookingId;
    public static DateTime Day;

    [ImportingConstructor]
    public SelectDepartureListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                            AccoSpecialOfferSearchTop2ViewModel specialOffer,
                                            IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      SpecialOffer = specialOffer;
      _startWeekday = SessionManager.CurrentAcco.StartWeekdayCalendar.ToUpper();
    }

    public DateTime BeginCalendar { get; set; }
    public DateTime Arrival { get; set; }
    public BindableCollection<Month> Months { get; set; }
    public DateTime StartPeriod { get; set; }
    public DateTime EndPeriod { get; set; }

    public event EventHandler<NewArrivalSelectedEventArgs> NewArrivalSelected;
  
    public Dictionary<DateTime, Day> Days { get; set; }
    public AccoSpecialOfferSearchTop2ViewModel SpecialOffer { get; set; }

    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    public override Entity Entity
    {
      get { return base.Entity; }
      set
      {
        {
          if (value != null)
          {
            base.Entity = value;

            Months = new BindableCollection<Month>();
            Days = new Dictionary<DateTime, Day>();
            GetDays();
            BuildCalendar();

            using (Busy.GetTicket())
              AddAvailabilityAndBookings();

          }
        }
      }
    }

    
    private void GetDays()
    {
     

      //A month not always begins with the first day and not always ends with the last day
      var fd = BeginCalendar.AddMonths(-3).Date;
      var td = BeginCalendar.AddMonths(6).Date;

      for (DateTime d = fd; d <= td; d = d.AddDays(1))
      {
        if (!Days.ContainsKey(d))
          Days.Add(d, new Day(d) { IsHistory = (d < DateTime.Today.Date), IsPossibleArrival = false});
      }
    }

    public void DayClicked(Day day)
    {
      if (NewArrivalSelected != null)  
        NewArrivalSelected(this, new NewArrivalSelectedEventArgs(day.Date));
    }

    private void BuildCalendar()
    {
      DateTime startOfMonth;
      DateTime endOfMonth;

      DayOfWeek lastDayOfWeek;

      StartPeriod = BeginCalendar.AddMonths(-1).Date;
      EndPeriod = BeginCalendar.AddMonths(2).Date;

      Months = new BindableCollection<Month>();

      var m = StartPeriod.Month;
      var y = StartPeriod.Year;


      do
      {
        var month = Months.FirstOrDefault(p => p.MonthNr == m) ?? new Month(y, m);

        if (month.Items.Count == 0)
        {

          //Get the start and the enddate of this month
          startOfMonth = new DateTime(y, m, 1);
          endOfMonth = new DateTime(y, m, DateTime.DaysInMonth(y, m));

          while (startOfMonth.DayOfWeek.ToString().ToUpper() != _startWeekday) //.Default.CalendarStartDay)
            startOfMonth = startOfMonth.AddDays(-1);

          lastDayOfWeek = startOfMonth.AddDays(6).DayOfWeek;

          while (endOfMonth.DayOfWeek != lastDayOfWeek)
            endOfMonth = endOfMonth.AddDays(- 1);


          //Add days to the month
          List<MonthItem> items = new List<MonthItem>();

          for (DateTime d = startOfMonth; d <= endOfMonth; d = d.AddDays(1))
            items.Add(new MonthItem(Days[d]) {IsActive = (d.Month == m)});

          month.Items = items;

          //Get the weekdays
          month.WeekDayNames = month.Items.Take(7).Select(p => p.Day.Date.ToString("ddd")).ToList();

          //Get weeknumbers
          month.WeekNumbers =
            month.Items.Where(p => p.Day.Date.DayOfWeek == DayOfWeek.Monday)
              .Select(
                p =>
                  CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(p.Day.Date, CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday))
              .ToList();

          if (!Months.Contains(month))
            Months.Add(month);
        }

        m++;
        if (m > 12)
        {
          m = 1;
          y++;
        }

      } while (m != EndPeriod.Month);

      if (Months.Count > 3)
      {
        var index = 0;
        for (int i = 0; i < Months.Count; i++)
        {
          if (Months[i].MonthNr == Arrival.Month)
            index = i;
        }
        if (index == 1)
          Months.RemoveAt(3);
        else if (index ==2)
          Months.RemoveAt(0);
        //if (index - 1 > 0)
        //  Months.RemoveAt(0);
        //else if (index + 1 > 2)
        //  Months.RemoveAt(3);
      }

      StartPeriod = Months[0].Items[0].Day.Date;
      EndPeriod = Months[2].Items[Months[2].Items.Count - 1].Day.Date;

      NotifyOfPropertyChange(() => Months);
    }

    private async Task AddAvailabilityAndBookings()
    {
      //seizoenen
      foreach (var season in ((DomainModel.Acco)Entity).AccoSeasons.Where(x => (x.SeasonStart >= StartPeriod && x.SeasonStart <= EndPeriod) || (x.SeasonEnd >= StartPeriod && x.SeasonEnd <= EndPeriod)))
      {
        for (DateTime d = season.SeasonStart;
          d <= season.SeasonEnd;
          d = d.AddDays(1))
        {
          if (d >= StartPeriod && d <= EndPeriod)
          {
            Days[d].IsSeason = true;
            Days[d].IsMorningBookable = true;
            Days[d].IsMiddayBookable = true;
            Days[d].RentColor = season.AccoRent.Color;
            Days[d].ArrivalColor = season.AccoRent.Color;
            Days[d].DepartureColor = season.AccoRent.Color;
            if (season.AccoRent.IsAvailablePerNight)
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;
            if (season.AccoRent.IsAvailablePerWeekend && (d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Monday))
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;
            if (season.AccoRent.IsAvailablePerWeek &&
                (d.DayOfWeek.ToString().ToUpper() == season.AccoRent.WeekExchangeDay ||
                 d.DayOfWeek.ToString().ToUpper() == season.AccoRent.OptionalWeekExchangeDay))
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;

          }
        }
      }

      //basis rente
      var rent = ((DomainModel.Acco)Entity).AccoRent;

      if (rent != null && (rent.IsAvailablePerNight || rent.IsAvailablePerWeekend || rent.IsAvailablePerWeek))
        for (DateTime d = StartPeriod;
          d <= EndPeriod;
          d = d.AddDays(1))
        {
          if (!Days[d].IsSeason)
          {
            Days[d].IsMorningBookable = true;
            Days[d].IsMiddayBookable = true;
            Days[d].RentColor = rent.Color; //fill rectangle with same colors
            Days[d].ArrivalColor = rent.Color;
            Days[d].DepartureColor = rent.Color;
            if (rent.IsAvailablePerNight)
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;
            if (rent.IsAvailablePerWeekend && (d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Monday))
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;
            if (rent.IsAvailablePerWeek &&
                (d.DayOfWeek.ToString().ToUpper() == rent.WeekExchangeDay ||
                 d.DayOfWeek.ToString().ToUpper() == rent.OptionalWeekExchangeDay))
              Days[d].IsPossibleArrival = Days[d].Date > DateTime.Now;
          }
        }


      //geblokkeerd
      var blocks = ((DomainModel.Acco)Entity).Bookings.Where(x => x.IsBlock && (x.Arrival >= StartPeriod && x.Arrival <= EndPeriod) || (x.Departure >= StartPeriod && x.Departure <= EndPeriod));

      foreach (var booking in blocks)
      {
        if (!booking.IsBlock)
          continue;

        for (DateTime d = booking.Arrival;
             d <= booking.Departure;
             d = d.AddDays(1))

          if (d >= StartPeriod && d <= EndPeriod)
          {
            Days[d].Bookings.Add(new Common.Booking(booking.Arrival,
                                                    booking.Departure));

            if (d == booking.Arrival)
            {
              Days[d].IsMiddayBookable = false;
              Days[d].IsMiddayBooked = false;
              Days[d].IsPossibleArrival = false;
              Days[d].ArrivalColor = booking.BookingColor ?? SessionManager.CurrentAcco.ColorBlock;

            }
            else if (d == booking.Departure)
            {
              Days[d].IsMorningBookable = false;
              Days[d].IsMorningBooked = false;
              Days[d].DepartureColor = booking.BookingColor ?? SessionManager.CurrentAcco.ColorBlock;
            }
            else
            {
              Days[d].IsMorningBookable = false;
              Days[d].IsMiddayBookable = false;
              Days[d].IsMorningBooked = false;
              Days[d].IsMiddayBooked = false;
              Days[d].IsPossibleArrival = false;
              Days[d].RentColor = booking.BookingColor ?? SessionManager.CurrentAcco.ColorBlock;
              Days[d].ArrivalColor = booking.BookingColor ?? SessionManager.CurrentAcco.ColorBlock;
              Days[d].DepartureColor = booking.BookingColor ?? SessionManager.CurrentAcco.ColorBlock;
            }
          }
      }

      //boekingen
      var bookings = ((DomainModel.Acco)Entity).Bookings.Where(x => x.Status != BookingStatus.Cancelled && x.Status != BookingStatus.Expired && !x.IsBlock && (x.Arrival >= StartPeriod && x.Arrival <= EndPeriod) || (x.Departure >= StartPeriod && x.Departure <= EndPeriod));

      foreach (var booking in bookings)
      {
        if (booking.Status == BookingStatus.Cancelled || booking.Status==BookingStatus.Expired)
          continue;

        for (DateTime d = booking.Arrival;
             d <= booking.Departure;
             d = d.AddDays(1))

          if (d >= StartPeriod && d <= EndPeriod)
          {
            Days[d].Bookings.Add(new Common.Booking(booking.Arrival,
                                                    booking.Departure));

            if (d == booking.Arrival)
            {
              Days[d].IsMiddayBooked = true;
              Days[d].IsPossibleArrival = false;
              Days[d].ArrivalColor = booking.BookingColor ?? Colors.Red.ToString();
            }
            else if (d == booking.Departure)
            {
              Days[d].IsMorningBooked = true;
              Days[d].DepartureColor = booking.BookingColor ?? Colors.Red.ToString();
            }
            else
            {
              Days[d].IsMorningBooked = true;
              Days[d].IsMiddayBooked = true;
              Days[d].IsPossibleArrival = false;
              Days[d].RentColor = booking.BookingColor ?? Colors.Red.ToString();
              Days[d].ArrivalColor = booking.BookingColor ?? Colors.Red.ToString();
              Days[d].DepartureColor = booking.BookingColor ?? Colors.Red.ToString();
            }
          }
      }


    }

    public void NextPeriod()
    {
      BeginCalendar = BeginCalendar.AddMonths(3);
      
      Start(SessionManager.CurrentAcco.AccoId);
    }

    public void PreviousPeriod()
    {
      BeginCalendar = BeginCalendar.AddMonths(-3);

      Start(SessionManager.CurrentAcco.AccoId);
    }

    public override BaseScreen<DomainModel.Acco> Start(int entityid)
    {

      SpecialOffer.Start(entityid);
      ((IActivate)SpecialOffer).Activate();
      return base.Start(entityid);
    }

    public DateTime FindFirstArrival()
    {
      for (DateTime d = DateTime.Now.Date; d <= EndPeriod; d = d.AddDays(1))
      {
        if (Days[d].IsPossibleArrival)
          return d;
      }
      return DateTime.Now;
    }
  }

  public class NewArrivalSelectedEventArgs : EventArgs
  {
    public DateTime Arrival { get; set; }
 
    public NewArrivalSelectedEventArgs(DateTime arrival)
    {
      Arrival = arrival;
    }
  }

}
