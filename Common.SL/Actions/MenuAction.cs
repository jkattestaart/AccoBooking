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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Caliburn.Micro;
using Action = System.Action;

namespace Common.Actions
{
  public class MenuAction : PropertyChangedBase
  {
    private readonly Action _action;
    private readonly object _owner;
    private PropertyInfo _actionEnabledProperty;
    private string _label;
    private string _image;
    private bool _isEnabled;
    private bool _isVisible;

    public MenuAction(object owner, string label, string image, Action action)
    {
      if (owner == null) throw new ArgumentNullException("owner");
      if (action == null) throw new ArgumentNullException("action");

      _owner = owner;
      _action = action;
      Label = label;
      Image = image;
      IsEnabled = true;
      IsVisible = true;
      EnsureOwner();
    }

    public MenuAction(object owner, string label, Action action)
    {
      if (owner == null) throw new ArgumentNullException("owner");
      if (action == null) throw new ArgumentNullException("action");

      _owner = owner;
      _action = action;
      Label = label;

      EnsureOwner();
    }

    
    public string Image
    {
      get { return _image; }
      set
      {
        _image = value;
        NotifyOfPropertyChange(() => Image);
      }
    }

    public string Label
    {
      get { return _label; }
      set
      {
        _label = value;
        NotifyOfPropertyChange(() => Label);
      }
    }

    public bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        _isEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
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

    public bool CanExecute
    {
      get { return _actionEnabledProperty == null || (bool) _actionEnabledProperty.GetValue(_owner, null); }
    }

    private void EnsureOwner()
    {
      if (!(_owner is INotifyPropertyChanged)) return;

      var actionMethodInfo = _action.Method;
      _actionEnabledProperty = _owner.GetType().GetProperty("Can" + actionMethodInfo.Name, typeof(bool));

      if (_actionEnabledProperty != null)
        (_owner as INotifyPropertyChanged).PropertyChanged += OwnerPropertyChanged;
    }

    private void OwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == _actionEnabledProperty.Name)
        NotifyOfPropertyChange(() => CanExecute);
    }

    public void Execute()
    {
      //if (_actionCoroutine != null)
      //  _actionCoroutine();
      //else
        _action();
    }
  }

  public class MenuGroup : ObservableCollection<MenuAction>
  {
    private ObservableCollection<MenuAction> _children;

    public MenuGroup(int level, string label)
    {
      Level = level;
      Label = label;
      _children = new ObservableCollection<MenuAction>();
    }

    public int Level { get; private set; }

    public string Label { get; private set; }

    public ObservableCollection<MenuAction> Children { get; set; }

    public void AddAction(MenuAction @action)
    {
      _children.Add(action);
    }
  }
}