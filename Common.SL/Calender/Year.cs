using System.Collections.Generic;

namespace Common
{
  public class Year
  {

    #region Properties

    public int YearNr { get; set; }
    public List<Month> Months { get; private set;}
    
    #endregion

    #region Constructors

    public Year(int year)
    {
      this.YearNr = year;
      this.Months = new List<Month>();
    }

    #endregion

    public override string ToString()
    {
      return this.YearNr.ToString();
    }

  }
}
