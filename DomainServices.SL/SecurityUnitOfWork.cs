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

using System.ComponentModel.Composition;
using Cocktail;
using DomainServices.Factories;
using DomainServices.Repositories;
using DomainServices.Services;
using Security;

namespace DomainServices
{
  [Export(typeof (ISecurityUnitOfWork)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class SecurityUnitOfWork : UnitOfWork, ISecurityUnitOfWork
  {
    [ImportingConstructor]
    public SecurityUnitOfWork(
      [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<SecurityEntities>
        entityManagerProvider,
      [Import(AllowDefault = true)] IGlobalCache globalCache = null)
      : base(entityManagerProvider)
    {

      UserFactory = new UserFactory(entityManagerProvider);
      Users = new UserRepository(entityManagerProvider);
      UserSearchService = new UserSearchService(Users);
    }

    #region ISecurityUnitOfWork Members

    //public bool HasEntity(object entity)
    //{
    //  var entityAspect = EntityAspect.Wrap(entity);
    //  return EntityManager == entityAspect.EntityManager;
    //}

    public IFactory<User> UserFactory { get; private set; }
    public IRepository<User> Users { get; private set; }
    public IUserSearchService UserSearchService { get; private set; }

    //public void Clear()
    //{
    //  EntityManager.Clear();
    //}
                                                  
    #endregion
  }
}