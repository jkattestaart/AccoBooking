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
using DomainModel;
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoPayPatternDetailViewModel : BaseDetailViewModel<AccoPayPattern>
  {
    [ImportingConstructor]
    public AccoPayPatternDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                         [ImportMany] IEnumerable<IBaseDetailSection<AccoPayPattern>> sections, 
                                         IDialogManager dialogManager)
      : base(unitOfWorkManager, sections, null, dialogManager)
    {

    }

    protected override IRepository<AccoPayPattern> Repository()
    {
      return UnitOfWork.AccoPayPatterns;
    }

    protected override IFactory<AccoPayPattern> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoPayPatternFactory;
    }

    protected override void OnCreateEntity(AccoPayPattern entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);
      entity.AccoId = parentid;
    }

    //Toolbar needed
    protected override void ChangeActiveItem(Caliburn.Micro.IScreen newItem, bool closePrevious)
    {
      var toobarVisible = (newItem.GetType() == typeof(AccoPayPatternSectionViewModel));
      (Parent as AccoPayPatternManagementViewModel).BottomToolbar.IsVisible = toobarVisible;

      base.ChangeActiveItem(newItem, closePrevious);
    }

  
  }

  
}
