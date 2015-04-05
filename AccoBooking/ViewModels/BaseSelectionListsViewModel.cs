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

using System.Linq;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;
using DomainServices;

namespace AccoBooking.ViewModels
{

  public class BaseSelectionListsViewModel<TEntity, TListItem, TAllListItem> : BaseScreen<TEntity>
    where TEntity : class
    where TListItem : class
    where TAllListItem : class
  {
    protected BindableCollection<TListItem> _items;
    protected BindableCollection<TAllListItem> _allitems;
    protected TListItem _currentItem;
    protected TAllListItem _currentAllItem;
    protected ToolbarGroup _toolbarGroup;
    protected bool present;


    public BaseSelectionListsViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                       IDialogManager dialogManager,
                                       ToolbarViewModel toolbar)
      : base(unitOfWorkManager, dialogManager)
    {
      EventFns.Subscribe(this);

      Toolbar = toolbar;
    }

    public ToolbarViewModel Toolbar { get; private set; }

    public BindableCollection<TAllListItem> AllItems
    {
      get { return _allitems; }
      set
      {
        _allitems = value;
        NotifyOfPropertyChange(() => AllItems);
      }
    }

    public BindableCollection<TListItem> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        NotifyOfPropertyChange(() => Items);
      }
    }

    public TAllListItem CurrentAllItem
    {
      get { return _currentAllItem; }
      set
      {
        _currentAllItem = value;
        NotifyOfPropertyChange(() => CurrentAllItem);
      }
    }

    public TListItem CurrentItem
    {
      get { return _currentItem; }
      set
      {
        _currentItem = value;
        NotifyOfPropertyChange(() => CurrentItem);
        NotifyChanges();
      }
    }

    public virtual void GetItems()
    {
      
    }

    public virtual BaseSelectionListsViewModel<TEntity, TListItem, TAllListItem> Start()
    {
      Busy.AddWatch();

      GetItems();

      Toolbar.IsVisible = true;
      Toolbar.IsHorizontal = false;

      if (_toolbarGroup == null)
      {
        _toolbarGroup = new ToolbarGroup(10)
          {
            new ToolbarAction(this, Resources.AccoBooking.but_ALL_LEFT, "all-left.png", AllLeft),
            new ToolbarAction(this, Resources.AccoBooking.but_LEFT, "left.png", Left),
            new ToolbarAction(this, Resources.AccoBooking.but_RIGHT, "right.png", Right),
            new ToolbarAction(this, Resources.AccoBooking.but_ALL_RIGHT, "all-right.png", AllRight),
            new ToolbarAction(this, Resources.AccoBooking.but_TOP, "top.png", Top),
            new ToolbarAction(this, Resources.AccoBooking.but_UP, "up.png", Up),
            new ToolbarAction(this, Resources.AccoBooking.but_DOWN, "down.png", Down),
            new ToolbarAction(this, Resources.AccoBooking.but_BOTTOM, "bottom.png", Bottom),
          };

        Toolbar.AddGroup(_toolbarGroup);
      }

      return this;
    }


    public bool CanTop
    {
      get { return CanUp; }
    }

    public bool CanUp
    {
      get
      {
        if (Items != null)
          return Items.IndexOf(CurrentItem) > 0;
        return false;
      }
    }

    public bool CanDown
    {
      get
      {
        if (Items != null)
          return Items.IndexOf(CurrentItem) < Items.Count - 1;
        return false;
      }
    }

    public bool CanBottom
    {
      get { return CanDown; }
    }

    public bool CanAllLeft
    {
      get { return CanLeft; }
    }

    public bool CanLeft
    {
      get
      {
        if (Items != null) return (Items.Count > 0);
        return false;
      }
    }

    public bool CanRight
    {
      get
      {
        if (AllItems != null) return (AllItems.Count > 0);
        return false;
      }
    }

    public bool CanAllRight
    {
      get { return CanRight; }
    }

    public void Top()
    {
      while (Items.IndexOf(CurrentItem) > 0)
      {
        Up();
      }
    }

    public void Up()
    {
      var i = Items.IndexOf(CurrentItem);
      var current = CurrentItem;
      var custprod = Items.ElementAt(i - 1);

      BindableCollection<TListItem> copy = Items;
      Items = new BindableCollection<TListItem>();

      Items.AddRange(copy.Take(i - 1));
      Items.Add(current);
      Items.Add(custprod);
      Items.AddRange(copy.Skip(i + 1));
      CurrentItem = Items.ElementAt(i - 1);

      NotifyOfPropertyChange(() => CurrentItem);
      NotifyChanges();
    }

    public void Down()
    {
      var i = Items.IndexOf(CurrentItem);
      var current = CurrentItem;
      var custprod = Items.ElementAt(i + 1);

      BindableCollection<TListItem> copy = Items;
      Items = new BindableCollection<TListItem>();

      Items.AddRange(copy.Take(i));
      Items.Add(custprod);
      Items.Add(current);
      Items.AddRange(copy.Skip(i + 2));
      CurrentItem = Items.ElementAt(i + 1);

      NotifyOfPropertyChange(() => CurrentItem);
      NotifyChanges();
    }

    public void Bottom()
    {
      while (Items.IndexOf(CurrentItem) < Items.Count - 1)
      {
        Down();
      }
    }

    public void AllLeft()
    {
      while (CurrentItem != null)
      {
        Left();
      }
    }

    protected virtual TAllListItem MoveToLeft()
    {
      return null;
    }

    protected virtual void AddItem()
    {
    }

    protected virtual void DeleteItem()
    {
      
    }

    public void Left()
    {
      DeleteItem();

      TAllListItem prod = MoveToLeft();
     
      AllItems.Add(prod);
      Items.Remove(CurrentItem);
      CurrentItem = Items.FirstOrDefault();
      CurrentAllItem = prod;
      NotifyChanges();
    }

    protected virtual TListItem MoveToRight()
    {
      return null;
    }

    public void Right()
    {
      TListItem custprod = MoveToRight();

      AddItem();

      Items.Add(custprod);
      AllItems.Remove(CurrentAllItem);
      CurrentItem = custprod;
      CurrentAllItem = AllItems.FirstOrDefault();
      NotifyChanges();
    }

    public void AllRight()
    {
      while (CurrentAllItem != null)
      {
        Right();
      }
    }

    public void NotifyChanges()
    {
      NotifyOfPropertyChange(() => AllItems);
      NotifyOfPropertyChange(() => Items);
      NotifyOfPropertyChange(() => CanAllLeft);
      NotifyOfPropertyChange(() => CanLeft);
      NotifyOfPropertyChange(() => CanRight);
      NotifyOfPropertyChange(() => CanAllRight);
      NotifyOfPropertyChange(() => CanTop);
      NotifyOfPropertyChange(() => CanUp);
      NotifyOfPropertyChange(() => CanDown);
      NotifyOfPropertyChange(() => CanBottom);
    }

    protected override void OnActivate()

      // @@ JTK Onactivate wordt niet aangeroepen
    {
      //base.OnActivate();

      //if (_toolbarGroup == null)
      //{
      //  _toolbarGroup = new ToolbarGroup(10)
      //                    {
      //                      new ToolbarAction(this, "Left", "left.png", (Func<IEnumerable<IResult>>) Left),
      //                      new ToolbarAction(this, "Right", "right.png", (Func<IEnumerable<IResult>>) Right),
      //                      new ToolbarAction(this, "Up", "up.png", (Func<IEnumerable<IResult>>) Up),
      //                      new ToolbarAction(this, "Down", "down.png", (Func<IEnumerable<IResult>>) Down),
      //                    };
      //}
      //Toolbar.AddGroup(_toolbarGroup);   

    }

    protected override void OnDeactivate(bool close)
    {

      base.OnDeactivate(close);

      Toolbar.RemoveGroup(_toolbarGroup);
    }

    protected override IRepository<TEntity> Repository()
    {
      return null;
    }

    
  }


}