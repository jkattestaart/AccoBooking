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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainModel.Projections;

namespace DomainServices.Services
{

  public class SequenceSearchService : ISequenceSearchService
  {
    private readonly IRepository<Sequence> _repository;

    public SequenceSearchService(IRepository<Sequence> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public Task<IEnumerable<SequenceListItem>> FindSequencesAsync(
      string searchText, CancellationToken cancellationToken)
    {
      Expression<Func<Sequence, bool>> filter = null;
      
      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new SequenceListItem
        {
          Id = x.CurrentId,
          Name = x.Name,

          Label = x.Name
        }),

       cancellationToken, filter, null);
    }

    #endregion
  }

  public class AccoSearchService : IAccoSearchService
  {
    private readonly IRepository<Acco> _repository;

    public AccoSearchService(IRepository<Acco> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public Task<IEnumerable<AccoListItem>> FindAccoesAsync(
      string searchText, CancellationToken cancellationToken)
    {
      Expression<Func<Acco, bool>> filter = null;
      if (!string.IsNullOrWhiteSpace(searchText))
        filter = x => x.Description.Contains(searchText);

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoListItem
        {
          Id = x.AccoId,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));
    }

    #endregion
  }

  
  public class AccoReminderSearchService : IAccoReminderSearchService
  {
    private readonly IRepository<AccoReminder> _repository;

    public AccoReminderSearchService(IRepository<AccoReminder> repository)
    {
      _repository = repository; 
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<AccoReminderListItem>> FindAccoRemindersAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoReminder, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var reminders = await  _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoReminderListItem
        {
          Id = x.AccoReminderId,
          Description = x.Description,
          Milestone = x.Milestone,
          Offset = x.Offset,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));

      foreach (var reminder in reminders)
      {
        reminder.Milestone = SystemCodeService.Description(reminder.Milestone, SystemGroupName.Milestone);
        reminder.DisplaySequence = SystemCodeService.DisplaySequence(reminder.Milestone, SystemGroupName.Milestone)
                                     * 100 + reminder.DisplaySequence;

      }

      return reminders;
    }

    #endregion
  }



  public class AccoAdditionSearchService : IAccoAdditionSearchService
  {
    private readonly IRepository<AccoAddition> _repository;

    public AccoAdditionSearchService(IRepository<AccoAddition> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<AccoAdditionListItem>> FindAccoAdditionsAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoAddition, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var additions = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoAdditionListItem
        {
          Id = x.AccoAdditionId,
          Description = x.AccoAdditionDescriptions.FirstOrDefault(y => y.AccoAdditionId == x.AccoAdditionId && y.LanguageId == x.Acco.AccoOwner.LanguageId).Description,     //x.Description,
          Price = x.Price,
          Unit = x.Unit,
          IsDefaultBooked = x.IsDefaultBooked,
          DisplaySequence = x.DisplaySequence,

          Label = x.AccoAdditionDescriptions.FirstOrDefault(y => y.AccoAdditionId == x.AccoAdditionId && y.LanguageId == x.Acco.AccoOwner.LanguageId).Description,
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));

      foreach (var addition in additions)
        addition.Unit = SystemCodeService.Description(addition.Unit, SystemGroupName.Unit);
      
      return additions;

    }

    #endregion
  }


  public class AccoAdditionDescriptionSearchService : IAccoAdditionDescriptionSearchService
  {
    private readonly IRepository<AccoAdditionDescription> _repository;

    public AccoAdditionDescriptionSearchService(IRepository<AccoAdditionDescription> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<AccoAdditionDescriptionListItem>> FindAccoAdditionDescriptionsAsync(
      int accoadditionid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoAdditionDescription, bool>> filter = null;
      filter = x => x.AccoAdditionId == accoadditionid;

      var additions = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoAdditionDescriptionListItem
          {
            Id = x.AccoAdditionDescriptionId,
            Description = x.Description,
            Language=x.Language.Description,

            Label = x.Description
          }),

        cancellationToken, filter, q => q.OrderBy(i => i.Description));

      foreach (var addition in additions)
        addition.Language = SystemCodeService.Description(addition.Language, SystemGroupName.Language);

      return additions;
    }

    #endregion
    
  }


  
  public class AccoCancelConditionSearchService : IAccoCancelConditionSearchService
  {
    private readonly IRepository<AccoCancelCondition> _repository;

    public AccoCancelConditionSearchService(IRepository<AccoCancelCondition> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<AccoCancelConditionListItem>> FindAccoCancelConditionsAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoCancelCondition, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var conditions = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoCancelConditionListItem
        {
          Id = x.AccoCancelConditionId,
          DaysBeforeArrival = x.DaysBeforeArrival,
          RentPercentageToPay = x.RentPercentageToPay,

        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));

      foreach (var condition in conditions)
        condition.Label = condition.DaysBeforeArrival.ToString();

      return conditions;
    }

    #endregion
  }
  
  public class AccoOwnerSearchService : IAccoOwnerSearchService
  {
    private readonly IRepository<AccoOwner> _repository;

    public AccoOwnerSearchService(IRepository<AccoOwner> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public Task<IEnumerable<AccoOwnerListItem>> FindAccoOwnersAsync(
      string searchText, CancellationToken cancellationToken)
    {
      Expression<Func<AccoOwner, bool>> filter = null;
      if (!string.IsNullOrWhiteSpace(searchText))
        filter = x => x.Name.Contains(searchText);

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoOwnerListItem
        {
          Id = x.AccoOwnerId,
          //Description = x.Acco.Description,
          //Location = x.Acco.Location,
          //AccomodationCountry = x.Acco.AccoCountry.Description,
          Owner = x.Name,
          Language = x.Language.Description,
          Email = x.Email,
          Phone = x.Phone,
          Country = x.Country.Description,
          Subscribed = x.Created.HasValue? x.Created.Value: new DateTime(1,1,1),
//          LicenseExpiration = x.Acco.LicenceExpiration.HasValue? x.Acco.LicenceExpiration.Value: new DateTime(1,1,1),
//          IsActive = x.Acco.IsActive,

          Label = x.Name
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));
    }

    #endregion
  }


  
  public class AccoPayPatternSearchService : IAccoPayPatternSearchService
  {
    private readonly IRepository<AccoPayPattern> _repository;

    public AccoPayPatternSearchService(IRepository<AccoPayPattern> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public Task<IEnumerable<AccoPayPatternListItem>> FindAccoPayPatternsAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoPayPattern, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoPayPatternListItem
        {
          Id = x.AccoPayPatternId,
          Description = x.Description,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));
    }

    #endregion
  }


  
  public class AccoPayPatternPaymentSearchService : IAccoPayPatternPaymentSearchService
  {
    private readonly IRepository<AccoPayPatternPayment> _repository;

    public AccoPayPatternPaymentSearchService(IRepository<AccoPayPatternPayment> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<AccoPayPatternPaymentListItem>> FindAccoPayPatternPaymentsAsync(
      int patternid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoPayPatternPayment, bool>> filter = null;
      filter = x => x.AccoPayPatternId == patternid;

      var patterns = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoPayPatternPaymentListItem
        {
          Id = x.AccoPayPatternPaymentId,
          PaymentAmount = x.PaymentAmount,
          PaymentPercentage = x.PaymentPercentage,
          DaysToPayAfterBooking = x.DaysToPayAfterBooking,
          DaysToPayBeforeArrival = x.DaysToPayBeforeArrival,
          //Accommodaton = x.Acco.Description,
          //Description = x.Description,
          //DisplaySequence = x.DisplaySequence.Value

        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));

      foreach (var  pattern in patterns)
        pattern.Label = pattern.PaymentAmount.ToString();

      return patterns;
    }

    #endregion
  }

  public class AccoRentSearchService : IAccoRentSearchService
  {
    private readonly IRepository<AccoRent> _repository;

    public AccoRentSearchService(IRepository<AccoRent> repository)
    {
      _repository = repository;
    }

    #region IAccoRentSearchService Members

    public async Task<IEnumerable<AccoRentListItem>> FindAccoRentsAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoRent, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var rents = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoRentListItem
        {
          Id = x.AccoRentId,
          Description = x.Description,
          RentYear = x.RentYear,
          RentPerNight = x.RentPerNight,
          RentPerWeekend = x.RentPerWeekend,
          RentPerMidweek = x.RentPerMidweek,
          RentPerWeek = x.RentPerWeek,
          IsActive = x.IsActive,
          Label = x.Description
        }),

        cancellationToken, filter, null);

      return rents;
    }

    #endregion

  }

  public class AccoSeasonSearchService : IAccoSeasonSearchService
  {
    private readonly IRepository<AccoSeason> _repository;

    public AccoSeasonSearchService(IRepository<AccoSeason> repository)
    {
      _repository = repository;
    }

    #region IAccoRentSearchService Members

    public async Task<IEnumerable<AccoSeasonListItem>> FindAccoSeasonsAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoSeason, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var rents = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoSeasonListItem
        {
          Id = x.AccoSeasonId,
          Description = x.Description,
          SeasonYear = x.SeasonYear,
          SeasonStart = x.SeasonStart,
          SeasonEnd = x.SeasonEnd,
          RentPerNight = x.AccoRent.RentPerNight,
          RentPerWeekend = x.AccoRent.RentPerWeekend,
          RentPerMidweek = x.AccoRent.RentPerMidweek,
          RentPerWeek = x.AccoRent.RentPerWeek,
          Label = x.Description
        }),

        cancellationToken, filter, null);

      return rents;
    }

    #endregion
  }

  public class AccoSpecialOfferSearchService : IAccoSpecialOfferSearchService
  {
    private readonly IRepository<AccoSpecialOffer> _repository;

    public AccoSpecialOfferSearchService(IRepository<AccoSpecialOffer> repository)
    {
      _repository = repository;
    }

    #region IAccoRentSearchService Members

    public async Task<IEnumerable<AccoSpecialOfferListItem>> FindAccoSpecialOffersAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoSpecialOffer, bool>> filter = null;
      filter = x => x.AccoId == accoid;

      var offers = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoSpecialOfferListItem
        {
          Id = x.AccoSpecialOfferId,
          Description = x.Description,
          SpecialOfferStart = x.SpecialOfferStart,
          SpecialOfferEnd = x.SpecialOfferEnd,
          Language = x.Language.Description,
          Label = x.Description
        }),

        cancellationToken, filter, null);

      foreach (var offer in offers)
        offer.Language = SystemCodeService.Description(offer.Language, SystemGroupName.Language);

      return offers;
    }

    public async Task<IEnumerable<AccoSpecialOfferListItem>> FindAccoSpecialOffersAsync(
      int accoid, int languageid, int take, CancellationToken cancellationToken)
    {
      Expression<Func<AccoSpecialOffer, bool>> filter = null;
      filter = x => x.AccoId == accoid && 
                    x.SpecialOfferStart <= DateTime.Now && 
                    x.SpecialOfferEnd >= DateTime.Now  &&
                    x.LanguageId == languageid;

      var offers = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoSpecialOfferListItem
        {
          Id = x.AccoSpecialOfferId,
          Description = x.Description,
          SpecialOfferStart = x.SpecialOfferStart,
          SpecialOfferEnd = x.SpecialOfferEnd,
          Language = x.Language.Description,
          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(x => x.SpecialOfferStart));

      foreach (var offer in offers)
        offer.Language = SystemCodeService.Description(offer.Language, SystemGroupName.Language);

      return offers.Take(take);
    }

    #endregion

  }

  public class AccoTrusteeSearchService : IAccoTrusteeSearchService
  {
    private readonly IRepository<AccoTrustee> _repository;

    public AccoTrusteeSearchService(IRepository<AccoTrustee> repository)
    {
      _repository = repository;
    }

    #region IAccoRentSearchService Members

    public async Task<IEnumerable<AccoTrusteeListItem>> FindAccoTrusteesAsync(
      int accoownerid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoTrustee, bool>> filter = null;
      filter = x => x.AccoOwnerId == accoownerid;

      var trustees = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoTrusteeListItem
        {
          Id = x.AccoTrusteeId,
          Name = x.Name,
          Login = x.Login,
          Owner = x.AccoOwner.Name,          
          // Language = x.Langugae.Description,
          Label = x.Name
        }),

        cancellationToken, filter, null);

      return trustees;
    }

    #endregion

  }

  public class AccoNotificationSearchService : IAccoNotificationSearchService
  {
    private readonly IRepository<AccoNotification> _repository;

    public AccoNotificationSearchService(IRepository<AccoNotification> repository)
    {
      _repository = repository;
    }

    #region IAccoNotificationSearchService Members

    public async Task<IEnumerable<AccoNotificationListItem>> FindAccoNotificationsAsync(
      int accoownerid, CancellationToken cancellationToken)
    {
      Expression<Func<AccoNotification, bool>> filter = null;
      filter = x => x.AccoOwnerId == accoownerid;

      var notifications = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new AccoNotificationListItem
        {
          Id = x.BookingId.HasValue? x.BookingId.Value : x.AccoId,
          Accommodation = x.Acco.Description,
          Context = x.NoticationType,
          Description = x.Description,
          ExpirationDate = x.ExpirationDate,
          DaysToExpire = x.DaystoExpire,
          Guest = x.Booking.Booker
        }),

        cancellationToken, filter, o => o.OrderBy(n=>n.ExpirationDate));

      return notifications;
    }

    #endregion

  }

}