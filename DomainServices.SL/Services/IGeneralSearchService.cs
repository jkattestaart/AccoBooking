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
  public interface ISystemGroupSearchService : IHideObjectMembers
  {
    Task<IEnumerable<SystemGroupListItem>> FindSystemGroupsAsync(
      string searchText, CancellationToken cancellationToken);
  }

  public interface ISystemCodeSearchService : IHideObjectMembers
  {
    Task<IEnumerable<SystemCodeListItem>> FindSystemCodesAsync(
      int groupid, string searchText, CancellationToken cancellationToken);

    Task<IEnumerable<SystemCodeListItem>> FindSystemCodesByGroupAsync(
      string group, CancellationToken cancellationToken);
  }

  public interface ICountrySearchService : IHideObjectMembers
  {
    Task<IEnumerable<CountryListItem>> FindCountriesAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface ICurrencySearchService : IHideObjectMembers
  {
    Task<IEnumerable<CurrencyListItem>> FindCurrenciesAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface ILanguageSearchService : IHideObjectMembers
  {
    Task<IEnumerable<LanguageListItem>> FindLanguagesAsync(
      int accoid, CancellationToken cancellationToken);
  }

  public interface IMailTemplateSearchService : IHideObjectMembers
  {
    Task<IEnumerable<MailTemplateListItem>> FindMailTemplatesAsync(
      int accoid, CancellationToken cancellationToken);
  }

}