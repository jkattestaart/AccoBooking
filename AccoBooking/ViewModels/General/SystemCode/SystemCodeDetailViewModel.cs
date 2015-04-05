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

using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
#if HARNESS
using DomainServices.SampleData;
#endif
using DomainServices;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SystemCodeDetailViewModel : BaseDetailViewModel<SystemCode>
  {
    [ImportingConstructor]
    public SystemCodeDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                     SystemCodeSummaryViewModel systemCodeSummary,
                                     IDialogManager dialogManager)
      : base(unitOfWorkManager, null, systemCodeSummary, dialogManager)
    {
    }

    protected override IFactory<SystemCode> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.SystemCodeFactory;
    }

    protected override IRepository<SystemCode> Repository()
    {
      return UnitOfWork.SystemCodes;
    }

    protected override void OnCreateEntity(SystemCode systemcode, int parentid)
    {
      systemcode.GroupId = parentid;
      systemcode.Code = "";
      systemcode.Description = "";
      systemcode.IsDefault = false;
    }

  }

 

}
