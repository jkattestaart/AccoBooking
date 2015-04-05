using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Threading;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;
using Common.Messages;
using DomainModel.Projections;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels
{

  public interface IBaseMasterDetailViewModel
  {
    int EntityId { get; set; }
  }

  /// <summary>
  /// Base class for standard maintenance programs (Caliburn Conductor for IScreen)
  /// </summary>
  /// <typeparam name="TDetailViewModel">Detail ViewModel used</typeparam>
  /// <typeparam name="TListItem">Listitem in the grid</typeparam>
  /// <typeparam name="TEntity">Entity for this program</typeparam>
  public abstract class BaseMasterViewModel<TSearchViewModel, TDetailViewModel, TListItem, TEntity> : Conductor<IScreen>,
    IDiscoverableViewModel,
    IHandle<EntityChangedMessage>,
    IBaseMasterDetailViewModel

    where TSearchViewModel : class
    where TDetailViewModel : class
    where TEntity : class
    where TListItem : class
  {

    protected readonly ExportFactory<TSearchViewModel> _searchFactory;
    protected readonly ExportFactory<TDetailViewModel> _detailFactory;
    protected readonly IDialogManager _dialogManager;
    protected readonly INavigator _navigator;
    protected readonly INavigator _searchNavigator;
    protected readonly DispatcherTimer _selectionChangeTimer;
    protected readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    protected IScreen _retainedActiveItem;
    protected ToolbarGroup _toolbarGroup;
    protected int _parentid;
    protected BaseDetailViewModel<TEntity> _activeDetail;
    private TListItem _item;
    protected ToolbarGroup _bottomToolbarGroup;
    private bool _useCopy = false;
    private bool _useAdd = true;
    private bool _useUpdate = true;
    private bool _useRefresh;
    private bool _useDelete = true;
    protected bool _useSearch = true;
    protected int _entityid;
    

    /// <summary>
    /// Constructor for the maintenance program
    /// </summary>
    /// <param name="searchFactory"></param>
    /// <param name="detailFactory">Factory created for Detail ViewModel part</param>
    /// <param name="unitOfWorkManager">Unit of Work manager</param>
    /// <param name="dialogManager">Dialog manager</param>
    /// <param name="toolbar">Toolbar manager</param>
    /// <param name="bottomToolbar"></param>
    protected BaseMasterViewModel(ExportFactory<TSearchViewModel> searchFactory,
      ExportFactory<TDetailViewModel> detailFactory,
      IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      IDialogManager dialogManager,
      ToolbarViewModel toolbar,
      ToolbarViewModel bottomToolbar)
    {
      EventFns.Subscribe(this);

      _searchFactory = searchFactory;
      _detailFactory = detailFactory;
      _unitOfWorkManager = unitOfWorkManager;
      _dialogManager = dialogManager;
      Toolbar = toolbar;
      BottomToolbar = bottomToolbar;

      _searchNavigator = new Navigator(this);
      _navigator = new Navigator(this);

      PropertyChanged += OnPropertyChanged;

      _selectionChangeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 200) };
      _selectionChangeTimer.Tick += OnSelectionChangeElapsed;

      Busy = new BusyWatcher();
    }

    public int EntityId
    {
      get { return _entityid; }
      set { _entityid = value; }
    }

    public bool UseSearch
    {
      get { return _useSearch; }
      set { _useSearch = value; }
    }

    protected virtual string EntityLabel
    {
      get { return ((SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem as ListItemBase).Label; }
    }

    public event EventHandler<ResultCompletionEventArgs> Completed;

    public void DoubleClicked()
    {
      Edit();
    }


    /// <summary>
    /// Busy indicator
    /// </summary>
    public IBusyWatcher Busy { get; private set; }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public ToolbarViewModel Toolbar { get; protected set; }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public ToolbarViewModel BottomToolbar { get; set; }

    public bool UseAdd
    {
      get { return _useAdd; }
      set { _useAdd = value; }
    }

    public bool UseCopy
    {
      get { return _useCopy; }
      set { _useCopy = value; }
    }

    public bool UseUpdate
    {
      get { return _useUpdate; }
      set { _useUpdate = value; }
    }

    public bool UseDelete
    {
      get { return _useDelete; }
      set { _useDelete = value; }
    }

    public bool UseRefresh
    {
      get { return _useRefresh; }
      set { _useRefresh = value; }
    }


    /// <summary>
    /// Grid ViewModel placeholder
    /// </summary>
    public TSearchViewModel SearchPane { get; protected set; }

    /// <summary>
    /// Active unit of work for this entity
    /// </summary>
    protected IAccoBookingUnitOfWork ActiveUnitOfWork
    {
      get { return _unitOfWorkManager.Get((int)ActiveEntity.EntityAspect.EntityKey.Values[0]); }
    }

    /// <summary>
    /// Active Detail ViewModel
    /// </summary>
    protected virtual BaseDetailViewModel<TEntity> ActiveDetail
    {
      get
      {
        if (IsDetailActive)
          _activeDetail = (BaseDetailViewModel<TEntity>)ActiveItem;
        else
          _activeDetail = null;
        return _activeDetail;
      }

    }

    public bool IsDetailActive
    {
      get { return ActiveItem is BaseDetailViewModel<TEntity>; }
    }

    public bool IsSearchActive
    {
      get { return ActiveItem is BaseSearchViewModel<TEntity, TListItem>; }
    }

    /// <summary>
    /// Active Entity
    /// </summary>
    protected Entity ActiveEntity
    {
	  get { return IsDetailActive ? (ActiveDetail as BaseDetailViewModel<TEntity>).Entity : null; }

      //get { return ActiveDetail != null ? (ActiveDetail as BaseDetailViewModel<TEntity>).Entity : null; }
    }

    //<summary>
    //Deletes enabled when there is a item selected in the grid
    //</summary>
    public bool CanDelete
    {
      get { return !IsDetailActive && (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem != null; }
    }

    /// <summary>
    /// Save enabled when there is an active entity and there are changes in the active unit of work
    /// Not valid if active entity is deleted
    /// </summary>
    public bool CanSave
    {
      get
      {
        return IsDetailActive && ActiveEntity != null && ActiveUnitOfWork.HasChanges() &&
               !ActiveEntity.EntityAspect.EntityState.IsDeleted();
      }
    }

    /// <summary>
    /// Cancel enabled like save
    /// </summary>
    public bool CanCancel
    {
      get { return IsDetailActive; }
    }

    /// <summary>
    /// Editing enabled
    /// </summary>
    public bool CanEdit
    {
      get { return !IsDetailActive; }
    }

    /// <summary>
    /// Editing enabled
    /// </summary>
    public bool CanAdd
    {
      get { return !IsDetailActive; }
    }

    /// <summary>
    /// Editing enabled
    /// </summary>
    public bool CanCopy
    {
      get { return !IsDetailActive; }
    }
    
    /// <summary>
    /// Refresh enabled when active entity is set and not added
    /// </summary>
    public bool CanRefreshData
    {
      get { return ActiveEntity != null && !ActiveEntity.EntityAspect.EntityState.IsAdded(); }
    }

    #region IHandle<EntityChangedMessage> Members

    /// <summary>
    /// If there are no messsages update the comamnd buttons
    /// </summary>
    /// <param name="message">melding die ontstaan is</param>
    public void Handle(EntityChangedMessage message)
    {
      if (ActiveEntity == null || !ActiveUnitOfWork.HasEntity(message.Entity))
        return;
      UpdateCommands();
    }

    #endregion

    /// <summary>
    /// When active item changes handle the property changed for the new active item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "ActiveItem") return;
      if (_retainedActiveItem != null)
        _retainedActiveItem.PropertyChanged -= OnActiveDetailPropertyChanged;

      _retainedActiveItem = ActiveItem;
      if (ActiveItem != null)
        ActiveItem.PropertyChanged += OnActiveDetailPropertyChanged;

      UpdateCommands();
    }

    /// <summary>
    /// When entity changes update the command buttons
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnActiveDetailPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Entity")
        UpdateCommands();
    }

    /// <summary>
    /// Start this program as 1 to many from the parent
    /// </summary>
    /// <param name="parentid">entity id van de parent waarmee entity een relatie heeft</param>
    /// <returns>this program</returns>
    public virtual BaseMasterViewModel<TSearchViewModel, TDetailViewModel, TListItem, TEntity> Start(int parentid)
    {
      _parentid = parentid;
      Start();
      ((IActivate)this).Activate();
      return this;
    }

    /// <summary>
    /// Start this program without a parent
    /// </summary>
    /// <returns>this program</returns>
    public virtual BaseMasterViewModel<TSearchViewModel, TDetailViewModel, TListItem, TEntity> Start()
    {
      //if (_parentid == 0)
      //  SearchPane.Start();
      //else
      //  SearchPane.Start(_parentid);
      if (Toolbar != null)
        Toolbar.IsVisible = true;

      StartAsync();
      return this;
    }

    public async void StartAsync()
    {
      try
      {

        //if (ActiveStaffingResource != null && ActiveStaffingResource.Id == SearchPane.CurrentStaffingResource.Id)
        //  return;
        var search = SearchPane ?? _searchFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(search.GetType(),
          target =>
          {
            if (_item != null)
              if (_parentid == 0)
                (target as BaseSearchViewModel<TEntity, TListItem>).Search((_item as ListItemBase).Id);
              else
                (target as BaseSearchViewModel<TEntity, TListItem>).Search(_parentid, (_item as ListItemBase).Id);
            else
              if (_parentid == 0)
                (target as BaseSearchViewModel<TEntity, TListItem>).Start();
              else
                (target as BaseSearchViewModel<TEntity, TListItem>).Start(_parentid);
            if (SearchPane != target)
            {
              SearchPane = (TSearchViewModel)target;
              (SearchPane as BaseSearchViewModel<TEntity, TListItem>).PropertyChanged += OnSearchPanePropertyChanged;
            }
            ((IActivate)SearchPane).Activate();
          });

      }



      catch (TaskCanceledException)
      {
        (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem = null;
        UpdateCommands();
      }


    }

    /// <summary>
    /// Activating the program by starting the program and activating the grid
    /// </summary>
    protected override void OnActivate()
    {
      base.OnActivate();

      if (UseSearch)
        Start();
      // SearchPane.PropertyChanged += OnSearchPanePropertyChanged;
      // ((IActivate) SearchPane).Activate();

      if (_toolbarGroup == null)
      {
        _toolbarGroup = new ToolbarGroup(10);
        if (UseCopy)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_COPY, "copy.png", Copy));
        if (UseAdd)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_ADD, "add.png", Add));
        if (UseUpdate)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_EDIT, "edit.png", Edit));
        if (UseDelete)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_DELETE, "remove.png", Delete));

        if (BottomToolbar == null)
        {
          if (UseAdd || UseUpdate)
          {
            _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_CANCEL, "cancel.png", Undo));
            _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_SAVE, "ok.png", Save));
          }
        }
        if (UseRefresh)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_REFRESH, "refresh.png", RefreshData));

        Toolbar.AddGroup(_toolbarGroup);
      }
      if (BottomToolbar != null && _bottomToolbarGroup == null)
      {
        _bottomToolbarGroup = new ToolbarGroup(10);
        if (UseAdd || UseUpdate)
        {
          _bottomToolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_CANCEL, "cancel.png", Undo));
          _bottomToolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_SAVE, "ok.png", Save));
        }
        BottomToolbar.AddGroup(_bottomToolbarGroup);
        BottomToolbar.HorizontalAlignment = 3; // right
      }
    }

    /// <summary>
    /// Selection changed on the grid: start the detail through the navigation service
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSelectionChangeElapsed(object sender, EventArgs e)
    {
      _selectionChangeTimer.Stop();

      //if (SearchPane.CurrentItem == null) return;
      //{
      //  _navigationService.NavigateToAsync(() => ActiveDetail ?? _detailFactory.CreatePart(),
      //                                     target =>
      //                                     (target as BaseDetailViewModel<TEntity>).Start(
      //                                       (SearchPane.CurrentItem as ListItemBase).Id))
      //    .ContinueWith(navigation => { if (navigation.Cancelled) UpdateCommands(); });
      //}

      NotifyOfPropertyChange(() => CanDelete);
      NotifyOfPropertyChange(() => CanEdit);
    }

    /// <summary>
    /// Current item changed in the grid (start the timer, when elapsed do selectionchanged)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSearchPanePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "CurrentItem") return;

      if (_selectionChangeTimer.IsEnabled) _selectionChangeTimer.Stop();
      _selectionChangeTimer.Start();
    }

    /// <summary>
    /// Deactivation of this program including grid and toolbar
    /// </summary>
    /// <param name="close">indication that program closes</param>
    protected override void OnDeactivate(bool close)
    {
      if (close)
      {
        ActiveItem = null;
        _parentid = 0;
      }
      base.OnDeactivate(close);
      if (SearchPane != null)
      {
        (SearchPane as BaseSearchViewModel<TEntity, TListItem>).PropertyChanged -= OnSearchPanePropertyChanged;
        ((IDeactivate)SearchPane).Deactivate(close);
      }
      else
        (Parent as Screen).TryClose();

      // @@@ JKT anders verwijnt ie bij wisselen menu, TODO wijzigingen in toolbar
      //Toolbar.RemoveGroup(_toolbarGroup);
    }

    protected virtual async Task<DialogResult> CheckEdit()
    {
      return DialogResult.Ignore;
    }

    public async void Edit()
    {
      if ((SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem == null)
        await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_NO_RECORD_AVAILABLE, DialogButtons.Ok);

      _item = (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem;
      _entityid = (_item as ListItemBase).Id;
      
      var result = await CheckEdit();
      if (result != DialogResult.Ignore)
        return;
      
      try
      {

        var detail = ActiveDetail as TDetailViewModel ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(detail.GetType(),
          target =>
          {
            
            (target as BaseDetailViewModel<TEntity>).Parent = this; // @@@@
            (target as BaseDetailViewModel<TEntity>).Start(_entityid);
          });
      }
      catch (TaskCanceledException)
      {
        (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem = null;
        UpdateCommands();

      }
    }

    /// <summary>
    /// Add a new entity by starting the detail viewmodel
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    public virtual async void Add()
    {
      try
      {
        if (ActiveDetail != null)
          (ActiveDetail as BaseDetailViewModel<TEntity>).TryClose();

        (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem = null;

        var detail = ActiveDetail as TDetailViewModel ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(
          detail.GetType(),
          target => (target as BaseDetailViewModel<TEntity>).Start(true, _parentid));
        //target => target.Start(nameEditor.Name))

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }

    /// <summary>
    /// Add a new entity by starting the detail viewmodel
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    public virtual async void Copy()
    {
      if ((SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem == null)
        await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_NO_RECORD_AVAILABLE, DialogButtons.Ok);

      _item = (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem;
      _entityid = (_item as ListItemBase).Id;

      var result = await CheckEdit();
      if (result != DialogResult.Ignore)
        return;
      try
      {
        if (ActiveDetail != null)
          (ActiveDetail as BaseDetailViewModel<TEntity>).TryClose();

        var detail = ActiveDetail as TDetailViewModel ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(
          detail.GetType(),
          target => (target as BaseDetailViewModel<TEntity>).Start(true, _parentid, "COPY", _entityid)
          );
        //target => target.Start(nameEditor.Name))

      }
      catch (TaskCanceledException)
      {
        UpdateCommands();
      }
    }

    /// <summary>
    /// Repository used in the delete operation
    /// </summary>
    /// <param name="unitOfWork">unit of work die gebruikt moet worden</param>
    /// <returns>repository for the entity</returns>
    protected abstract IRepository<TEntity> Repository(IAccoBookingUnitOfWork unitOfWork);

    /// <summary>
    /// Delete the entity in the unit of work
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    public async void Delete()
    {
      ListItemBase listitem = (SearchPane as BaseSearchViewModel<TEntity, TListItem>).CurrentItem as ListItemBase;

      listitem.Label = EntityLabel;

      var dialogresult = await _dialogManager.ShowMessageAsync(string.Format(Resources.AccoBooking.mes_DELETE, EntityLabel), new[] {Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO});
        //DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);
      if (dialogresult == Resources.AccoBooking.but_NO)
        return;

      var unitOfWork = _unitOfWorkManager.Get(listitem.Id);
	  
      try
      {
        using ((SearchPane as BaseSearchViewModel<TEntity, TListItem>).Busy.GetTicket())
        {
          var entity = await Repository(unitOfWork).WithIdFromDataSourceAsync(listitem.Id);

          await OnPreDelete();

          OnDelete(unitOfWork, entity);
          await unitOfWork.CommitAsync();
          await OnPreDelete();
        }
      }
      catch (TaskCanceledException)
      {
        unitOfWork.Rollback();
      }
      catch (Exception)
      {
        unitOfWork.Rollback();
        throw;
      }

    }

    protected virtual void OnDelete(IAccoBookingUnitOfWork unitOfWork, TEntity entity)
    {
      Repository(unitOfWork).Delete(entity);
    }

    public virtual async Task OnSave()
    {
    }

    public virtual async Task OnPostSave(bool isDelete)
    {
    }

    public virtual async Task OnPreDelete()
    {
    }

    public virtual async Task OnPostDelete()
    {
    }

    /// <summary>
    /// Save the entity in the active unit of work
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    public virtual async void Save()
    {
      await SaveAsync(true);
    }

    public virtual async Task SaveAsync(bool returnAfterSave)
    {
      try
      {
        var result = await LocalCheck();
        if (!string.IsNullOrEmpty(result))
        {
          ActiveUnitOfWork.Rollback();
          return;
        }
        await OnSave();

        using ((ActiveDetail as BaseDetailViewModel<TEntity>).Busy.GetTicket())
          await ActiveUnitOfWork.CommitAsync();

        await OnPostSave(false);
        
        if (returnAfterSave)
          await Cancel();

      }
      catch (Exception)
      {

        throw;
      }
    }


    public virtual async Task<string> LocalCheck()
    {
      return "";
    }

    public async  void Undo()
    {
      await Cancel();
    }

    /// <summary>
    /// Cancel the modifictions
    /// </summary>
    public virtual async Task Cancel()
    {

      if (ActiveUnitOfWork.HasChanges())
      {
        var dialogResult =
          await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_CANCEL, new[] { Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO });
          //  DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel);

        if (dialogResult == Resources.AccoBooking.but_YES)
        {
          Busy.AddWatch();

          var shouldClose = ActiveEntity.EntityAspect.EntityState.IsAdded();
          ActiveUnitOfWork.Rollback();

          if (shouldClose)
            (ActiveDetail as BaseDetailViewModel<TEntity>).TryClose();

          if (SearchPane == null & Completed != null)
            Completed(this, new ResultCompletionEventArgs());
          else
          {
            if (_parentid == 0)
              Start();
            else
              Start(_parentid);
          }
        }
      }

      else
      {
        (ActiveDetail as BaseDetailViewModel<TEntity>).TryClose();

        if (SearchPane == null)
        {
          TryClose();
        }
        else
        {
          if (_parentid == 0)
            Start();
          else
            Start(_parentid);
        }
      }
    }


    /// <summary>
    /// Refresh the active detail 
    /// </summary>
    /// <returns>operation result (Coroutine)</returns>
    public void RefreshData()
    {
      (ActiveDetail as BaseDetailViewModel<TEntity>).RefreshData();
    }

    /// <summary>
    /// Update the command buttons
    /// </summary>
    protected virtual void UpdateCommands()
    {
      if (Toolbar != null)
        Toolbar.IsVisible = !IsDetailActive;
      if (BottomToolbar != null)
        BottomToolbar.IsVisible = IsDetailActive;

      NotifyOfPropertyChange(() => IsDetailActive);
      NotifyOfPropertyChange(() => IsSearchActive);

      NotifyOfPropertyChange(() => CanAdd);
      NotifyOfPropertyChange(() => CanCopy);
      NotifyOfPropertyChange(() => CanSave);
      NotifyOfPropertyChange(() => CanEdit);
      NotifyOfPropertyChange(() => CanCancel);
      NotifyOfPropertyChange(() => CanDelete);
      NotifyOfPropertyChange(() => CanRefreshData);
    }
  }
}