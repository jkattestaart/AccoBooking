using System.ComponentModel.Composition;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MainMenuViewModel : MenuViewModel
  {
  }
}
