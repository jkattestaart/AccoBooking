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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;
using Common.Workspace;
using IdeaBlade.Core;

namespace AccoBooking.ViewModels
{
  [Export]
  public class HomeViewModel : Conductor<IScreen>, IDiscoverableViewModel
  {
    private MenuGroup _menuGroup;

    protected readonly INavigator _navigator;
    private IEnumerable<IWorkspace> _workspaces;
    private bool first = true;

    [ImportingConstructor]
    public HomeViewModel([ImportMany] IEnumerable<IWorkspace> workspaces, MainMenuViewModel topHomeMenu, MainMenuViewModel bottomHomeMenu)
    {
      EventFns.Subscribe(this);

      _workspaces = workspaces;
      
      TopHomeMenu = topHomeMenu;
      BottomHomeMenu = bottomHomeMenu;
      _navigator = new Navigator(this);
	  
	    TopHomeMenu.IsHorizontal = true;
      BottomHomeMenu.IsHorizontal = true;

      var mainGroup = new MenuGroup(0, "Home");

      _workspaces.OrderBy(w => w.Sequence)
        .Where(x => x.IsPublic && x.GroupName == mainGroup.Label)
        .ForEach(w => mainGroup.Add(new MenuAction(this, w.DisplayName, async () => await NavigateToWorkspace(w))));

      BottomHomeMenu.AddGroup(mainGroup);
      TopHomeMenu.AddGroup(mainGroup);
    }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public MainMenuViewModel TopHomeMenu { get; private set; }
    public MainMenuViewModel BottomHomeMenu { get; private set; }

    protected override void OnActivate()
    {
      base.OnActivate();

      if (first)
        Start();
      
    }

    private async Task NavigateToWorkspace(IWorkspace workspace)
    {
      if (workspace.DisplayName != Resources.AccoBooking.ws_HOME_RENTAL)
      {
        TopHomeMenu.IsVisible = true;
        BottomHomeMenu.IsVisible = false;
      }

      await _navigator.NavigateToAsync(workspace.ViewModelType);
    }
    
    public async void Start()
    {
      first = false;

      TopHomeMenu.IsVisible = false;
      BottomHomeMenu.IsVisible = true;

      var homerental = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_HOME_RENTAL);

      await NavigateToWorkspace(homerental);
		  (ActiveItem as HomeRentalViewModel).Start();
    }

  }
}
