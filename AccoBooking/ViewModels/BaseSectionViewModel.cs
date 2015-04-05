using Caliburn.Micro;
using Cocktail;
#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels
{
  public abstract class BaseSectionViewModel<TEntity, TSectionViewModel> : Conductor<IScreen>.Collection.AllActive, 
                                              IDiscoverableViewModel,
                                              IHarnessAware, 
                                              IBaseDetailSection<TEntity>
    where TEntity : class
    {

    protected BaseSectionViewModel()
    {
      EventFns.Subscribe(this);
    }

    public TSectionViewModel Section { get; set; }

    #region IHarnessAware Members

    public void Setup()
    {
#if HARNESS
            Start(AccoBookingSampleDataProvider.CreateGuid(1));
#endif
    }

    #endregion

    #region IBaseDetailSection Members

    public abstract int Index { get; }

    public bool IsVisible { get; set; }

    void IBaseDetailSection<TEntity>.Start(int entityid)
    {
      Start(entityid);
    }

    void IBaseDetailSection<TEntity>.Start(TEntity entity)
    {
      //Start(entity);
    }

    #endregion

    public virtual BaseSectionViewModel<TEntity, TSectionViewModel> Start(int entityid)
    {
      return this;
    }

    public virtual BaseSectionViewModel<TEntity, TSectionViewModel> Start(TEntity entity)
    {
      return this;
    }
    
  }
}
