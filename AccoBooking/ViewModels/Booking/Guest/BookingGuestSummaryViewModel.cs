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

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingGuestSummaryViewModel : BaseScreen<BookingGuest>
  {
    private bool _activityFlows = true;

    [ImportingConstructor]
    public BookingGuestSummaryViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                        SystemCodeListViewModel genderList,
                                        IDialogManager dialogManager)
      : base(unitOfWorkManager, dialogManager)
    {
      GenderList = genderList;
      GenderList.PropertyChanged += GenderListOnPropertyChanged;
    }


    private void GenderListOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
        if (Entity != null)
        {
          ((BookingGuest)Entity).Gender = GenderList.ShortName;
        }
      }
    }

    public SystemCodeListViewModel GenderList { get; set; }

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

          GenderList.ShortName = ((BookingGuest)Entity).Gender;
        }
      }
    }


    // Done kan alleen true worden bij wijzigen activity en IsDue = true
    // in alle andere gevallen checkbox niet laten zien
    // 
    protected override IRepository<BookingGuest> Repository()
    {
      return UnitOfWork.BookingGuests;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      GenderList.Start("GENDER");
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (close)
      {
        GenderList.PropertyChanged -= GenderListOnPropertyChanged;
      }
    }

  }
}