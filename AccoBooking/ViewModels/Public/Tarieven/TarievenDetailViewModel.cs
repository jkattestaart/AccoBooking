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
using Caliburn.Micro;
using Cocktail;
using Common.Actions;

namespace AccoBooking.ViewModels
{
  [Export]
  public class TarievenDetailViewModel : Conductor<IScreen>, IDiscoverableViewModel
  {
    private MenuGroup _menuGroup;

    protected readonly INavigator _navigator;

    [ImportingConstructor]
    public TarievenDetailViewModel()
    {
      EventFns.Subscribe(this);
 
      _navigator = new Navigator(this);
    }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public MenuViewModel TopHomeMenu { get; private set; }
    public MenuViewModel BottomHomeMenu { get; private set; }

   
    public async void LicenseTerms()
    {
      (Parent as TarievenViewModel).LicenseTerms();
    }


  }
}
