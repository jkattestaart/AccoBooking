// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Cocktail;
using DomainModel;
using DomainServices;
#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingDetailViewModel : BaseDetailViewModel<DomainModel.Booking>
  {

    [ImportingConstructor]
    public BookingDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                  [ImportMany] IEnumerable<IBaseDetailSection<DomainModel.Booking>> sections,
                                  //BookingSummaryViewModel bookingSummary,
                                  IDialogManager dialogManager)
      : base(unitOfWorkManager, sections, null, dialogManager)
    {
      var newSections = _sections.ToList();
      for (int i = 0; i < newSections.Count(); i++)
      {
        if (newSections[i].GetType() == typeof(BookingTrusteeMainDetailSectionViewModel))
        {
          newSections.RemoveAt(i);
        }
      }
      _sections = newSections;
    }

    protected override IRepository<DomainModel.Booking> Repository()
    {
      return UnitOfWork.Bookings;
    }

    protected override IFactory<DomainModel.Booking> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.BookingFactory;
    }

    protected override void OnCreateEntity(DomainModel.Booking entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);
      entity.AccoId = SessionManager.CurrentAcco.AccoId;
    }

    //Toolbar needed beetje gekunseld, IsVisible werkt niet altijd goed SL4 probleem?
    protected override void ChangeActiveItem(Caliburn.Micro.IScreen newItem, bool closePrevious)
    {
      var toobarVisible = (newItem.GetType() == typeof (BookingMainDetailSectionViewModel))
                          || (newItem.GetType() == typeof (PaymentManagementSectionViewModel));

     // @@@
      if (Parent == null)
        return;
      if (Parent.GetType() == typeof(BookingManagementViewModel))
      {
        if (!toobarVisible)
        {
          if ((Parent as BookingManagementViewModel).BottomToolbar != null)
          {
            (Parent as BookingManagementViewModel).BottomToolbar.IsVisible = false;
            (Parent as BookingManagementViewModel).CopyToolbarViewModel = (Parent as BookingManagementViewModel).BottomToolbar;
            (Parent as BookingManagementViewModel).BottomToolbar = null;
          }
        }
        else
        {
          if ((Parent as BookingManagementViewModel).BottomToolbar == null)
            (Parent as BookingManagementViewModel).BottomToolbar = (Parent as BookingManagementViewModel).CopyToolbarViewModel;
          if ((Parent as BookingManagementViewModel).BottomToolbar != null)
            (Parent as BookingManagementViewModel).BottomToolbar.IsVisible = true;
        }
      }



      //else if (Parent.GetType() == typeof(CreateBookingDialogViewModel))
      //    (Parent as CreateBookingDialogViewModel).BottomToolbar.IsVisible = toobarVisible;
      ////else if (Parent.GetType() == typeof(UpdateBookingDialogViewModel))
      //  (Parent as UpdateBookingDialogViewModel).BottomToolbar.IsVisible = toobarVisible;
          
      base.ChangeActiveItem(newItem, closePrevious);
    }

  }


}
