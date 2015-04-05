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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel.Projections;
using Security;

namespace DomainServices.Services
{
  public class UserSearchService : IUserSearchService
  {
    private readonly IRepository<User> _repository;

    public UserSearchService(IRepository<User> repository)
    {
      _repository = repository;
    }

    #region IUserSearchService Members

    public Task<IEnumerable<UserListItem>> FindUsersAsync(
      string group, CancellationToken cancellationToken)
    {
      Expression<Func<User, bool>> filter = null;

      return _repository.FindInDataSourceAsync(
        q => q.Select(x => new UserListItem
        {
          UserName = x.Username,

          Label = x.Username
        }),

        cancellationToken, filter, q => q.OrderBy(i => i.UserName));
    }

    #endregion
  }


}