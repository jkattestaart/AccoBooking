//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System.ComponentModel;
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoRentSummaryViewModel : BaseScreen<AccoRent>
  {
    [ImportingConstructor]
    public AccoRentSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                    SystemCodeListViewModel WeekdayListViewModel1,
                                    SystemCodeListViewModel WeekdayListViewModel2,
                                    IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      WeekExchangeDayList = WeekdayListViewModel1;
      WeekExchangeDayList.PropertyChanged += WeekExchangeDayListOnPropertyChanged;

      OptionalWeekExchangeDayList = WeekdayListViewModel2;
      OptionalWeekExchangeDayList.PropertyChanged += OptionalWeekExchangeDayListOnPropertyChanged;
     
    }

    private void WeekExchangeDayListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
        if (Entity != null)
          ((AccoRent)Entity).WeekExchangeDay = WeekExchangeDayList.ShortName;     
    }

    private void OptionalWeekExchangeDayListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
        if (Entity != null)
          ((AccoRent)Entity).OptionalWeekExchangeDay = OptionalWeekExchangeDayList.ShortName;

    }

    public SystemCodeListViewModel WeekExchangeDayList { get; set; }
    public SystemCodeListViewModel OptionalWeekExchangeDayList { get; set; }

    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        base.Entity = value;
        if (Entity != null)
        {
          if (_entity != null) Entity.PropertyChanged -= EntityPropertyChanged;
          Entity.PropertyChanged += EntityPropertyChanged;
          WeekExchangeDayList.ShortName = ((AccoRent)Entity).WeekExchangeDay;
          OptionalWeekExchangeDayList.ShortName = ((AccoRent)Entity).OptionalWeekExchangeDay;
        }
      }
    }

    void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsAvailablePerWeek")
      {
        WeekExchangeDayList.IsEnabled = ((AccoRent)Entity).IsAvailablePerWeek;
        OptionalWeekExchangeDayList.IsEnabled = ((AccoRent)Entity).IsAvailablePerWeek;
      }
    }

    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      WeekExchangeDayList.Start("WEEKDAY");
      OptionalWeekExchangeDayList.Start("WEEKDAY");
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        WeekExchangeDayList.PropertyChanged -= WeekExchangeDayListOnPropertyChanged;
        OptionalWeekExchangeDayList.PropertyChanged -= OptionalWeekExchangeDayListOnPropertyChanged;
      }
    }


  }
}