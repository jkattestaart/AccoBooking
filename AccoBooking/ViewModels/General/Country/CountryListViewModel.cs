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
using Caliburn.Micro;
using DomainModel;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class CountryListViewModel : BaseListViewModel<Country>
  {
    [ImportingConstructor]
    public CountryListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    }
    
    public async override void LoadDataAsync(int selection)
    {
      var countries = await _unitOfWork.Countries.AllInDataSourceAsync(q => q.OrderBy(c => c.DisplaySequence));
      foreach (var country in countries)
      {
        var ctry = SystemCodeService.SystemCodeList
          .FirstOrDefault(t => t.Code == country.Description.ToUpper() && t.SystemGroup.Name == "COUNTRY");
        if (ctry != null)
          country.Description = ctry.Description;
        else
          country.Description = "!!" + country.Description;
      }
      Items = new BindableCollection<Country>(countries);

    }
  }
}