//TODO ActiveEntity toevoegen ...

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
  /// <summary>
  /// Base class for standard maintenance programs (Caliburn Conductor for IScreen)
  /// </summary>
  /// <typeparam name="TDetailViewModel">Detail ViewModel used</typeparam>
  /// <typeparam name="TListItem">Listitem in the grid</typeparam>
  /// <typeparam name="TEntity">Entity for this program</typeparam>
  public abstract class BaseManagementViewModel<TDetailViewModel, TListItem, TEntity> : Conductor<IScreen>,
                                                                                        IDiscoverableViewModel,
                                                                                        IHandle<EntityChangedMessage>
                                                                                        
    where TDetailViewModel : class
    where TEntity : class
    where TListItem : class
  {
    protected readonly ExportFactory<TDetailViewModel> _detailFactory;
    protected readonly IDialogManager _dialogManager;
    protected readonly INavigator _navigator;
    protected readonly DispatcherTimer _selectionChangeTimer;
    protected readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    protected IScreen _retainedActiveItem;
    protected ToolbarGroup _toolbarGroup;
    protected int _parentid;
    private TListItem _item;
    protected ToolbarGroup _bottomToolbarGroup;
    private bool _useAdd = true;
    private bool _useUpdate = true;
    private bool _useRefresh;
    private bool _useDelete = true;

    /// <summary>
    /// Constructor for the maintenance program
    /// </summary>
    /// <param name="searchPane">Grid ViewModel</param>
    /// <param name="detailFactory">Factory created for Detail ViewModel part</param>
    /// <param name="unitOfWorkManager">Unit of Work manager</param>
    /// <param name="errorHandler">Handler for errors</param>
    /// <param name="dialogManager">Dialog manager</param>
    /// <param name="toolbar">Toolbar manager</param>
    /// <param name="bottomToolbar">Toolbar manager</param>
    protected BaseManagementViewModel(BaseSearchViewModel<TEntity, TListItem> searchPane,
                                      ExportFactory<TDetailViewModel> detailFactory,
                                      IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                      IDialogManager dialogManager,
                                      ToolbarViewModel toolbar,
                                      ToolbarViewModel bottomToolbar
                                     )
    {
      EventFns.Subscribe(this);

      SearchPane = searchPane;
      _detailFactory = detailFactory;
      _unitOfWorkManager = unitOfWorkManager;
      _dialogManager = dialogManager;
      Toolbar = toolbar;
      BottomToolbar = bottomToolbar;

      _navigator = new Navigator(this);

      PropertyChanged += OnPropertyChanged;

      _selectionChangeTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 200)};
      _selectionChangeTimer.Tick += OnSelectionChangeElapsed;
    }

    public void DoubleClicked()
    {
    
    }

    protected virtual string EntityLabel { get { return (SearchPane.CurrentItem as ListItemBase).Label; } }


    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public ToolbarViewModel Toolbar { get; private set; }

    /// <summary>
    /// Toolbar placeholder
    /// </summary>
    public ToolbarViewModel BottomToolbar { get; set; }

    public bool UseAdd
    {
      get { return _useAdd; }
      set { _useAdd = value; }
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
    public BaseSearchViewModel<TEntity, TListItem> SearchPane { get; set; }

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
    protected TDetailViewModel ActiveDetail
    {
      get { return (TDetailViewModel) ActiveItem; }
    }

    /// <summary>
    /// Active Entity
    /// </summary>
    private Entity ActiveEntity
    {
      get { return ActiveDetail != null ? (ActiveDetail as BaseDetailViewModel<TEntity>).Entity : null; }
    }

    /// <summary>
    /// Deletes enabled when there is a item selected in the grid
    /// </summary>
    public bool CanDelete
    {
      get { return SearchPane.CurrentItem != null; }
    }

    /// <summary>
    /// Save enabled when there is an active entity and there are changes in the active unit of work
    /// Not valid if active entity is deleted
    /// </summary>
    public bool CanSave
    {
      get
      {
        return ActiveEntity != null && ActiveUnitOfWork.HasChanges() &&
               !ActiveEntity.EntityAspect.EntityState.IsDeleted();
      }
    }

    /// <summary>
    /// Cancel enabled like save
    /// </summary>
    public bool CanCancel
    {
      get { return CanSave; }
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
    public BaseManagementViewModel<TDetailViewModel, TListItem, TEntity> Start(int parentid)
    {
      _parentid = parentid;
      Start();
      ((IActivate) this).Activate();
      return this;
    }

    /// <summary>
    /// Start this program without a parent
    /// </summary>
    /// <returns>this program</returns>
    public virtual BaseManagementViewModel<TDetailViewModel, TListItem, TEntity> Start()
    {
      if (_parentid == 0)
        SearchPane.Start();
      else
        SearchPane.Start(_parentid);
      Toolbar.IsVisible = true;
      if (BottomToolbar != null)
        BottomToolbar.IsVisible = true;
      return this;
    }
    /// <summary>
    /// Activating the program by starting the program and activating the grid
    /// </summary>
    protected override void OnActivate()
    {
      base.OnActivate();

      Start();
      SearchPane.PropertyChanged += OnSearchPanePropertyChanged;
      ((IActivate) SearchPane).Activate();

      if (_toolbarGroup == null)
      {
        _toolbarGroup = new ToolbarGroup(10);
        if (UseAdd)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_ADD, "add.png", Add));
        if (UseDelete)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_DELETE, "remove.png", Delete));
      
        if (BottomToolbar == null)
        {
          if (UseAdd || UseUpdate)
          {
            _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_CANCEL, "cancel.png", Cancel));
            _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_SAVE, "ok.png", Save));
          }
        }
        if (UseRefresh)
          _toolbarGroup.Add(new ToolbarAction(this, Resources.AccoBooking.but_REFRESH, "refresh.png", RefreshData));
           
        Toolbar.AddGroup(_toolbarGroup);

        if (BottomToolbar != null)
        {
          if (_bottomToolbarGroup == null)
          {
            _bottomToolbarGroup = new ToolbarGroup(10)
              {
                new ToolbarAction(this, Resources.AccoBooking.but_CANCEL, "cancel.png", Cancel),
                new ToolbarAction(this, Resources.AccoBooking.but_SAVE, "ok.png", Save),
                //new ToolbarAction(this, "Refresh", "refresh.png", RefreshData)
              };
          }
          BottomToolbar.AddGroup(_bottomToolbarGroup);
          BottomToolbar.HorizontalAlignment = 3; // right
        }
      }
    }

    /// <summary>
    /// Selection changed on the grid: start the detail through the navigation service
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSelectionChangeElapsed(object sender, EventArgs e)
    {
      _selectionChangeTimer.Stop();

      if (SearchPane.CurrentItem == null) return;

      try
      {

        //if (ActiveStaffingResource != null && ActiveStaffingResource.Id == SearchPane.CurrentStaffingResource.Id)
        //  return;
        var detail = ActiveDetail ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(detail.GetType(),
          target =>
          {
            _item = SearchPane.CurrentItem;
            (target as BaseDetailViewModel<TEntity>).Start((SearchPane.CurrentItem as ListItemBase).Id);
          });
      
      }
      catch (TaskCanceledException)
      {
        SearchPane.CurrentItem = null;
        UpdateCommands();
      }
       
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
      SearchPane.PropertyChanged -= OnSearchPanePropertyChanged;
      ((IDeactivate) SearchPane).Deactivate(close);
      // @@@ JKT anders verwijnt ie bij wisselen menu, TODO wijzigingen in toolbar
      //Toolbar.RemoveGroup(_toolbarGroup);
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

        SearchPane.CurrentItem = null;

        var detail = ActiveDetail ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(
          detail.GetType(),
          target => (target as BaseDetailViewModel<TEntity>).Start(true, _parentid)
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
      ListItemBase listitem = SearchPane.CurrentItem as ListItemBase;

      listitem.Label = EntityLabel;

      var dialogresult = await _dialogManager.ShowMessageAsync(string.Format(Resources.AccoBooking.mes_DELETE, EntityLabel), new[] {Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO});
        //DialogResult.Yes, DialogResult.No, DialogButtons.YesNo);

      if (dialogresult == Resources.AccoBooking.but_YES)
      try
      {
        using ((ActiveDetail as BaseDetailViewModel<TEntity>).Busy.GetTicket())
        {
          var entity = await Repository(ActiveUnitOfWork).WithIdFromDataSourceAsync(listitem.Id);

          await OnPreDelete();
          OnDelete(entity);
          await ActiveUnitOfWork.CommitAsync();

          await OnPreDelete();

          if (ActiveEntity != null && (int) ActiveEntity.EntityAspect.EntityKey.Values[0] == listitem.Id)
            ActiveItem.TryClose();
        }
      }
      catch (TaskCanceledException)
      {
        ActiveUnitOfWork.Rollback();
      }
      catch (Exception)
      {
        ActiveUnitOfWork.Rollback();
        throw;
      }

    }

    protected virtual void OnDelete(TEntity entity)
    {
      Repository(ActiveUnitOfWork).Delete(entity);
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
    public async void Save()
    {
      await SaveAsync();
    }

    public async Task SaveAsync()
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
        
        //Cancel();

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

    /// <summary>
    /// Cancel the modifictions
    /// </summary>
    public void Cancel()
    {
      var shouldClose = ActiveEntity.EntityAspect.EntityState.IsAdded();
      ActiveUnitOfWork.Rollback();

      if (shouldClose)
        (ActiveDetail as BaseDetailViewModel<TEntity>).TryClose();
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
    protected void UpdateCommands()
    {
      NotifyOfPropertyChange(() => CanSave);
      NotifyOfPropertyChange(() => CanCancel);
      NotifyOfPropertyChange(() => CanDelete);
      NotifyOfPropertyChange(() => CanRefreshData);
    }
  }
}