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

using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.Booking
{
  [Export]
  public class BookingAdditionManagementViewModel : BaseMasterViewModel<BookingAdditionSearchViewModel,BookingAdditionDetailViewModel, BookingAdditionListItem, BookingAddition>
                                        
  {
   
    [ImportingConstructor]
    public BookingAdditionManagementViewModel(ExportFactory<BookingAdditionSearchViewModel> searchFactory,
                                              ExportFactory<BookingAdditionDetailViewModel> detailFactory,
                                              IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                              IDialogManager dialogManager,
                                              ToolbarViewModel toolbar,
                                              ToolbarViewModel bottomToolbar
                                              )
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
    
    }

    protected override IRepository<BookingAddition> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.BookingAdditions;
    }

    public override async Task OnPostSave(bool additionDeleted)
    {
      IAccoBookingUnitOfWork unitOfWork;
      DomainModel.Booking booking = null;

      //Bij delete van addition is er geen current unitofwork, 
      //Bij gewoon save kan de active unitofwork genomen worden
      if (additionDeleted)
      {
        unitOfWork = _unitOfWorkManager.Get(_parentid);
        booking = await unitOfWork.Bookings.WithIdFromDataSourceAsync(_parentid);
      }
      else
      {
        unitOfWork = ActiveUnitOfWork;
        booking = ((BookingAddition) ActiveDetail.Entity).Booking;
      }

      var additions =
        await unitOfWork.BookingAdditions.FindInDataSourceAsync(ad => ad.BookingId == _parentid, CancellationToken.None);

      decimal result = 0;
      foreach (var addition in additions)
        result = result + addition.Amount;

      booking.Additions = result;

      // Zorg ervoor
      try
      {
        using (Busy.GetTicket())
        {
          await unitOfWork.CommitAsync();
          await UpdateBookingService.ExecuteAsync(booking.BookingId);
        }
      }
      catch (Exception)
      {

        throw;
      }
    }

    public async override Task OnPostDelete()
    {
      try
      {
        using (Busy.GetTicket())
        {
          await UpdateBookingService.ExecuteAsync(_parentid);
        }
      }
      catch (Exception)
      {

        throw;
      }
      await base.OnPostDelete();
    }
  }
}
