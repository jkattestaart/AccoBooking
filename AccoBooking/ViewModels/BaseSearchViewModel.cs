
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Cocktail;
using Common.Messages;
using DomainModel.Projections;
using DomainServices;
using IdeaBlade.EntityModel;


namespace AccoBooking.ViewModels
{
  /// <summary>
  /// Base class for the search program (probably a grid type)
  /// </summary>
  /// <typeparam name="TListItem">Listitem in the grid</typeparam>
  /// <typeparam name="TEntity">Entity for this program</typeparam>
  public abstract class BaseSearchViewModel<TEntity, TListItem> : Screen, IDiscoverableViewModel, IHarnessAware,
    IHandle<SavedMessage>

    where TEntity : class
    where TListItem : class
  {
    private readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    private IAccoBookingUnitOfWork _unitOfWork;
    protected TListItem _currentItem;
    protected BindableCollection<TListItem> _items;
    protected string _searchText;
    protected int _parentid;

    /// <summary>
    /// Constructor for the search program
    /// </summary>
    /// <param name="unitOfWorkManager"></param>
    protected BaseSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
    {
      EventFns.Subscribe(this);

      _unitOfWorkManager = unitOfWorkManager;
      Busy = new BusyWatcher();
    }

    /// <summary>
    /// Busy indicator
    /// </summary>
    public IBusyWatcher Busy { get; private set; }

    /// <summary>
    /// Propert for the filter
    /// </summary>
    public string SearchText
    {
      get { return _searchText; }
      set
      {
        _searchText = value;
        NotifyOfPropertyChange(() => SearchText);
        NotifyOfPropertyChange(() => CanSearch);
        NotifyOfPropertyChange(() => CanClear);
      }
    }

    /// <summary>
    /// The listitems for the grid. 
    /// </summary>
    public virtual BindableCollection<TListItem> Items
    {
      get { return _items; }
      set
      {
        _items = value;
        NotifyOfPropertyChange(() => Items);
      }
    }

    /// <summary>
    /// THe current item that is selected
    /// </summary>
    public TListItem CurrentItem
    {
      get { return _currentItem; }
      set
      {
        _currentItem = value;
        NotifyOfPropertyChange(() => CurrentItem);
      }
    }

    /// <summary>
    /// Command if search is possible
    /// </summary>
    public virtual bool CanSearch
    {
      get { return !string.IsNullOrWhiteSpace(SearchText); }
    }

    /// <summary>
    /// Command if cancel is possible
    /// </summary>
    public virtual bool CanClear
    {
      get { return !string.IsNullOrWhiteSpace(SearchText); }
    }

    protected IAccoBookingUnitOfWork UnitOfWork
    {
      // Return the shared UoW associated with Guid.Empty
      get { return _unitOfWork ?? (_unitOfWork = _unitOfWorkManager.Get(0)); }
    }

    #region IHandle<SavedMessage> Members

    /// <summary>
    /// Handle messages after commit (reposition to current item)
    /// </summary>
    /// <param name="message"></param>
    public void Handle(SavedMessage message)
    {
      // Exit if no entitye was saved
      if (!message.Entities.OfType<TEntity>().Any()) return;

      // If selected entity is detached now, that means it got deleted.
      bool wasDeleted = message.Entities
        .OfType<TEntity>()
        .Any(entity => (entity as Entity).EntityAspect.EntityState.IsDetached() &&
                       (int) (entity as Entity).EntityAspect.EntityKey.Values[0] == (CurrentItem as ListItemBase).Id);

      if (wasDeleted)
      {
        Search();
        return;
      }

      if (
        CurrentItem != null)
      {
        Search((CurrentItem as ListItemBase).Id);
        return;
      }

      TEntity newEntity = message.Entities
        .OfType<TEntity>()
        .FirstOrDefault();
      if (newEntity != null)
        //if (_parentid != 0)
        //  Search(_parentid, (int)(newEntity as Entity).EntityAspect.EntityKey.Values[0]);
        //else
        Search((int) (newEntity as Entity).EntityAspect.EntityKey.Values[0]);
    }

    #endregion

    #region IHarnessAware Members

    public void Setup()
    {
      Start(0);
    }

    #endregion

    /// <summary>
    /// Start the search program
    /// </summary>
    /// <returns>this program</returns>
    public virtual BaseSearchViewModel<TEntity, TListItem> Start()
    {
      Search();
      return this;
    }


    public virtual BaseSearchViewModel<TEntity, TListItem> Reposition(int entityid)
    {
      Search(entityid);
      return this;
    }


    /// <summary>
    /// Start the search program (one to many)
    /// </summary>
    /// <param name="parentid">id from the parent entity</param>
    /// <returns>this program</returns>
    public virtual BaseSearchViewModel<TEntity, TListItem> Start(int parentid)
    {
      _parentid = parentid;
      Search();
      return this;
    }

    /// <summary>
    /// Execute the Search with no parent entity
    /// </summary>
    public virtual void Search()
    {
      Search(0);
    }

    /// <summary>
    /// The actual query is done by a Search Service
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    protected abstract Task<IEnumerable<TListItem>> ExecuteQuery();

    /// <summary>
    /// Search the items for the selectedid, if not found the first item
    /// </summary>
    /// <param name="selectedid">the id to be selected</param>
    public virtual void Search(int parentid, int selectedid)
    {
      _parentid = parentid;
      Search(selectedid);
    }

    public virtual async void Search(int selectedid)
    {
      using (Busy.GetTicket())
      {
        var items = await ExecuteQuery();

        Items = new BindableCollection<TListItem>(items);

        PostSearch();

        CurrentItem = Items.FirstOrDefault(code => (code as ListItemBase).Id == selectedid) ??
                      Items.FirstOrDefault();
      }

    }

    public virtual void PostSearch()
    {
      
    }

    /// <summary>
    /// Clear the search filter
    /// </summary>
    public virtual void Clear()
    {
      SearchText = "";
      Search();
    }

    /// <summary>
    /// Enter on search text
    ///  </summary>
    /// <param name="e">key parameter argument</param>
    public void SearchTextKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        Search();
    }

    /// <summary>
    /// Deactivation of this program 
    /// </summary>
    /// <param name="close">indication that program closes</param>
    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);

      if (close)
        _unitOfWork = null;

      //if (close)
      //  _parentid = 0;
    }

  }

}
