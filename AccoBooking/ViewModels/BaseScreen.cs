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
using System.ComponentModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;
using Common.Errors;
using DomainServices;
using IdeaBlade.EntityModel;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels
{
  /// <summary>
  /// Base Caliburn Screen for the detail program
  /// </summary>
  /// <typeparam name="TEntity"></typeparam>
  public abstract class BaseScreen<TEntity> : Screen, IDiscoverableViewModel, IHarnessAware
    where TEntity : class
  {
    protected Entity _entity;
    protected int _entityid;
    protected IAccoBookingUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor for the the detail program
    /// </summary>
    /// <param name="unitOfWorkManager">Unit of work manager used</param>
    /// <param name="errorHandler">Handler for errors</param>
    /// <param name="dialogManager"></param>
    protected BaseScreen(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                         IDialogManager dialogManager)
    {
      EventFns.Subscribe(this);

      DomainUnitOfWorkManager = unitOfWorkManager;
      DialogManager = dialogManager;

      Busy = new BusyWatcher();
    }

    public IBusyWatcher Busy { get; private set; }
  


    /// <summary>
    /// Dialog Manager
    /// </summary>
    public IDialogManager DialogManager { get; private set; }

    /// <summary>
    /// Domain unit of work manager
    /// </summary>
    public IUnitOfWorkManager<IAccoBookingUnitOfWork> DomainUnitOfWorkManager { get; private set; }

    /// <summary>
    /// Error handler
    /// </summary>
    public IErrorHandler ErrorHandler { get; private set; }

    /// <summary>
    /// Unit of work for this screen
    /// </summary>
    protected IAccoBookingUnitOfWork UnitOfWork
    {
      get { return _unitOfWork ?? (_unitOfWork = DomainUnitOfWorkManager.Get(_entityid)); }
    }

    /// <summary>
    /// Entity used in this screen
    /// </summary>
    public virtual Entity Entity
    {
      get { return _entity; }
      set
      {
        if (Entity != null)
        {
          Entity.PropertyChanged -= OnPropertyChanged;
        }
        _entity = value;
        if (_entity != null)
          Entity.PropertyChanged += OnPropertyChanged;
        
        NotifyOfPropertyChange(() => Entity);
      }
    }

    public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => Entity);   
    }

    public bool CanSave
    {
      get
      {
        return Entity != null && UnitOfWork.HasChanges() &&
               !Entity.EntityAspect.EntityState.IsDeleted();
      }
    }

    public virtual Task OnSave()
    {
      return TaskEx.FromResult(true);
    }

    public virtual Task OnPostSave(bool isDelete)
    {
      return TaskEx.FromResult(true);
    }

    public async Task Save()
    {
      try
      {
        await OnSave();

        using (Busy.GetTicket())
          await UnitOfWork.CommitAsync();

        await OnPostSave(false);

        NotifyOfPropertyChange(() => CanSave);
      }

      catch (TaskCanceledException)
      {
        UnitOfWork.Rollback();
      }
      catch (Exception)
      {
        UnitOfWork.Rollback();
        throw;
      }


    }

    #region IHarnessAware Members

    public void Setup()
    {
#if HARNESS
            Start(AccoBookingSampleDataProvider.CreateGuid(1));
#endif
    }

    #endregion

    /// <summary>
    /// Repository used to read the entity
    /// </summary>
    /// <returns>repository</returns>
    protected abstract IRepository<TEntity> Repository();

    /// <summary>
    /// Start the screen
    /// </summary>
    /// <param name="entityid">id to start with</param>
    /// <returns>this program</returns>
    public virtual BaseScreen<TEntity> Start(int entityid)
    {
      LoadDataAsync(entityid);
     
      return this;
    }

    protected async void LoadDataAsync(int entityid) //, EditMode editMode)
    {
      _unitOfWork = null;
      _entityid = entityid;
      //EditMode = editMode;
      Entity = (await Repository().WithIdAsync(entityid)) as Entity;
      if (Entity == null || !Entity.EntityAspect.EntityState.IsAdded())
        LoadDataFromDataSourceAsync(entityid);
    }

    protected async void LoadDataFromDataSourceAsync(int entityid) //, EditMode editMode)
    {
      _unitOfWork = null;
      _entityid = entityid;
      //EditMode = editMode;
      Entity = (await Repository().WithIdFromDataSourceAsync(entityid)) as Entity;
    }

    /// <summary>
    /// Start screen, entity is passed by parent program
    /// Example: Relation from Customer
    /// </summary>
    /// <param name="entity">entity passes</param>
    /// <returns>this program</returns>
    public virtual BaseScreen<TEntity> Start(Entity entity)
    {
      Entity = entity;
      _entityid = (int)entity.EntityAspect.EntityKey.Values[0];
      return this;
    }

    /// <summary>
    /// Deactivate, reset entiry en unit of work
    /// </summary>
    /// <param name="close"></param>
    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);

      if (close)
      {
        Entity = null;
        _unitOfWork = null;
      }
    }

  }
}