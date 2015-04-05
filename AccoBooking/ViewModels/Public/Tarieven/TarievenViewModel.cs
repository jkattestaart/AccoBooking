//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;

namespace AccoBooking.ViewModels
{
  [Export]
  public class TarievenViewModel : Conductor<IScreen>, IDiscoverableViewModel
  {
    private MenuGroup _menuGroup;

    protected readonly INavigator _navigator;
    private ExportFactory<LicenseTermsViewModel> _licenseFactory;
    private ExportFactory<TarievenDetailViewModel> _tarievenDetailFactory;

    [ImportingConstructor]
    public TarievenViewModel(ExportFactory<TarievenDetailViewModel> tarievenDetailFactory,
                             ExportFactory<LicenseTermsViewModel> licenseFactory)
    {
      EventFns.Subscribe(this);

      _tarievenDetailFactory = tarievenDetailFactory;
      _licenseFactory = licenseFactory;

      _navigator = new Navigator(this);
    }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public MenuViewModel TopHomeMenu { get; private set; }
    public MenuViewModel BottomHomeMenu { get; private set; }

    protected override void OnActivate()
    {
      base.OnActivate();
      Start();
    }

    public async void Start()
    {
      try
      {
        var home = _tarievenDetailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(home.GetType(),
          target =>
          {

            (target as TarievenDetailViewModel).Parent = this; // @@@@
            ((IActivate)target).Activate();
            //(target as HelpdeskViewModel).Start();
          });
      }
      catch (TaskCanceledException)
      {

      }
    }

    public async void LicenseTerms()
    {
      try
      {
        var home = _licenseFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(home.GetType(),
          target =>
          {

            (target as LicenseTermsViewModel).Parent = this; // @@@@
            ((IActivate)target).Activate();
            //(target as HelpdeskViewModel).Start();
          });
      }
      catch (TaskCanceledException)
      {

      }
    }


  }
}
