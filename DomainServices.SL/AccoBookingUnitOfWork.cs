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
using DomainServices.Factories;
using DomainServices.Repositories;
using DomainServices.Services;

namespace DomainServices
{
  [Export(typeof(IAccoBookingUnitOfWork)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoBookingUnitOfWork : UnitOfWork, IAccoBookingUnitOfWork
  {
    [ImportingConstructor]
    public AccoBookingUnitOfWork(
        [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<AccoBookingEntities>
            entityManagerProvider,
        [Import(AllowDefault = true)] IGlobalCache globalCache = null)
      : base(entityManagerProvider)
    {
      SystemGroupFactory = new SystemGroupFactory(entityManagerProvider);
      SystemGroups = new SystemGroupRepository(entityManagerProvider);
      SystemGroupSearchService = new SystemGroupSearchService(SystemGroups);

      SystemCodeFactory = new SystemCodeFactory(entityManagerProvider);
      SystemCodes = new SystemCodeRepository(entityManagerProvider);
      SystemCodeSearchService = new SystemCodeSearchService(SystemCodes);

      SequenceFactory = new SequenceFactory(entityManagerProvider);
      Sequences = new SequenceRepository(entityManagerProvider);
      SequenceSearchService = new SequenceSearchService(Sequences);

      AccoFactory = new AccoFactory(entityManagerProvider);
      Accoes = new AccoRepository(entityManagerProvider);
      AccoSearchService = new AccoSearchService(Accoes);

      AccoReminderFactory = new AccoReminderFactory(entityManagerProvider);
      AccoReminders = new AccoReminderRepository(entityManagerProvider);
      AccoReminderSearchService = new AccoReminderSearchService(AccoReminders);

      AccoAdditionFactory = new AccoAdditionFactory(entityManagerProvider);
      AccoAdditions = new AccoAdditionRepository(entityManagerProvider);
      AccoAdditionSearchService = new AccoAdditionSearchService(AccoAdditions);

      AccoAdditionDescriptionFactory = new AccoAdditionDescriptionFactory(entityManagerProvider);
      AccoAdditionDescriptions = new AccoAdditionDescriptionRepository(entityManagerProvider);
      AccoAdditionDescriptionSearchService = new AccoAdditionDescriptionSearchService(AccoAdditionDescriptions);

      AccoCancelConditionFactory = new AccoCancelConditionFactory(entityManagerProvider);
      AccoCancelConditions = new AccoCancelConditionRepository(entityManagerProvider);
      AccoCancelConditionSearchService = new AccoCancelConditionSearchService(AccoCancelConditions);

      CountryFactory = new CountryFactory(entityManagerProvider);
      Countries = new CountryRepository(entityManagerProvider);
      CountrySearchService = new CountrySearchService(Countries);

      CurrencyFactory = new CurrencyFactory(entityManagerProvider);
      Currencies = new CurrencyRepository(entityManagerProvider);
      CurrencySearchService = new CurrencySearchService(Currencies);

      LanguageFactory = new LanguageFactory(entityManagerProvider);
      Languages = new LanguageRepository(entityManagerProvider);
      LanguageSearchService = new LanguageSearchService(Languages);

      MailTemplateFactory = new MailTemplateFactory(entityManagerProvider);
      MailTemplates = new MailTemplateRepository(entityManagerProvider);
      MailTemplateSearchService = new MailTemplateSearchService(MailTemplates);

      AccoOwnerFactory = new AccoOwnerFactory(entityManagerProvider);
      AccoOwners = new AccoOwnerRepository(entityManagerProvider);
      AccoOwnerSearchService = new AccoOwnerSearchService(AccoOwners);

      AccoPayPatternFactory = new AccoPayPatternFactory(entityManagerProvider);
      AccoPayPatterns = new AccoPayPatternRepository(entityManagerProvider);
      AccoPayPatternSearchService = new AccoPayPatternSearchService(AccoPayPatterns);

      AccoPayPatternPaymentFactory = new AccoPayPatternPaymentFactory(entityManagerProvider);
      AccoPayPatternPayments = new AccoPayPatternPaymentRepository(entityManagerProvider);
      AccoPayPatternPaymentSearchService = new AccoPayPatternPaymentSearchService(AccoPayPatternPayments);

      AccoRentFactory = new AccoRentFactory(entityManagerProvider);
      AccoRents = new AccoRentRepository(entityManagerProvider);
      AccoRentSearchService = new AccoRentSearchService(AccoRents);

      AccoSeasonFactory = new AccoSeasonFactory(entityManagerProvider);
      AccoSeasons = new AccoSeasonRepository(entityManagerProvider);
      AccoSeasonSearchService = new AccoSeasonSearchService(AccoSeasons);

      AccoTrusteeFactory = new AccoTrusteeFactory(entityManagerProvider);
      AccoTrustees = new AccoTrusteeRepository(entityManagerProvider);
      AccoTrusteeSearchService = new AccoTrusteeSearchService(AccoTrustees);

      AccoSpecialOfferFactory = new AccoSpecialOfferFactory(entityManagerProvider);
      AccoSpecialOffers = new AccoSpecialOfferRepository(entityManagerProvider);
      AccoSpecialOfferSearchService = new AccoSpecialOfferSearchService(AccoSpecialOffers);

      AccoNotificationFactory = new AccoNotificationFactory(entityManagerProvider);
      AccoNotifications = new AccoNotificationRepository(entityManagerProvider);
      AccoNotificationSearchService = new AccoNotificationSearchService(AccoNotifications);

      BookingFactory = new BookingFactory(entityManagerProvider);
      Bookings = new BookingRepository(entityManagerProvider);
      BookingSearchService = new BookingSearchService(Bookings);

      BookingGuestFactory = new BookingGuestFactory(entityManagerProvider);
      BookingGuests = new BookingGuestRepository(entityManagerProvider);
      BookingGuestSearchService = new BookingGuestSearchService(BookingGuests);

      BookingAdditionFactory = new BookingAdditionFactory(entityManagerProvider);
      BookingAdditions = new BookingAdditionRepository(entityManagerProvider);
      BookingAdditionSearchService = new BookingAdditionSearchService(BookingAdditions);

      BookingReminderFactory = new BookingReminderFactory(entityManagerProvider);
      BookingReminders = new BookingReminderRepository(entityManagerProvider);
      BookingReminderSearchService = new BookingReminderSearchService(BookingReminders);

      BookingPaymentFactory = new BookingPaymentFactory(entityManagerProvider);
      BookingPayments = new BookingPaymentRepository(entityManagerProvider);
      BookingPaymentSearchService = new BookingPaymentSearchService(BookingPayments);

      BookingCancelConditionFactory = new BookingCancelConditionFactory(entityManagerProvider);
      BookingCancelConditions = new BookingCancelConditionRepository(entityManagerProvider);
      BookingCancelConditionSearchService = new BookingCancelConditionSearchService(BookingCancelConditions);

      BookingPaymentToDoSearchService = new BookingPaymentToDoSearchService(BookingPayments);
      BookingReminderToDoSearchService = new BookingReminderToDoSearchService(BookingReminders);
      //BookingExpiredToDoSearchService = new BookingExpiredToDoSearchService(Bookings);
      PayPatternToDoSearchService = new PayPatternToDoSearchService(Bookings);
      LicenseExpiredToDoSearchService = new LicenseExpiredToDoSearchService(Accoes);

      // @@@ JKT Named queries
      ExpiredBookings = new ExpiredBookingRepository(entityManagerProvider);
      BookingExpiredToDoSearchService = new BookingExpiredToDoSearchService(ExpiredBookings);
    }

    #region IAccoBookingUnitOfWork Members

    public IFactory<SystemGroup> SystemGroupFactory { get; private set; }
    public IFactory<SystemCode> SystemCodeFactory { get; private set; }
    public IFactory<Sequence> SequenceFactory { get; private set; }
    public IFactory<Acco> AccoFactory { get; private set; }
    public IFactory<AccoReminder> AccoReminderFactory { get; private set; }
    public IFactory<AccoAddition> AccoAdditionFactory { get; private set; }
    public IFactory<AccoAdditionDescription> AccoAdditionDescriptionFactory { get; private set; }
    public IFactory<AccoCancelCondition> AccoCancelConditionFactory { get; private set; }
    public IFactory<Country> CountryFactory { get; private set; }
    public IFactory<Currency> CurrencyFactory { get; private set; }
    public IFactory<Language> LanguageFactory { get; private set; }
    public IFactory<MailTemplate> MailTemplateFactory { get; private set; }
    public IFactory<AccoOwner> AccoOwnerFactory { get; private set; }
    public IFactory<AccoPayPattern> AccoPayPatternFactory { get; private set; }
    public IFactory<AccoPayPatternPayment> AccoPayPatternPaymentFactory { get; private set; }
    public IFactory<AccoRent> AccoRentFactory { get; private set; }
    public IFactory<AccoSeason> AccoSeasonFactory { get; private set; }
    public IFactory<AccoTrustee> AccoTrusteeFactory { get; private set; }
    public IFactory<AccoSpecialOffer> AccoSpecialOfferFactory { get; private set; }
    public IFactory<AccoNotification> AccoNotificationFactory { get; private set; }
    public IFactory<Booking> BookingFactory { get; private set; }
    public IFactory<BookingGuest> BookingGuestFactory { get; private set; }
    public IFactory<BookingAddition> BookingAdditionFactory { get; private set; }
    public IFactory<BookingReminder> BookingReminderFactory { get; private set; }
    public IFactory<BookingPayment> BookingPaymentFactory { get; private set; }
    public IFactory<BookingCancelCondition> BookingCancelConditionFactory { get; private set; }

    public IRepository<Acco> Accoes { get; private set; }
    public IRepository<AccoReminder> AccoReminders { get; private set; }
    public IRepository<AccoAddition> AccoAdditions { get; private set; }
    public IRepository<AccoAdditionDescription> AccoAdditionDescriptions { get; private set; }
    public IRepository<AccoCancelCondition> AccoCancelConditions { get; private set; }
    public IRepository<Country> Countries { get; private set; }
    public IRepository<Currency> Currencies { get; private set; }
    public IRepository<Language> Languages { get; private set; }
    public IRepository<MailTemplate> MailTemplates { get; private set; }
    public IRepository<AccoOwner> AccoOwners { get; private set; }
    public IRepository<AccoPayPattern> AccoPayPatterns { get; private set; }
    public IRepository<AccoPayPatternPayment> AccoPayPatternPayments { get; private set; }
    public IRepository<AccoRent> AccoRents { get; private set; }
    public IRepository<AccoSeason> AccoSeasons { get; private set; }
    public IRepository<AccoTrustee> AccoTrustees { get; private set; }
    public IRepository<AccoSpecialOffer> AccoSpecialOffers { get; private set; }
    public IRepository<AccoNotification> AccoNotifications { get; private set; }
    public IRepository<Booking> Bookings { get; private set; }
    public IRepository<BookingGuest> BookingGuests { get; private set; }
    public IRepository<BookingAddition> BookingAdditions { get; private set; }
    public IRepository<BookingReminder> BookingReminders { get; private set; }
    public IRepository<BookingPayment> BookingPayments { get; private set; }
    public IRepository<BookingCancelCondition> BookingCancelConditions { get; private set; }
    public IRepository<BookingReminder> BookingToDoes { get; private set; }
    public IRepository<SystemGroup> SystemGroups { get; private set; }
    public IRepository<SystemCode> SystemCodes { get; private set; }
    public IRepository<Sequence> Sequences { get; private set; }

    public ISystemGroupSearchService SystemGroupSearchService { get; private set; }
    public ISystemCodeSearchService SystemCodeSearchService { get; private set; }
    public ISequenceSearchService SequenceSearchService { get; private set; }
    public IAccoSearchService AccoSearchService { get; private set; }
    public IAccoReminderSearchService AccoReminderSearchService { get; private set; }
    public IAccoAdditionSearchService AccoAdditionSearchService { get; private set; }
    public IAccoAdditionDescriptionSearchService AccoAdditionDescriptionSearchService { get; private set; }
    public IAccoCancelConditionSearchService AccoCancelConditionSearchService { get; private set; }
    public ICountrySearchService CountrySearchService { get; private set; }
    public ICurrencySearchService CurrencySearchService { get; private set; }
    public ILanguageSearchService LanguageSearchService { get; private set; }
    public IMailTemplateSearchService MailTemplateSearchService { get; private set; }
    public IAccoOwnerSearchService AccoOwnerSearchService { get; private set; }
    public IAccoPayPatternSearchService AccoPayPatternSearchService { get; private set; }
    public IAccoPayPatternPaymentSearchService AccoPayPatternPaymentSearchService { get; private set; }
    public IAccoRentSearchService AccoRentSearchService { get; private set; }
    public IAccoSeasonSearchService AccoSeasonSearchService { get; private set; }
    public IAccoTrusteeSearchService AccoTrusteeSearchService { get; private set; }
    public IAccoSpecialOfferSearchService AccoSpecialOfferSearchService { get; private set; }
    public IAccoNotificationSearchService AccoNotificationSearchService { get; private set; }
    public IBookingSearchService BookingSearchService { get; private set; }
    public IBookingGuestSearchService BookingGuestSearchService { get; private set; }
    public IBookingAdditionSearchService BookingAdditionSearchService { get; private set; }
    public IBookingReminderSearchService BookingReminderSearchService { get; private set; }
    public IBookingPaymentSearchService BookingPaymentSearchService { get; private set; }
    public IBookingCancelConditionSearchService BookingCancelConditionSearchService { get; private set; }
    public IBookingPaymentToDoSearchService BookingPaymentToDoSearchService { get; private set; }
    public IBookingReminderToDoSearchService BookingReminderToDoSearchService { get; private set; }
    public IBookingExpiredToDoSearchService BookingExpiredToDoSearchService { get; private set; }
    public IPayPatternToDoSearchService PayPatternToDoSearchService { get; private set; }
    public ILicenseExpiredToDoSearchService LicenseExpiredToDoSearchService { get; private set; }

    // @@@ JKT Named query
    public IRepository<Booking> ExpiredBookings { get; private set; }


    #endregion
  }
}