using System.Collections.Generic;
using System.Globalization;
using Caliburn.Micro;

namespace Common
{
  public class Month: PropertyChangedBase
  {

    #region Properties

    public int YearNr { get; private set; }
    public int MonthNr { get; private set; }
    public string Header { get; set; }

    private List<MonthItem> _items;
    public List<MonthItem> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        NotifyOfPropertyChange(() => Items);
      }
    }

    private List<string> _weekDayNames;
    public List<string> WeekDayNames
    {
      get { return _weekDayNames; }
      set
      {
        _weekDayNames = value;
        NotifyOfPropertyChange(() => WeekDayNames);
      }
    }

    private List<int> _weekNumbers;
    public List<int> WeekNumbers
    {
      get { return _weekNumbers; }
      set
      {
        _weekNumbers = value;
        NotifyOfPropertyChange(() => WeekNumbers);
      }
    }

    #endregion

    public Month(int year, int month)
    {
      this.YearNr = year;
      this.MonthNr = month;
      this.Header = DateTimeFormatInfo.CurrentInfo.MonthNames[month - 1] + " " + year.ToString();
      this.Items = new List<MonthItem>();
      this.WeekDayNames = new List<string>();
      this.WeekNumbers = new List<int>();
    }

    public override string ToString()
    {
      return this.YearNr.ToString("D2") + "-" + this.MonthNr.ToString("D2");
    }


  }
}
