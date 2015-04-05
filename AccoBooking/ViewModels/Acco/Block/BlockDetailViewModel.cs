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
using System.Linq;
using Cocktail;
using DomainModel;
using DomainServices;
#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class BlockDetailViewModel : BaseDetailViewModel<DomainModel.Booking>
  {
    [ImportingConstructor]
    public BlockDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                          BlockSummaryViewModel rentSummary,
                                          IDialogManager dialogManager)
      : base(unitOfWorkManager, null, rentSummary, dialogManager)
    {
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
      entity.IsBlock = true;
      entity.BookingColor = SessionManager.CurrentAcco.ColorBlock;
      entity.Booker = "Block";
      entity.BookerPhone = "N/A";
      entity.BookerEmail = "block@accobooking.nl";
      var guest = _unitOfWork.BookingGuests.FindInCache(g => g.BookingId == entity.BookingId).FirstOrDefault();
      _unitOfWork.BookingGuests.Delete(guest);

    }
  }

}
