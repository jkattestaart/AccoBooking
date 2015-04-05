using System;

namespace Common
{
  public class Availability
  {    

    #region Properties

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    #endregion

    public Availability(DateTime startDate, DateTime endDate)
    {
      StartDate = startDate;
      EndDate = endDate;
    }

  }
}
