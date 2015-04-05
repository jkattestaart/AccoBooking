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

using Cocktail;
using DomainModel;
using DomainServices.Services;

namespace DomainServices
{
  public interface IAccoBookingUnitOfWork : IUnitOfWork
  {
    // Factories
    IFactory<SystemGroup> SystemGroupFactory { get; }
    IFactory<SystemCode> SystemCodeFactory { get; }
    IFactory<Sequence> SequenceFactory { get; }
    IFactory<Acco> AccoFactory { get; }
    IFactory<AccoReminder> AccoReminderFactory { get; }
    IFactory<AccoAddition> AccoAdditionFactory { get; }
    IFactory<AccoAdditionDescription> AccoAdditionDescriptionFactory { get; }
    IFactory<AccoCancelCondition> AccoCancelConditionFactory { get; }
    IFactory<Country> CountryFactory { get; }
    IFactory<Currency> CurrencyFactory { get; }
    IFactory<Language> LanguageFactory { get; }
    IFactory<MailTemplate> MailTemplateFactory { get; }
    IFactory<AccoOwner> AccoOwnerFactory { get; }
    IFactory<AccoPayPattern> AccoPayPatternFactory { get; }
    IFactory<AccoPayPatternPayment> AccoPayPatternPaymentFactory { get; }
    IFactory<AccoRent> AccoRentFactory { get; }
    IFactory<AccoSeason> AccoSeasonFactory { get; }
    IFactory<AccoTrustee> AccoTrusteeFactory { get; }
    IFactory<AccoSpecialOffer> AccoSpecialOfferFactory { get; }
    IFactory<AccoNotification> AccoNotificationFactory { get; }
    IFactory<Booking> BookingFactory { get; }
    IFactory<BookingGuest> BookingGuestFactory { get; }
    IFactory<BookingAddition> BookingAdditionFactory { get; }
    IFactory<BookingReminder> BookingReminderFactory { get; }
    IFactory<BookingPayment> BookingPaymentFactory { get; }
    IFactory<BookingCancelCondition> BookingCancelConditionFactory { get; }



    // Repositories
    IRepository<SystemGroup> SystemGroups { get; }
    IRepository<SystemCode> SystemCodes { get; }
    IRepository<Sequence> Sequences { get; }
    IRepository<Acco> Accoes { get; }
    IRepository<AccoReminder> AccoReminders { get; }
    IRepository<AccoAddition> AccoAdditions { get; }
    IRepository<AccoAdditionDescription> AccoAdditionDescriptions { get; }
    IRepository<AccoCancelCondition> AccoCancelConditions { get; }
    IRepository<Country> Countries { get; }
    IRepository<Currency> Currencies { get; }
    IRepository<Language> Languages { get; }
    IRepository<MailTemplate> MailTemplates { get; }
    IRepository<AccoOwner> AccoOwners { get; }
    IRepository<AccoPayPattern> AccoPayPatterns { get; }
    IRepository<AccoPayPatternPayment> AccoPayPatternPayments { get; }
    IRepository<AccoRent> AccoRents { get; }
    IRepository<AccoSeason> AccoSeasons { get; }
    IRepository<AccoTrustee> AccoTrustees { get; }
    IRepository<AccoSpecialOffer> AccoSpecialOffers { get; }
    IRepository<AccoNotification> AccoNotifications { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<BookingGuest> BookingGuests { get; }
    IRepository<BookingAddition> BookingAdditions { get; }
    IRepository<BookingReminder> BookingReminders { get; }
    IRepository<BookingPayment> BookingPayments { get; }
    IRepository<BookingCancelCondition> BookingCancelConditions { get; }

    // Services
    ISystemGroupSearchService SystemGroupSearchService { get; }
    ISystemCodeSearchService SystemCodeSearchService { get; }
    ISequenceSearchService SequenceSearchService { get; }
    IAccoSearchService AccoSearchService { get; }
    IAccoReminderSearchService AccoReminderSearchService { get; }
    IAccoAdditionSearchService AccoAdditionSearchService { get; }
    IAccoAdditionDescriptionSearchService AccoAdditionDescriptionSearchService { get; }
    IAccoCancelConditionSearchService AccoCancelConditionSearchService { get; }
    ICountrySearchService CountrySearchService { get; }
    ICurrencySearchService CurrencySearchService { get; }
    ILanguageSearchService LanguageSearchService { get; }
    IMailTemplateSearchService MailTemplateSearchService { get; }
    IAccoOwnerSearchService AccoOwnerSearchService { get; }
    IAccoPayPatternSearchService AccoPayPatternSearchService { get; }
    IAccoPayPatternPaymentSearchService AccoPayPatternPaymentSearchService { get; }
    IAccoRentSearchService AccoRentSearchService { get; }
    IAccoSeasonSearchService AccoSeasonSearchService { get; }
    IAccoTrusteeSearchService AccoTrusteeSearchService { get; }
    IAccoSpecialOfferSearchService AccoSpecialOfferSearchService { get; }
    IAccoNotificationSearchService AccoNotificationSearchService { get; }
    IBookingSearchService BookingSearchService { get; }
    IBookingGuestSearchService BookingGuestSearchService { get; }
    IBookingAdditionSearchService BookingAdditionSearchService { get; }
    IBookingReminderSearchService BookingReminderSearchService { get; }
    IBookingPaymentSearchService BookingPaymentSearchService { get; }
    IBookingCancelConditionSearchService BookingCancelConditionSearchService { get; }
    IBookingPaymentToDoSearchService BookingPaymentToDoSearchService { get; }
    IBookingReminderToDoSearchService BookingReminderToDoSearchService { get; }
    IBookingExpiredToDoSearchService BookingExpiredToDoSearchService { get; }
    IPayPatternToDoSearchService PayPatternToDoSearchService { get; }
    ILicenseExpiredToDoSearchService LicenseExpiredToDoSearchService { get; }

    // @@@ JKT named query
    IRepository<Booking> ExpiredBookings { get; }       
  }
}