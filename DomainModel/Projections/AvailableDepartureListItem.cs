using System.Runtime.Serialization;
using System;

namespace DomainModel.Projections
{
  [DataContract]
  public class AvailableDepartureListItem : ListItemBase
  {
    private string _arrivalWeekDay;
    private string _departureWeekDay;

    [DataMember]
    public DateTime Arrival { get; set; }

    [DataMember]
    public string ArrivalWeekDay
    {
      get
      {
        if (string.IsNullOrEmpty(_arrivalWeekDay))
          return Arrival.DayOfWeek.ToString().ToUpper();
        return _arrivalWeekDay;
      }
      set { _arrivalWeekDay = value; }
    }

    [DataMember]
    public DateTime Departure { get; set; }

    [DataMember]
    public string DepartureWeekDay
    {
      get
      {
        if (string.IsNullOrEmpty(_departureWeekDay))
          return Departure.DayOfWeek.ToString().ToUpper();
        return _departureWeekDay;
      }
      set { _departureWeekDay = value; }
    }

    [DataMember]
    public int Nights { get; set; }

    [DataMember]
    public decimal Rent { get; set; }

    
    public decimal RentPerNight
    {
      get
      {
        if ((Rent > 0) && (Nights > 0))
          return Rent/Nights;

        return 0;
      }
    }

    [DataMember]
    public string PeriodUnit { get; set; }

  }
}