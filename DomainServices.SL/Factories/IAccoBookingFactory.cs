using System.Threading;
using Cocktail;
using IdeaBlade.EntityModel;
using System.Linq;
using System.Threading.Tasks;

#if NETFX_CORE
using IdeaBlade.Core.Reflection;
#endif

namespace DomainServices.Factories
{
  public interface IAccoBookingFactory<TEntity> : IFactory<TEntity> where TEntity : class
  {
    Task<TEntity> CreateCopyAsync(CancellationToken cancellationToken, int entityid);
  }

  //public class AccoBookingFactory<TEntity> : Factory<TEntity> where TEntity : class, IAccoBookingFactory<TEntity>
  //{   
  //  public AccoBookingFactory(IEntityManagerProvider entityManagerProvider): base(entityManagerProvider)
  //  {

  //  }

  //  /// <returns>The newly created entity attached to the underlying EntityManager.</returns>
  //  public Task<TEntity> CreateCopyAsync()
  //  {
  //    return CreateCopyAsync(CancellationToken.None);
  //  }

  //  public async virtual Task<TEntity> CreateCopyAsync(CancellationToken cancellationToken)
  //  {
  //    cancellationToken.ThrowIfCancellationRequested();

  //    var instance = await Task.Factory.StartNew(() => CreateInstance(), cancellationToken);
  //    EntityManager.AddEntity(instance);

  //    return instance;
  //  }
  //}

  public static class AccoBookingFactory
  {
    public static void Clone(Entity source, Entity destination)
    {
      //clone the entity (except the primary key)
      foreach (var p in source.EntityAspect.EntityMetadata.DataProperties.Where(x=>!x.IsPartOfKey))
      {
        var dp = destination.EntityAspect.EntityMetadata.DataProperties.FirstOrDefault(e => e.Name == p.Name);
        dp.SetValue(destination, p.GetValue(source, EntityVersion.Current));

      }
    }

  }
}
