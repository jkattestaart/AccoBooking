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
using System.Globalization;
using Caliburn.Micro;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class SupportedLanguageListViewModel : BaseListViewModel<Language>
  {
    [ImportingConstructor]
    public SupportedLanguageListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    }

    public override BaseListViewModel<Language> Start(int accoid)
    {
      //var orderBySelector = new SortSelector("Name");
      Items = new BindableCollection<Language>();
      if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "nl" |
          CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "be")
      {
        Items.Add(new Language() {Description = "Nederlands", DisplaySequence = 10});
        Items.Add(new Language() {Description = "Engels", DisplaySequence = 20});
      }
      else
      {
        Items.Add(new Language() {Description = "English", DisplaySequence = 10});
        Items.Add(new Language() {Description = "Dutch", DisplaySequence = 20});
      }
      return this;
    }
  }
}