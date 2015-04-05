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

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailTemplateDetailViewModel : BaseDetailViewModel<MailTemplate>
  {
    [ImportingConstructor]
    public MailTemplateDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                       //MailTemplateSummaryViewModel mailTemplateSummary,
                                       [ImportMany] IEnumerable<IBaseDetailSection<MailTemplate>> sections,
                                       IDialogManager dialogManager)
      : base(unitOfWorkManager, sections, null, dialogManager)
    {

    }

    protected override IRepository<MailTemplate> Repository()
    {
      return UnitOfWork.MailTemplates;
    }

    protected override IFactory<MailTemplate> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.MailTemplateFactory;
    }


    //Toolbar needed
    protected override void ChangeActiveItem(Caliburn.Micro.IScreen newItem, bool closePrevious)
    {
      var toobarVisible = (newItem.GetType() == typeof(MailTemplateSectionViewModel));
      (Parent as MailTemplateManagementViewModel).BottomToolbar.IsVisible = toobarVisible;

      base.ChangeActiveItem(newItem, closePrevious);
    }
  
  }

}
