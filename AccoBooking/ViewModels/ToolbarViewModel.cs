using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;

namespace AccoBooking.ViewModels
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class ToolbarViewModel : Screen, IToolbar
  {
    private bool _isVisible;
    private bool _isHorizontal = true;
    private readonly List<ToolbarGroup> _groups;
    private int _horizontalAlignment;

    public ToolbarViewModel()
    {
      EventFns.Subscribe(this);
      _groups = new List<ToolbarGroup>();
    }

    #region IToolbarManager Members

    public IEnumerable<ToolbarAction> Actions
    {
      get { return Groups.SelectMany(g => g); }
    }

    public int HorizontalAlignment
    {
      get { return _horizontalAlignment; }

      set
      {
        _horizontalAlignment = value;
        NotifyOfPropertyChange(() => HorizontalAlignment);
      }
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

    public IEnumerable<ToolbarGroup> Groups
    {
      get { return _groups.OrderBy(g => g.Level); }
    }

    public void AddGroup(ToolbarGroup @group)
    {
      _groups.Add(@group);
      NotifyOfPropertyChange(() => Actions);
      NotifyOfPropertyChange(() => Groups);
    }

    public void RemoveGroup(ToolbarGroup @group)
    {
      _groups.Remove(@group);
      NotifyOfPropertyChange(() => Actions);
      NotifyOfPropertyChange(() => Groups);
    }

    public void Clear()
    {
      _groups.Clear();
      NotifyOfPropertyChange(() => Groups);
      NotifyOfPropertyChange(() => Actions);
    }

    #endregion
  }
}
