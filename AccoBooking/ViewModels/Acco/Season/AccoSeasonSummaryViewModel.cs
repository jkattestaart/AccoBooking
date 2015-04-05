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

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Cocktail;
using DomainModel;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoSeasonSummaryViewModel : BaseScreen<AccoSeason>
  {
    [ImportingConstructor]
    public AccoSeasonSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      AccoRentListViewModel rentList,
      IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      RentList = rentList;
      RentList.PropertyChanged += RentListPropertyChanged;
    }



    private void RentListPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
        if (Entity != null)
        {
          ((AccoSeason) Entity).AccoRentId = RentList.ItemId;
        }
    }

  

    public AccoRentListViewModel RentList { get; set; }

    public override Entity Entity
    {
      get
      {
        return base.Entity;
      }
      set
      {
        _entity = value;
        if (_entity != null)
          RentList.ItemId = ((AccoSeason) Entity).AccoRentId;

        NotifyOfPropertyChange(() => Entity);
        
      }
    }

    protected override IRepository<AccoSeason> Repository()
    {
      return UnitOfWork.AccoSeasons;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      RentList.Start(SessionManager.CurrentAcco.AccoId);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        RentList.PropertyChanged -= RentListPropertyChanged;
      }
    }

    public void MouseLeave(MouseEventArgs e)
    {
        CopySeasonStartToEnd();
    }

    public void KeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key==Key.Tab)
        CopySeasonStartToEnd();
    }

    public void SelectedDateChanged(EventArgs e)
    {
        CopySeasonStartToEnd();
    }

    private void CopySeasonStartToEnd()
    {
      if (Entity == null)
        return;
      if (((AccoSeason) Entity).SeasonEnd.Date < ((AccoSeason) Entity).SeasonStart.Date ||
          ((AccoSeason) Entity).SeasonEnd.Date == new DateTime(1, 1, 1))
      {
        ((AccoSeason) Entity).SeasonEnd = ((AccoSeason) Entity).SeasonStart.Date;
        NotifyOfPropertyChange(() => Entity);
      }

    }
  }
}