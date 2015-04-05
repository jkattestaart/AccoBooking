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
using System.Threading;
using System.Threading.Tasks;
using DomainModel.Projections;
using IdeaBlade.Core;

namespace DomainServices.Services
{
  public interface ISequenceSearchService : IHideObjectMembers
  {
    Task<IEnumerable<SequenceListItem>> FindSequencesAsync(
      string searchText, CancellationToken cancellationToken);
  }


  public interface IAccoSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoListItem>> FindAccoesAsync(
      string searchText, CancellationToken cancellationToken);
  }


  public interface IAccoReminderSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoReminderListItem>> FindAccoRemindersAsync(
      int accoid, CancellationToken cancellationToken);
  }


  public interface IAccoAdditionSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoAdditionListItem>> FindAccoAdditionsAsync(
      int accoid, CancellationToken cancellationToken);
  }


  public interface IAccoAdditionDescriptionSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoAdditionDescriptionListItem>> FindAccoAdditionDescriptionsAsync(
      int accoadditionid, CancellationToken cancellationToken);
  }


  public interface IAccoCancelConditionSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoCancelConditionListItem>> FindAccoCancelConditionsAsync(
      int accoid, CancellationToken cancellationToken);
  }


  public interface IAccoOwnerSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoOwnerListItem>> FindAccoOwnersAsync(
      string searchText, CancellationToken cancellationToken);
  }


  public interface IAccoPayPatternSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoPayPatternListItem>> FindAccoPayPatternsAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IAccoPayPatternPaymentSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoPayPatternPaymentListItem>> FindAccoPayPatternPaymentsAsync(
      int patternid, CancellationToken cancellationToken);
  }

  public interface IAccoRentSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoRentListItem>> FindAccoRentsAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IAccoSeasonSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoSeasonListItem>> FindAccoSeasonsAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IAccoSpecialOfferSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoSpecialOfferListItem>> FindAccoSpecialOffersAsync(
      int accoid, CancellationToken cancellationToken);

    Task<IEnumerable<AccoSpecialOfferListItem>> FindAccoSpecialOffersAsync(
      int accoid, int languageid, int take, CancellationToken cancellationToken);
  }

  public interface IAccoTrusteeSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoTrusteeListItem>> FindAccoTrusteesAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IAccoNotificationSearchService : IHideObjectMembers
  {
    Task<IEnumerable<AccoNotificationListItem>> FindAccoNotificationsAsync(
      int accoid, CancellationToken cancellationToken);
  }

}