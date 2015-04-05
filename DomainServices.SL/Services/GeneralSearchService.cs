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
  public class SystemCodeSearchService : ISystemCodeSearchService
  {
    private readonly IRepository<SystemCode> _repository;

    public SystemCodeSearchService(IRepository<SystemCode> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public Task<IEnumerable<SystemCodeListItem>> FindSystemCodesByGroupAsync(
      string group, CancellationToken cancellationToken)
    {
      Expression<Func<SystemCode, bool>> filter = null;
      filter = x =>
        (string.IsNullOrWhiteSpace(group) || x.SystemGroup.Name == group);

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new SystemCodeListItem
        {
          Id = x.CodeId,
          Code = x.Code,
          Description = x.Description,
          IsDefault = x.IsDefault,
          Group = x.SystemGroup.Name,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Code));
    }

    public Task<IEnumerable<SystemCodeListItem>> FindSystemCodesAsync(
      int groupid, string searchText, CancellationToken cancellationToken)
    {
      Expression<Func<SystemCode, bool>> filter = null;
      if (!string.IsNullOrWhiteSpace(searchText))
        filter = x =>
          (groupid == 0 || x.GroupId == groupid) &&
          (x.Code.Contains(searchText) || x.Description.Contains(searchText));
      else
        filter = x =>
          (groupid == 0 || x.GroupId == groupid);

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new SystemCodeListItem
        {
          Id = x.CodeId,
          Code = x.Code,
          Description = x.Description,
          IsDefault = x.IsDefault,
          Group = x.SystemGroup.Name,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Description));
    }

    #endregion
  }

  public class SystemGroupSearchService : ISystemGroupSearchService
  {
    private readonly IRepository<SystemGroup> _repository;

    public SystemGroupSearchService(IRepository<SystemGroup> repository)
    {
      _repository = repository;
    }

    #region ISystemGroupSearchRepository Members

    public Task<IEnumerable<SystemGroupListItem>> FindSystemGroupsAsync(
      string searchText, CancellationToken cancellationToken)
    {
      Expression<Func<SystemGroup, bool>> filter = null;
      if (!string.IsNullOrWhiteSpace(searchText))
        filter = x => x.Name.Contains(searchText) || x.Name.Contains(searchText);

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new SystemGroupListItem
        {
          Id = x.GroupId,
          Group = x.Name,
          Description = x.Description,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Description));
    }

    #endregion
  }

  public class CountrySearchService : ICountrySearchService
  {
    private readonly IRepository<Country> _repository;

    public CountrySearchService(IRepository<Country> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<CountryListItem>> FindCountriesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Country, bool>> filter = null;
      //filter = x => x.AccoId == accoid;

      var countries = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new CountryListItem
        {
          Id = x.CountryId,
          //Accommodaton = x.Acco.Description,
          Description = x.Description,
          DisplaySequence = x.DisplaySequence.Value,

          Label = x.Description

        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));

      foreach (var country in countries)
        country.Description = SystemCodeService.Description(country.Description, SystemGroupName.Country);

      return countries;
    }

    #endregion
  }


  public class CurrencySearchService : ICurrencySearchService
  {
    private readonly IRepository<Currency> _repository;

    public CurrencySearchService(IRepository<Currency> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<CurrencyListItem>> FindCurrenciesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Currency, bool>> filter = null;
      //filter = x => x.AccoId == accoid;

      var currencies = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new CurrencyListItem
        {
          Id = x.CurrencyId,
          //Accommodaton = x.Acco.Description,
          Description = x.Description,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description

        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));

      foreach (var currency in currencies)
        currency.Description = SystemCodeService.Description(currency.Description, SystemGroupName.Country);

      return currencies;
    }

    #endregion
  }


  public class LanguageSearchService : ILanguageSearchService
  {
    private readonly IRepository<Language> _repository;

    public LanguageSearchService(IRepository<Language> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<LanguageListItem>> FindLanguagesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<Language, bool>> filter = null;
      //filter = x => x.AccoId == accoid;

      var languages = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new LanguageListItem
        {
          Id = x.LanguageId,
          //Accommodaton = x.Acco.Description,
          Description = x.Description,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.DisplaySequence));


      foreach (var language in languages)
        language.Description = SystemCodeService.Description(language.Description, SystemGroupName.Language);

      return languages;
    }

    #endregion
  }

  public class MailTemplateSearchService : IMailTemplateSearchService
  {
    private readonly IRepository<MailTemplate> _repository;

    public MailTemplateSearchService(IRepository<MailTemplate> repository)
    {
      _repository = repository;
    }

    #region ISystemCodeSearchService Members

    public async Task<IEnumerable<MailTemplateListItem>> FindMailTemplatesAsync(
      int accoid, CancellationToken cancellationToken)
    {
      Expression<Func<MailTemplate, bool>> filter = null;

      var templates = await _repository.FindInDataSourceAsync(
        q => q.Select(x => new MailTemplateListItem
        {
          Id = x.MailTemplateId,
          //Accommodaton = x.Acco.Description,
          MailContext = x.MailContext,
          Description = x.Description,
          DisplaySequence = x.DisplaySequence,

          Label = x.Description
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.Id));

      foreach (var template in templates)
        template.MailContext = SystemCodeService.Description(template.MailContext, SystemGroupName.MailContext);
      
      return templates;
    }

    #endregion
  }
}