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
  public class AccoAvailablePeriodListViewModel : BaseScreen<DomainModel.Acco>
  {
    private string _startWeekday;
//    private DateTime _startOfYear;
//    private DateTime _endOfYear;

    public static int BookingId;
    public static DateTime Day;

    [ImportingConstructor]
    public AccoAvailablePeriodListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                            AccoSpecialOfferSearchTop2ViewModel specialOffer,
                                            IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      SpecialOffer = specialOffer;
      _startWeekday = SessionManager.CurrentAcco.StartWeekdayCalendar.ToUpper();
    }

    public DateTime BeginCalendar { get; set; }
    public DateTime StartPeriod { get; set; }
    public DateTime EndPeriod { get; set; }
    public Dictionary<DateTime, Day> Days { get; set; }
    public AccoSpecialOfferSearchTop2ViewModel SpecialOffer { get; set; }

    private BindableCollection<Year> _years;

    public BindableCollection<Year> Years
    {
      get { return _years; }
      private set
      {
        _years = value;
        NotifyOfPropertyChange(() => Years);
      }
    }

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

            Years = new BindableCollection<Year>();
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
      var td = BeginCalendar.AddMonths(15).Date;

      for (DateTime d = fd; d <= td; d = d.AddDays(1))
      {
        if (!Days.ContainsKey(d))
          Days.Add(d, new Day(d) { IsHistory = (d < DateTime.Today.Date), IsPossibleArrival = false});
      }
    }

    public event EventHandler<EventArgs> BookingSelected;

    public event EventHandler<EventArgs> AvailablePeriodSelected;

    public void DayClicked(Day day)
    {
      //DomainModel.Acco.PropertyMetadata.Bookings.GetEntityReference(Entity).Load(MergeStrategy.PreserveChanges);
      //DomainModel.Acco.PropertyMetadata.AccoAvailablePeriods.GetEntityReference(Entity).Load(MergeStrategy.PreserveChanges);

      //start select available period to create a booking
      if (day.IsMiddayBookable && !day.IsMiddayBooked)
      {
        //save the selected date
        Day = day.Date;
        
        if (AvailablePeriodSelected == null)
          return;
        AvailablePeriodSelected(this, new EventArgs() );
      }

      //start update booking
      if (day.IsMorningBooked || day.IsMiddayBooked)
      {
        //save the selected booking
                //save the selected booking
        var booking = ((DomainModel.Acco) Entity).Bookings.First(b => b.IsValid && b.Arrival <= day.Date && b.Departure >= day.Date);
        BookingId = booking.BookingId;
        
        if (BookingSelected == null || booking.IsBlock)
          return;
        BookingSelected(this, new EventArgs());
        
      }

      //start create availability (@@@JKT moved to separate program)
      //if ((!day.IsMorningBookable && !day.IsMorningBooked) || (!day.IsMiddayBookable && !day.IsMiddayBooked))
      //{
      //  AccoAvailablePeriodDialogViewModel.Arrival = day.Date;
      //  yield return _detailAvailabilityDialogFactory.CreatePart();
      //}

    }

    private void BuildCalendar()
    {
      DateTime startOfMonth;
      DateTime endOfMonth;
      DayOfWeek lastDayOfWeek;

      StartPeriod = BeginCalendar.AddMonths(-1).Date;
      EndPeriod = BeginCalendar.AddMonths(11).Date;

      var m = StartPeriod.Month;
      var y = StartPeriod.Year;

      var year = Years.FirstOrDefault(p => p.YearNr == y);

      if (year == null)
      {
        year = new Year(y);
        year.Months.ForEach(p => p.Items.Clear());
      }


      int n = 0;
      do
      {

        var month = year.Months.FirstOrDefault(p => p.MonthNr == m) ?? new Month(y, m);

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

          if (!year.Months.Contains(month))
            year.Months.Add(month);
        }

     
        m++;
        n++;
        if (m > 12)
        {
          m = 1;
          y++;
        }

      } while (m != EndPeriod.Month);

      if (!Years.Contains(year))
        Years.Add(year);

      int index = 0;
      for (int i = 0; i < year.Months.Count; i++)
      {
        if (year.Months[i].MonthNr == BeginCalendar.Month)
          index = i;
      }
      if (index - 1 > 0)
        year.Months.RemoveAt(0);
      else if (index + 1 > 11)
        year.Months.RemoveAt(3);


      StartPeriod = year.Months[0].Items[0].Day.Date;
      EndPeriod = year.Months[11].Items[year.Months[11].Items.Count - 1].Day.Date;

      NotifyOfPropertyChange(() => Years);

    }

    private async Task AddAvailabilityAndBookings()
    {
      try
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
      catch (Exception ex)
      {

        throw;
      }



    }

    public void NextYear()
    {
      BeginCalendar = BeginCalendar.AddMonths(12);
      //StartPeriod = StartPeriod.AddMonths(12);
      //EndPeriod = EndPeriod.AddMonths(12);

      Start(SessionManager.CurrentAcco.AccoId);

    }

    public void PreviousYear()
    {
      BeginCalendar = BeginCalendar.AddMonths(-12);
      //StartPeriod = StartPeriod.AddMonths(-12);
      //EndPeriod = EndPeriod.AddMonths(-12);

      Start(SessionManager.CurrentAcco.AccoId);

    }

    public override BaseScreen<DomainModel.Acco> Start(int entityid)
    {
      SpecialOffer.Start(entityid);
      ((IActivate)SpecialOffer).Activate();
      return base.Start(entityid);     
    }
  }

  public class BookingSelectedEventArgs : EventArgs
  {
    public int BookingId { get; set; }

    public BookingSelectedEventArgs(int bookingid)
    {
      BookingId = bookingid;
    }
  }

  public class ArrivalSelectedEventArgs : EventArgs
  {
    public DateTime Arrival { get; set; }

    public ArrivalSelectedEventArgs(DateTime arrival)
    {
      Arrival = arrival;
    }
  }
}
