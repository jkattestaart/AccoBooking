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

namespace AccoBooking.ViewModels.Booking
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class CreateBookingDetailViewModel : BaseDetailViewModel<DomainModel.Booking>
  {

    [ImportingConstructor]
    public CreateBookingDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                  BookingSummaryViewModel bookingSummary,
                                  IDialogManager dialogManager)
      : base(unitOfWorkManager, null, bookingSummary, dialogManager)
    {
      bookingSummary.Parent = this;
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

  }

}
