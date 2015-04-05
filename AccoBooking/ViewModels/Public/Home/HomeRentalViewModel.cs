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

namespace AccoBooking.ViewModels
{
  [Export]
  public class HomeRentalViewModel : Screen, IDiscoverableViewModel
  {
    private bool _selected2EHuis;
    private bool _selectedMeerdereHuizen;

    [ImportingConstructor]
    public HomeRentalViewModel(MenuViewModel homeMenu)
    {
      EventFns.Subscribe(this);
    }

    public bool Selected2eHuis
    {
      get { return _selected2EHuis; }
      set
      {
        _selected2EHuis = value;
        NotifyOfPropertyChange(() => Selected2eHuis);
        NotifyOfPropertyChange(() => NoSelections);
      }
    }

    public bool SelectedMeerdereHuizen
    {
      get { return _selectedMeerdereHuizen; }
      set
      {
        _selectedMeerdereHuizen = value;
        NotifyOfPropertyChange(() => SelectedMeerdereHuizen);
        NotifyOfPropertyChange(() => NoSelections);
      }
    }


    public bool NoSelections
    {
      get { return !Selected2eHuis && !SelectedMeerdereHuizen; }
    }

    public void LeesVerder2eHuis()
    {
      (Parent as HomeViewModel).TopHomeMenu.IsVisible = false;
      Selected2eHuis = true;
    }

    public void LeesVerderMeerdereHuizen()
    {
      SelectedMeerdereHuizen = true;
    }

    public void Start()
    {
      Selected2eHuis = false;
      SelectedMeerdereHuizen = false;
    }

    public void Aanmelden()
    {

     ((Parent as HomeViewModel).Parent as PublicViewModel).Registreren();
    }

    public void PubDiscount1()
    {
      Aanmelden();
    }

    public void PubDiscount2()
    {
      Aanmelden();
    }

  }
}
