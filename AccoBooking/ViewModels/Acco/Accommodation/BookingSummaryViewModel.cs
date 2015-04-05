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
using AccoBooking.ViewModels.General;
using Cocktail;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingSummaryViewModel : BaseScreen<DomainModel.Acco>
  {
    [ImportingConstructor]
    public BookingSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   AccoRentListViewModel rentList,
                                   CurrencyListViewModel currencyList,
                                   SystemCodeListViewModel weekdayListViewModel,
                                   IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      RentList = rentList;
      RentList.PropertyChanged += RentListOnPropertyChanged;
      
      WeekdayList = weekdayListViewModel;
      WeekdayList.PropertyChanged += WeekdayListOnPropertyChanged;

      CurrencyList = currencyList;
      CurrencyList.PropertyChanged += CurrencyListOnPropertyChanged;

    }

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(sender, e);
      NotifyOfPropertyChange(() => CanSave);
    }

    private void RentListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).BaseRentId = RentList.ItemId;
        }
      }
    }

    private void WeekdayListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).StartWeekdayCalendar = WeekdayList.ShortName;
        }
      }
    }

    private void CurrencyListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((DomainModel.Acco)Entity).Currency = CurrencyList.ShortName;
        }
      }
    }


    public AccoRentListViewModel RentList { get; set; }
    public SystemCodeListViewModel WeekdayList { get; set; }
    public CurrencyListViewModel CurrencyList { get; set; }


    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        base.Entity = value;
        if (_entity != null)
        {
          if (((DomainModel.Acco)Entity).BaseRentId.HasValue)
            RentList.ItemId = ((DomainModel.Acco)Entity).BaseRentId.Value;
          WeekdayList.ShortName = ((DomainModel.Acco)Entity).StartWeekdayCalendar; 
          CurrencyList.ShortName = ((DomainModel.Acco)Entity).Currency;

        }
      }
    }

    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    public override BaseScreen<DomainModel.Acco> Start(Entity entity)
    {
      base.Start(entity);
      RentList.Start(((DomainModel.Acco)Entity).AccoId);
      WeekdayList.Start("WEEKDAY");
      CurrencyList.Start(0);

      return this;
    }


    public override BaseScreen<DomainModel.Acco> Start(int entityid)
    {
      base.Start(entityid);
      WeekdayList.Start("WEEKDAY");
      RentList.Start(((DomainModel.Acco)Entity).AccoId);
      CurrencyList.Start(0);
      return this;
    }

    protected override void OnActivate()
    {
      RentList.Start(((DomainModel.Acco)Entity).AccoId);
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        RentList.PropertyChanged -= RentListOnPropertyChanged;
        WeekdayList.PropertyChanged -= WeekdayListOnPropertyChanged;
        CurrencyList.PropertyChanged -= CurrencyListOnPropertyChanged;

      }
    }

  }
}