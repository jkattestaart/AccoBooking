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
  public class PublicViewModel : Conductor<IScreen>, IDiscoverableViewModel
  {
    protected readonly INavigator _navigator;
    private MenuGroup _menuGroup;
    private IEnumerable<IWorkspace> _workspaces;
     
    [ImportingConstructor]
    public PublicViewModel([ImportMany] IEnumerable<IWorkspace> workspaces, MenuViewModel publicMenu)
    {
      EventFns.Subscribe(this);

      _workspaces = workspaces;
      PublicMenu = publicMenu;
      _navigator = new Navigator(this);

      var mainGroup = new MenuGroup(0, "Public");

      _workspaces.OrderBy(w => w.Sequence)
        .Where(x => x.IsPublic && x.GroupName == mainGroup.Label)
        .ForEach(w => mainGroup.Add(new MenuAction(this, w.DisplayName, async () => await NavigateToWorkspace(w))));

      PublicMenu.AddGroup(mainGroup);

    }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public MenuViewModel PublicMenu { get; private set; }

    protected async override void OnActivate()
    {
      base.OnActivate();
      PublicMenu.IsHorizontal = true;


      var home = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_HOME);

      await NavigateToWorkspace(home);
    }

    private async Task NavigateToWorkspace(IWorkspace workspace)
    {
      await _navigator.NavigateToAsync(workspace.ViewModelType);
      if (ActiveItem != null && workspace.DisplayName == Resources.AccoBooking.ws_HOME)
        (ActiveItem as HomeViewModel).Start();      //altijd terug naar homepagina (kan nog op andere pagina staan)
    }

    public async void Registreren()
    {
      var subscribe = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_SUBSCRIBE);

      await NavigateToWorkspace(subscribe);
    }
    
  }
}
