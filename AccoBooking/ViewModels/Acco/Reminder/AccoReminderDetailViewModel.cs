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

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoReminderDetailViewModel : BaseDetailViewModel<AccoReminder>
  {
    [ImportingConstructor]
    public AccoReminderDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                       AccoReminderSummaryViewModel reminderSummary,
                                       IDialogManager dialogManager)
      : base(unitOfWorkManager, null, reminderSummary, dialogManager)
    {

    }

    protected override IRepository<AccoReminder> Repository()
    {
      return UnitOfWork.AccoReminders;
    }

    protected override IFactory<AccoReminder> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoReminderFactory;
    }

    protected override void OnCreateEntity(AccoReminder entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);
      entity.AccoId = parentid;

      // todo complementeren ivm errors bij add!
      // id is geen identity
    }

  }

  
}
