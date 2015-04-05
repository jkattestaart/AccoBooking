using System;

namespace Common
{
  public class Booking
  {

    #region Properties

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    #endregion

    public Booking(DateTime startDate, DateTime endDate)
    {
      StartDate = startDate;
      EndDate = endDate;
    }

  }
}
