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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainServices.Services;

namespace DomainServices.Factories
{

  public class SystemGroupFactory : Factory<SystemGroup>
  {
    public SystemGroupFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<SystemGroup> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.SystemGroupId, cancellationToken);

      var systemgroup = new SystemGroup();
      EntityManager.AddEntity(systemgroup);
      systemgroup.GroupId = sequence.CurrentId;

      return systemgroup;
    }
  }


  public class SystemCodeFactory : Factory<SystemCode>
  {

    public SystemCodeFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<SystemCode> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.SystemCodeId, cancellationToken);

      var systemcode = new SystemCode();
      EntityManager.AddEntity(systemcode);
      systemcode.CodeId = sequence.CurrentId;

      return systemcode;
    }
  }

  public class CountryFactory : Factory<Country>
  {
    private readonly IRepository<Country> _countries;

    public CountryFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _countries = new Repository<Country>(entityManagerProvider);
    }

    public override async Task<Country> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.CountryId, cancellationToken);

      var countries = await _countries.AllInDataSourceAsync(cancellationToken);
      
      var lastcountry = countries.OrderBy(x => x.DisplaySequence).LastOrDefault();

      var country = new Country();
      EntityManager.AddEntity(country);
      country.CountryId = sequence.CurrentId;

      if (lastcountry != null)
        country.DisplaySequence = lastcountry.DisplaySequence + 10;
      else
        country.DisplaySequence = 10;

      return country;
    }
  }


  public class CurrencyFactory : Factory<Currency>
  {
    private readonly IRepository<Currency> _currencies;

    public CurrencyFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _currencies = new Repository<Currency>(entityManagerProvider);
    }

    public override async Task<Currency> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.CurrencyId, cancellationToken);

      var countries = await _currencies.AllInDataSourceAsync(cancellationToken);

      var lastcountry = countries.OrderBy(x => x.DisplaySequence).LastOrDefault();

      var currency = new Currency();
      EntityManager.AddEntity(currency);
      currency.CurrencyId = sequence.CurrentId;

      if (lastcountry != null)
        currency.DisplaySequence = lastcountry.DisplaySequence + 10;
      else
        currency.DisplaySequence = 10;

      return currency;
    }
  }
  
  
  public class LanguageFactory : Factory<Language>
  {
    public LanguageFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
    }

    public override async Task<Language> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.LanguageId, cancellationToken);

      var accoLanguage = new Language();
      EntityManager.AddEntity(accoLanguage);
      accoLanguage.LanguageId = sequence.CurrentId;

      return accoLanguage;
    }
  }

  public class MailTemplateFactory : Factory<MailTemplate>
  {
    private readonly IRepository<Language> _languages;

    public MailTemplateFactory(IEntityManagerProvider<AccoBookingEntities> entityManagerProvider)
      : base(entityManagerProvider)
    {
      _languages = new Repository<Language>(entityManagerProvider);
    }

    public override async Task<MailTemplate> CreateAsync(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var sequence = await SequenceKeyService.NextValueAsync(SequenceName.MailTemplateId, cancellationToken);

      var accoMailTemplate = new MailTemplate();
      EntityManager.AddEntity(accoMailTemplate);
      accoMailTemplate.MailTemplateId = sequence.CurrentId;

      return accoMailTemplate;
    }
  }

}