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
  public class SupportViewModel : Conductor<IScreen>, IDiscoverableViewModel
  {
    private MenuGroup _menuGroup;

    protected readonly INavigator _navigator;
    private ExportFactory<HelpdeskViewModel> _helpdeksFactory;
    private ExportFactory<VideoViewModel> _videoFactory;
    private ExportFactory<FAQViewModel> _faqFactory;

    [ImportingConstructor]
    public SupportViewModel(
      ExportFactory<HelpdeskViewModel> helpdeskFactory,
      ExportFactory<VideoViewModel> videoFactory,
      ExportFactory<FAQViewModel> faqFactory)
    {
      EventFns.Subscribe(this);

      _helpdeksFactory = helpdeskFactory;
      _videoFactory = videoFactory;
      _faqFactory = faqFactory;
      
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
        var home = _helpdeksFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(home.GetType(),
          target =>
          {

            (target as HelpdeskViewModel).Parent = this; // @@@@
            ((IActivate)target).Activate();
            //(target as HelpdeskViewModel).Start();
          });
      }
      catch (TaskCanceledException)
      {

      }
    }

    public async void FAQ()
    {
      try
      {
        var home = _faqFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(home.GetType(),
          target =>
          {

            (target as FAQViewModel).Parent = this; // @@@@
            ((IActivate)target).Activate();
            //(target as FAQViewModel).Start();
          });
      }
      catch (TaskCanceledException)
      {

      }
    }

    public async void Videos()
    {
      try
      {
        var home = _videoFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(home.GetType(),
          target =>
          {

            (target as VideoViewModel).Parent = this; // @@@@
            ((IActivate)target).Activate();
            //(target as VideoViewModel).Start();
          });
      }
      catch (TaskCanceledException)
      {

      }
    }

  }
}
