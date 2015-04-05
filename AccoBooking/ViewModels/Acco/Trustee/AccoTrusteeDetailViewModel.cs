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
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoTrusteeDetailViewModel : BaseDetailViewModel<AccoTrustee>
  {
    [ImportingConstructor]
    public AccoTrusteeDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                              AccoTrusteeSummaryViewModel cancelConditionSummary,
                                              IDialogManager dialogManager)
      : base(unitOfWorkManager, null, cancelConditionSummary, dialogManager)
    {

    }

    protected override IRepository<AccoTrustee> Repository()
    {
      return UnitOfWork.AccoTrustees;
    }

    protected override IFactory<AccoTrustee> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoTrusteeFactory;
    }

    protected override void OnCreateEntity(AccoTrustee entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);

      entity.AccoOwnerId = SessionManager.CurrentAcco.AccoOwnerId; //parentid;
    }


  
  }


}
