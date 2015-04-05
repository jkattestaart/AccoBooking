using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;

namespace Common
{
  public class Day : PropertyChangedBase
  {

    #region Properties

    public DateTime Date { get; private set; }
    public bool IsHistory { get; set; }

    public bool HasOverlap { get; set; }

    private bool _isMorningBookable;
    public bool IsMorningBookable
    {
      get { return _isMorningBookable; }
      set
      {
        _isMorningBookable = value;
        NotifyOfPropertyChange(() => IsMorningBookable);
      }
    }

    private bool _isMiddayBookable;
    public bool IsMiddayBookable
    {
      get { return _isMiddayBookable; }
      set
      {
        _isMiddayBookable = value;
        NotifyOfPropertyChange(() => IsMiddayBookable);
      }
    }

    private bool _isMorningBooked;
    public bool IsMorningBooked
    {
      get { return _isMorningBooked; }
      set
      {
        _isMorningBooked = value;
        NotifyOfPropertyChange(() => IsMorningBooked);
      }
    }

    private bool _isMiddayBooked;
    public bool IsMiddayBooked
    {
      get { return _isMiddayBooked; }
      set
      {
        _isMiddayBooked = value;
        NotifyOfPropertyChange(() => IsMiddayBooked);
      }
    }

    private bool _isPossibleArrival;
    public bool IsPossibleArrival
    {
      get { return _isPossibleArrival; }
      set
      {
        _isPossibleArrival = value;
        NotifyOfPropertyChange(() => IsPossibleArrival);
      }
    }

    private ObservableCollection<Booking> _bookings;
    
    public ObservableCollection<Booking> Bookings
    {
      get { return _bookings; }
      private set
      {
        _bookings = value;
        NotifyOfPropertyChange(() => Bookings);
      }
    }

    //kleur van de beschikbaarheid
    public Color RentBrush { get; set; }
    
    private string _rentColor;
    public string RentColor
    {
      get { return _rentColor; }
      set
      {
        _rentColor = value;
        if (!string.IsNullOrEmpty(value))
          RentBrush = ColorFromBrush(value);    
      }
    }
    
    //kleur van de booking arrival
    public Color ArrivalBrush { get; set; }
    
    private string _arrivalColor;
    public string ArrivalColor
    {
      get { return _arrivalColor; }
      set
      {
        _arrivalColor = value;
        if (!string.IsNullOrEmpty(value))
          ArrivalBrush = ColorFromBrush(value);
      }
    }

    //kleur van de booking departure
    public Color DepartureBrush { get; set; }
    
    private string _departureColor;
    public string DepartureColor
    {
      get { return _departureColor; }
      set
      {
        _departureColor = value;
        if (!string.IsNullOrEmpty(value))
          DepartureBrush = ColorFromBrush(value);
      }
    }

    public bool IsSeason { get; set; }

    #endregion

    #region Constructor

    public Day(DateTime date)
    {
      IsMorningBooked = false;
      IsMorningBookable = false;
      IsMiddayBooked = false;
      IsMiddayBookable = false;
      Bookings = new ObservableCollection<Booking>();
      Bookings.CollectionChanged += BookingsCollectionChanged;
      Date = date;
    }

    #endregion

    public Color ColorFromBrush(string color)
    {
      return Color.FromArgb(byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber),
                            byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber),
                            byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber),
                            byte.Parse(color.Substring(7, 2), NumberStyles.HexNumber));
    }

    void BookingsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      var bookings = sender as IList<Booking>;
      IsMorningBooked = bookings.Any(p => p.StartDate < Date);
      IsMiddayBooked = bookings.Any(p => p.EndDate > Date);
      //if (IsMorningBooked)
      //  IsMorningBookable = false;
      //if (IsMiddayBooked)
      //  IsMiddayBookable = false;
    }

    public override string ToString()
    {
      return Date.ToString();
    }

  }

}
