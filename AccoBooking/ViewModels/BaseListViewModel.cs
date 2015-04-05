// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels
{

  public delegate void ItemSelectedEventHandler(object sender, EventArgs e);

  /// <summary>
  /// Base class for a list object, like combo-boxes
  /// </summary>
  /// <typeparam name="TEntity">Entity in dit programma</typeparam>
  public abstract class BaseListViewModel<TEntity> : Screen where TEntity : class
  {
    protected string _description;
    protected string _shortName;
    protected int _itemid;
    protected readonly IAccoBookingUnitOfWork _unitOfWork;
    protected BindableCollection<TEntity> _items;
    protected TEntity _selectedItem;
    protected readonly IErrorHandler _errorHandler;
    protected readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _domainUnitOfWorkManager;
    protected bool _isEnabled = true;
    private bool _isVisible = true;


    /// <summary>
    /// Constructor for de list class
    /// </summary>
    /// <param name="unitOfWorkManager">Unit of work manager used</param>
    /// <param name="errorHandler">Handler for errors</param>
    protected BaseListViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
    {
      EventFns.Subscribe(this);

      _domainUnitOfWorkManager = unitOfWorkManager;
     _unitOfWork = _domainUnitOfWorkManager.Create();
    }

    public virtual bool IsVisible
    {
      get { return _isVisible; }
      set
      {
        _isVisible = value;
        NotifyOfPropertyChange(() => IsVisible);

      }
    }

    public virtual bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        _isEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    public event ItemSelectedEventHandler ItemSelected;

    // Invoke the ItemSelected event; called wheneve list is doubleclicked
    protected virtual void OnItemSelected(EventArgs e)
    {
      if (ItemSelected != null)
        ItemSelected(this, e);
    }

    //Bound method from the view
    public void DoubleClicked()
    {
      OnItemSelected(EventArgs.Empty);
    }

    /// <summary>
    /// Item in the list
    /// </summary>
    public BindableCollection<TEntity> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        NotifyOfPropertyChange(() => Items);
        if (_itemid == 0)
          SelectedItem = _items.FirstOrDefault();
        
      }
    }

    /// <summary>
    /// De shortname of an item
    /// </summary>
    public string ShortName
    {
      get { return _shortName; }
      set
      {
        _shortName = value;
        NotifyOfPropertyChange(() => ShortName);
      }
    }

    /// <summary>
    /// De shortname of an item
    /// </summary>
    public string Description
    {
      get { return _description; }
      set
      {
        _description = value;
        NotifyOfPropertyChange(() => Description);
      }
    }
    
    /// <summary>
    /// Id of an item
    /// </summary>
    public int ItemId
    {
      get { return _itemid; }
      set
      {
        _itemid = value;
        SetSelectedItem();
      }
    }

    private void SetSelectedItem()
    {
      if (_items != null)
        for (int i = 0; i < _items.Count; i++)
        {
          Entity item = _items[i] as Entity;
          PropertyInfo prop = item.GetType().GetProperty("Id");
          int id = 0;
          if (prop != null)
            id = (int)prop.GetValue(item, null);
          else
          {
            id = (int)item.EntityAspect.EntityKey.Values[0];
          }
          if (id == _itemid)
            SelectedItem = _items[i];
        }

      NotifyOfPropertyChange(() => ItemId);
      
    }

    /// <summary>
    /// The selected item
    /// </summary>
    public TEntity SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        _selectedItem = value;
        NotifyOfPropertyChange(() => SelectedItem);
      }
    }

    /// <summary>
    /// Start the listitem, must be coded
    /// </summary>
    /// <param name="selection">Extra selection on the items to create the predicate</param>
    /// <example>For systemcodes a shortname for the group can be specified</example>
    /// <returns></returns>
    public virtual void LoadDataAsync(int selection)
    {     
    }

    public virtual void LoadDataAsync(string selection)
    { 
    }

    public virtual BaseListViewModel<TEntity> Start(int selection)
    {
      LoadDataAsync(selection);
      return this;
    }

    public virtual BaseListViewModel<TEntity> Start(string selection)
    {
      LoadDataAsync(selection);
      return this;
    }

  }


}