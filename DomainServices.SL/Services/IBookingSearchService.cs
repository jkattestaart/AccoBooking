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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DomainModel.Projections;
using IdeaBlade.Core;

namespace DomainServices.Services
{
  public interface IBookingSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingListItem>> FindBookingsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<BookingListItem>> FindBookingsAsync(
      string guestName, bool includeClosed, bool includeExpired, DateTime from, DateTime to,
      CancellationToken cancellationToken);

    Task<IEnumerable<BlockListItem>> FindBlocksAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IBookingGuestSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingGuestListItem>> FindBookingGuestsAsync(
     int bookingid, CancellationToken cancellationToken);
  }


  public interface IBookingReminderSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingReminderListItem>> FindBookingRemindersAsync(
      int bookingid, CancellationToken cancellationToken);
  }


  public interface IBookingAdditionSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingAdditionListItem>> FindBookingAdditionsAsync(
      int bookingid, CancellationToken cancellationToken);
  }


  public interface IBookingCancelConditionSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingCancelConditionListItem>> FindBookingCancelConditionsAsync(
      int bookingid, CancellationToken cancellationToken);
  }

  public interface IBookingPaymentSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingPaymentListItem>> FindBookingPaymentsAsync(
      int bookingid, CancellationToken cancellationToken);
  }

  public interface IBookingPaymentToDoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int bookingid, CancellationToken cancellationToken);
  }

  public interface IBookingReminderToDoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int bookingid, CancellationToken cancellationToken);
  }

  public interface IBookingExpiredToDoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int bookingid, CancellationToken cancellationToken);
  }

  public interface IPayPatternToDoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int bookingid, CancellationToken cancellationToken);
  }
  
  public interface ILicenseExpiredToDoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<BookingToDoListItem>> FindToDoesAsync(
      int bookingid, CancellationToken cancellationToken);
  }

}