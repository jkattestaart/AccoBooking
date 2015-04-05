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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using DomainServices;
using DomainServices.Factories;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels
{
  /// <summary>
  /// Base class for standard maintenance programs (Caliburn Conductor for IScreen)
  /// </summary>
  /// <typeparam name="TEntity">Entity for this program</typeparam>
  public abstract class BaseDetailViewModel<TEntity> : Conductor<IScreen>.Collection.OneActive,
    IDiscoverableViewModel,
    IHarnessAware
    where TEntity : class
  {
    protected readonly IDialogManager _dialogManager;
    
    protected IEnumerable<IBaseDetailSection<TEntity>> _sections;
    protected readonly IUnitOfWorkManager<IAccoBookingUnitOfWork> _unitOfWorkManager;
    protected Entity _entity;
    protected int _entityid;
    protected IAccoBookingUnitOfWork _unitOfWork;
    protected bool _add;

    /// <summary>
    /// Constructor for the update program
    /// Can contain sections (tab items) and or a summary (detail)
    /// </summary>
    /// <param name="unitOfWorkManager">Unit of Work manager</param>
    /// <param name="sections">Sections used</param>
    /// <param name="summary">Summary program used</param>
    /// <param name="dialogManager">Dialog manager</param>
    protected BaseDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      IEnumerable<IBaseDetailSection<TEntity>> sections,
      BaseScreen<TEntity> summary,
      IDialogManager dialogManager)
    {
      Summary = summary;
      _unitOfWorkManager = unitOfWorkManager;
     _dialogManager = dialogManager;
      _sections = sections;
      Busy = new BusyWatcher();

      PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Summary (detail) placeholder
    /// </summary>
    public BaseScreen<TEntity> Summary { get; protected set; }

    /// <summary>
    /// Busy indicator
    /// </summary>
    public IBusyWatcher Busy { get; private set; }

    /// <summary>
    /// Indicator if this program sould be visible
    /// </summary>
    public bool Visible
    {
      get { return Entity != null; }
    }

    /// <summary>
    /// Unit of work for this program
    /// </summary>
    public IAccoBookingUnitOfWork UnitOfWork
    {
      get { return _unitOfWork ?? (_unitOfWork = _unitOfWorkManager.Get(_entityid)); }
    }

    /// <summary>
    /// The index of the sections that is active
    /// </summary>
    public int ActiveSectionIndex
    {
      get { return Items.IndexOf(ActiveItem); }
      set { ActivateItem(Items[Math.Max(value, 0)]); }
    }

    /// <summary>
    /// The entity for this program
    /// </summary>
    public Entity Entity
    {
      get { return _entity; }
      set
      {
        _entity = value;
        NotifyOfPropertyChange(() => Entity);
        NotifyOfPropertyChange(() => Visible);
      }
    }

    #region IHarnessAware Members

    public void Setup()
    {
#if HARNESS
  //Start("John", "M.", "Doe");
            Start(AccoBookingSampleDataProvider.CreateGuid(1));
#endif
    }

    #endregion

    /// <summary>
    /// When active item changes notifty the active section

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ActiveItem")
        NotifyOfPropertyChange(() => ActiveSectionIndex);
    }

    /// <summary>
    /// Start this program from the add command
    /// </summary>
    /// <param name="entityid">id of the current entity</param>
    /// <returns>this program</returns>
    public BaseDetailViewModel<TEntity> StartCreate(int entityid)
    {
      LoadDataAsync(entityid);
      return this;
    }

    /// <summary>
    /// Start this program 
    /// </summary>
    /// <param name="entityid">id of the current entity</param>
    /// <returns>this program</returns>
    public virtual BaseDetailViewModel<TEntity> Start(int entityid)
    {
      LoadDataAsync(entityid);
      return this;
    }

    /// <summary>
    /// Repository used to read the entity
    /// </summary>
    /// <returns>repository</returns>
    protected abstract IRepository<TEntity> Repository();

    /// <summary>
    /// Factory used to create the entity
    /// </summary>
    /// <param name="unitOfWork">Unit of work for this factory</param>
    /// <returns>factory</returns>
    protected abstract IFactory<TEntity> Factory(IAccoBookingUnitOfWork unitOfWork);

    /// <summary>
    /// Extra when entity is created
    /// </summary>
    /// <param name="entity">entity created</param>
    /// <param name="parentid">id of the parent entity</param>
    protected virtual void OnCreateEntity(TEntity entity, int parentid)
    {
    }

    private async void LoadDataAsync(int entityid)
    {
      using (Busy.GetTicket())
      {
        _unitOfWork = null;
        _entityid = entityid;
        //EditMode = editMode;

        Entity = (await Repository().WithIdAsync(entityid)) as Entity;

        if (Summary != null)
          Summary.Start(entityid);

        if (_sections == null)
          return;

        _sections.ForEach(s => s.IsVisible = true); //voorlopig alles tonen
        _sections.ForEach(s => s.Start(entityid));

        if (Items.Count == 0)
        {
          Items.AddRange(_sections.OrderBy(s => s.Index));
          NotifyOfPropertyChange(() => Items);
          ActivateItem(Items.First());
        }
      }
    }

    private async void CreateDataAsync(int parentid, string action, int entityid)
    {
      using (Busy.GetTicket())
      {
        _unitOfWork = _unitOfWorkManager.Create();
        Entity entity;

        //NB zorg ervoor dat die factory IAccoBookingFactory heeft
        if (action == "COPY")
        {
          //load the entity in cache for the copy operation
          await Repository().WithIdFromDataSourceAsync(entityid);
          entity = (await (Factory(_unitOfWork) as IAccoBookingFactory<TEntity>).CreateCopyAsync(CancellationToken.None, entityid) as Entity);         
        }
        else
          entity = (await Factory(_unitOfWork).CreateAsync() as Entity);

        int id = (int)entity.EntityAspect.EntityKey.Values[0];
        if (Parent is IBaseMasterDetailViewModel)
          (Parent as IBaseMasterDetailViewModel).EntityId = id;
        _unitOfWorkManager.Add(id, _unitOfWork);
        OnCreateEntity((entity as TEntity), parentid);
        StartCreate(id);

      }
    }

    /// <summary>
    /// Start the program
    /// </summary>
    /// <param name="fromParent">start the detail from a parent entity</param>
    /// <param name="parentid">id of the parent entity</param>
    /// <param name="action">action: ADD, COPY</param>
    /// <returns></returns>
    public virtual BaseDetailViewModel<TEntity> Start(bool fromParent, int parentid)
    {
      CreateDataAsync(parentid, "", 0);
 
      return this;
    }

    /// <summary>
    /// Start the program
    /// </summary>
    /// <param name="fromParent">start the detail from a parent entity</param>
    /// <param name="parentid">id of the parent entity</param>
    /// <param name="action">action: ADD, COPY</param>
    /// <returns></returns>
    public virtual BaseDetailViewModel<TEntity> Start(bool fromParent, int parentid, string action, int entityid)
    {
      CreateDataAsync(parentid, action, entityid);

      return this;
    }

    /// <summary>
    /// Start screen, entity is passed by parent program
    /// Example: Relation from Customer
    /// </summary>
    /// <param name="entity">entity passed</param>
    /// <param name="unitOfWork">unit of work to be used</param>
    /// <returns>this program</returns>
    public virtual BaseDetailViewModel<TEntity> Start(Entity entity, IAccoBookingUnitOfWork unitOfWork)
    {
      return this;
    }

    /////// <summary>
    /////// When start is completed, start the detail and/or tabs
    /////// </summary>
    /////// <param name="entity"></param>
    ////protected virtual void OnStartCompleted(TEntity entity)
    ////{
    ////  Entity = entity as Entity;

    ////  int id = (int)Entity.EntityAspect.EntityKey.Values[0];

    ////  if (Summary != null)
    ////    Summary.Start(id);

    ////  if (_sections == null)
    ////    return;

    ////  _sections.ForEach(s => s.IsVisible = true); //voorlopig alles tonen
    ////  _sections.ForEach(s => s.Start(id));

    ////  if (Items.Count == 0)
    ////  {
    ////    Items.AddRange(_sections.OrderBy(s => s.Index).Cast<IScreen>());
    ////    NotifyOfPropertyChange(() => Items);
    ////    ActivateItem(Items.First());
    ////  }
    ////}

    /////// <summary>
    /////// When start is completed, start the detail and/or tabs
    /////// </summary>
    /////// <param name="entity"></param>
    ////protected virtual void OnStartCreateCompleted(TEntity entity)
    ////{
    ////  Entity = entity as Entity;

    ////  int id = (int)Entity.EntityAspect.EntityKey.Values[0];

    ////  if (Summary != null)
    ////    Summary.Start(id);

    ////  if (_sections == null)
    ////    return;

    ////  // @@@ JKT proberen om niet alle scherm starten omdat die bij create niet valid zijn
    ////  _sections.ForEach(s => s.IsVisible=true);

    ////  _sections.Where(s => s.Index == 10).ForEach(s => s.Start(id));
    ////  //_sections.Where(s => s.Index > 10).ForEach(s =>
    ////  //  {

    ////  //    if (s.IsActive) s.TryClose();
    ////  //  }
    ////  //  );

    ////  if (Items.Count == 0)
    ////  {
    ////    Items.AddRange(_sections.Where(s=>s.Index==10).OrderBy(s => s.Index).Cast<IScreen>());
    ////    NotifyOfPropertyChange(() => Items);
    ////    ActivateItem(Items.First());
    ////  }
    ////}

    /// <summary>
    /// Extra activaton for detail
    /// </summary>
    protected override void OnActivate()
    {
      base.OnActivate();
      if (Summary != null)
        ((IActivate) Summary).Activate();
      //if (_sections != null)
      //  _sections.ForEach(s => s.Activate());
    }

    /// <summary>
    /// Deactivation of this program 
    /// </summary>
    /// <param name="close">indication that program closes</param>
    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      if (Summary != null)
        ((IDeactivate) Summary).Deactivate(close);

      //if (_sections != null)
      //  _sections.ForEach(s => ((IDeactivate) s).Deactivate(close));
      if (close)
      {
        Entity = null;
        _unitOfWork = null;
      }
    }

    /// <summary>
    /// Verify if the program can be closed, if ok roll back
    /// </summary>
    /// <param name="callback"></param>
    public override async void CanClose(Action<bool> callback)
    {
      try
      {

        if (UnitOfWork.HasChanges())
        {
          var dialogResult = await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_SAVE_CHANGES, new[] {Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO, Resources.AccoBooking.but_CANCEL});
              //DialogResult.Yes, DialogResult.Cancel, DialogButtons.YesNoCancel);

          if (dialogResult == Resources.AccoBooking.but_CANCEL)
            return;

          using (Busy.GetTicket())
          {
            if (dialogResult == Resources.AccoBooking.but_YES)
              await UnitOfWork.CommitAsync();

            if (dialogResult == Resources.AccoBooking.but_NO)
              UnitOfWork.Rollback();

            callback(true);
          }
        }
        else
          base.CanClose(callback);
      }
      catch (TaskCanceledException)
      {
        callback(false);
      }
      catch (Exception)
      {
        callback(false);
        throw;
      }
    }

    /// <summary>
    /// Refresh the data, start the program again
    /// </summary>
    /// <returns></returns>
    public async void RefreshData()
    {
      if (UnitOfWork.HasChanges())
      {
        var dialogresult = await _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_DISCARD_CHANGES,
              new[] {Resources.AccoBooking.but_OK, Resources.AccoBooking.but_CANCEL});
        //DialogButtons.OkCancel);
        if (dialogresult  == Resources.AccoBooking.but_CANCEL)
          return;
      }
      UnitOfWork.Clear();
      int id = (int)Entity.EntityAspect.EntityKey.Values[0];
      Start(id);
    }
  }

}
