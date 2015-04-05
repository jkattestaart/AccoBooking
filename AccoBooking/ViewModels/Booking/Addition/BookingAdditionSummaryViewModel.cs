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
using AccoBooking.ViewModels.Acco;

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingAdditionSummaryViewModel : BaseScreen<BookingAddition>
  {
    [ImportingConstructor]
    public BookingAdditionSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                           AccoAdditionListViewModel accoAdditionList,
                                           SystemCodeListViewModel unitListViewModel,
                                           IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      AccoAdditionList = accoAdditionList;
      AccoAdditionList.PropertyChanged += AccoAdditionListOnPropertyChanged;

      UnitList = unitListViewModel;
      UnitList.PropertyChanged += UnitListOnPropertyChanged;

    }

    private void AccoAdditionListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // Vullen scherm variabelen vanuit geselecteerde AccoAddition
      if (e.PropertyName == "ItemId")
      {
        if (Entity != null)
        {
          AccoAddition addition;
          // Ophalen betreffende AccoAddition
          // en vul default waarde voor booking addition description, price en unit
          UnitOfWork.AccoAdditions.WithIdFromDataSourceAsync(AccoAdditionList.ItemId).ContinueWith((op) =>
            {
              addition = op.Result;
              ((BookingAddition)Entity).Description = addition.Description;
              ((BookingAddition) Entity).Price = addition.Price;
              ((BookingAddition) Entity).Unit = addition.Unit;
              ((BookingAddition) Entity).DisplaySequence = addition.DisplaySequence;
              ((BookingAddition)Entity).IsPaidFromDeposit = addition.IsPaidFromDeposit;
              ((BookingAddition)Entity).AccoAdditionId = AccoAdditionList.ItemId; 
              UnitList.ShortName = addition.Unit;
            });
        }
      }
      NotifyOfPropertyChange(() => Entity);  // en notify bijgewerkte properties are changed
    }

    private void UnitListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((BookingAddition)Entity).Unit = UnitList.ShortName;
        }
      }
    }

    public AccoAdditionListViewModel AccoAdditionList { get; set; }

    public SystemCodeListViewModel UnitList { get; set; }

    public bool IsReadOnly
    {
      get
      {
        if (Entity != null)
        switch (((BookingAddition)Entity).Booking.Status)
        {
          case "EXPIRED":
            return true;

          case "CANCELLED":
            return true;

          case "CLOSED":
            return true;

          default:
            return false; // RESERVED, CONFIRMED or PAID

        }
        return false;
      }
    }

    public bool AddAddition
    {
      get
      {
        if (Entity != null)
          return Entity.EntityAspect.EntityState.IsAdded();
        return false;

      }
    }


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
          UnitList.ShortName = ((BookingAddition)Entity).Unit;
          AccoAdditionList.Description = ((BookingAddition) Entity).Description;

          NotifyOfPropertyChange(() => IsReadOnly);
          NotifyOfPropertyChange(() => AddAddition);
        }
      }
    }

    
    protected override IRepository<BookingAddition> Repository()
    {
      return UnitOfWork.BookingAdditions;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      
      AccoAdditionList.Start(SessionManager.BookingAccoId);

      UnitList.Start("UNIT");
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        AccoAdditionList.PropertyChanged -= AccoAdditionListOnPropertyChanged;
        UnitList.PropertyChanged -= UnitListOnPropertyChanged;
      }
    }
  
  }
}