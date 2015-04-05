using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Common.Actions;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MenuViewModel : Screen, IMenu
  {
    private bool _isVisible = true;
    private readonly List<MenuGroup> _groups;
    private bool _isHorizontal;

    public MenuViewModel()
    {
      _groups = new List<MenuGroup>();
      _isHorizontal = false;
    }

    #region IMenuManager Members

    public IEnumerable<MenuAction> Actions
    {
      get { return Groups.SelectMany(g => g); }
    }

    public bool IsVisible
    {
      get { return _isVisible; }

      set
      {
        _isVisible = value;
        NotifyOfPropertyChange(() => IsVisible); 
      }
    }

    public bool IsHorizontal
    {
      get { return _isHorizontal; }

      set
      {
        _isHorizontal = value;
        NotifyOfPropertyChange(() => IsHorizontal);
      }
    }

    public IEnumerable<MenuGroup> Groups
    {
      get { return _groups.OrderBy(g => g.Level); }
    }

    public void AddGroup(MenuGroup @group)
    {
      _groups.Add(@group);
      NotifyOfPropertyChange(() => Actions);
      NotifyOfPropertyChange(() => Groups);
    }

    public void RemoveGroup(MenuGroup @group)
    {
      _groups.Remove(@group);
      NotifyOfPropertyChange(() => Actions);
      NotifyOfPropertyChange(() => Groups);
    }

    public void Clear()
    {
      _groups.Clear();
    }

    #endregion
  }
}
