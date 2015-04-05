using System;

namespace Common
{
  public class MonthItem
  {

    #region Properties

    public Day Day { get; private set; }
    public String Label { get; private set; }
    public bool IsActive { get; set; }

    public bool IsPossibleArrival
    {
      get { return Day.IsPossibleArrival; }
    }

    #endregion

    #region Constructor

    public MonthItem(Day day)
    {
      this.Day = day;
      this.Label = day.Date.Day.ToString();
    }

    #endregion

    public override string ToString()
    {
      return this.Day.Date.ToString();
    }

  }

}
