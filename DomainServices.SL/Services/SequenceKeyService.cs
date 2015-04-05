using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
  public static class SequenceKeyService
  {
    public static async Task<Sequence> NextValueAsync(string currentsequence, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var entityManagerProvider = new EntityManagerProvider<EntityManager>();
      var unitOfWork = new UnitOfWork<Sequence>(entityManagerProvider);

      var sequence = await unitOfWork.Entities.WithIdFromDataSourceAsync(currentsequence, cancellationToken);
      
      sequence.CurrentId += 1;

      await unitOfWork.CommitAsync();

      return sequence;
    }
  }
}
