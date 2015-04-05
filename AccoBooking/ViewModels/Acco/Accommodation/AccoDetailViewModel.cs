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
using Cocktail;
#if HARNESS
using DomainServices.SampleData;
#endif
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoDetailViewModel : BaseDetailViewModel<DomainModel.Acco>
  {
    private ToolbarViewModel _copyToolbar;
    [ImportingConstructor]
    public AccoDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                    [ImportMany] IEnumerable<IBaseDetailSection<DomainModel.Acco>> sections,
                                    //AccoSummaryViewModel accoSummary,
                                    IDialogManager dialogManager)
      : base(unitOfWorkManager, sections, null, dialogManager)
    {

    }

    protected override IRepository<DomainModel.Acco> Repository()
    {
      return UnitOfWork.Accoes;
    }

    protected override IFactory<DomainModel.Acco> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoFactory;
    }

    //Toolbar needed beetje gekunseld, IsVisible werkt niet altijd goed SL4 probleem?
    protected override void ChangeActiveItem(Caliburn.Micro.IScreen newItem, bool closePrevious)
    {
      var toobarVisible = (newItem.GetType() == typeof (AccommodationDetailSectionViewModel))
                          || (newItem.GetType() == typeof (AccoOwnerMainDetailSectionViewModel))
                          || (newItem.GetType() == typeof(BookingDetailSectionViewModel));

      if (Parent == null)
        return;
      if (Parent.GetType() == typeof(AccoManagementViewModel))
      {
        if (!toobarVisible)
        {
          if ((Parent as AccoManagementViewModel).BottomToolbar != null)
          {
            (Parent as AccoManagementViewModel).BottomToolbar.IsVisible = false;
            _copyToolbar = (Parent as AccoManagementViewModel).BottomToolbar;
            (Parent as AccoManagementViewModel).BottomToolbar = null;
          }
        }
        else
        {
          if ((Parent as AccoManagementViewModel).BottomToolbar == null)
            (Parent as AccoManagementViewModel).BottomToolbar = _copyToolbar;
          if ((Parent as AccoManagementViewModel).BottomToolbar != null)
            (Parent as AccoManagementViewModel).BottomToolbar.IsVisible = true;
        }
      }
      
      
      base.ChangeActiveItem(newItem, closePrevious);
    }
  
  }

}
