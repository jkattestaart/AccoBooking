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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocktail;
using Common.Errors;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using IdeaBlade.EntityModel;
using Security;

namespace AccoBooking.ViewModels.Acco
{
 [Export]
  public class AccoTrusteeManagementViewModel : BaseMasterViewModel<AccoTrusteeSearchViewModel,AccoTrusteeDetailViewModel, AccoTrusteeListItem, AccoTrustee>
                                        
  {
   private ISecurityUnitOfWorkManager<ISecurityUnitOfWork> _securityUnitOfWorkManager;

   [ImportingConstructor]
   public AccoTrusteeManagementViewModel(ExportFactory<AccoTrusteeSearchViewModel> searchFactory,
                                                 ExportFactory<AccoTrusteeDetailViewModel> detailFactory,
                                                 IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                                 ISecurityUnitOfWorkManager<ISecurityUnitOfWork> securityUnitOfWorkManager,
                                                 IErrorHandler errorHandler, IDialogManager dialogManager,
                                                 ToolbarViewModel toolbar,
                                                 ToolbarViewModel bottomToolbar
                                                )
     : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
      _securityUnitOfWorkManager = securityUnitOfWorkManager;
    }

    protected override IRepository<AccoTrustee> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoTrustees;
    }

   public override async Task<string> LocalCheck()
   {
     var securityUnitOfWork = _securityUnitOfWorkManager.Create(); // create a unit of work

     //If the login is changed delete the original user
     if (ActiveEntity.EntityAspect.EntityState == EntityState.Modified)
     {
       var oldlogin = (string)ActiveEntity.EntityAspect.GetValue("Login", EntityVersion.Original);
       var oldUser = await securityUnitOfWork.Users.FindInDataSourceAsync(u => u.Username == oldlogin);
       securityUnitOfWork.Users.Delete(oldUser);
       await securityUnitOfWork.CommitAsync();
     }

     var usr = await securityUnitOfWork.Users.FindInDataSourceAsync(u => u.Username == (ActiveEntity as AccoTrustee).Login);

     if (usr.Count() != 0)
     {
       var dialogresult = await _dialogManager.ShowMessageAsync(string.Format("User not unique"), new[] { Resources.AccoBooking.but_OK });
       return dialogresult;
     }

     return null;
   }

   public override async Task OnSave()
   {
     var securityUnitOfWork = _securityUnitOfWorkManager.Create(); // create a unit of work
     
     var ww = (ActiveEntity as AccoTrustee).Password;

     byte[] hash = CryptoHelper.GenerateKey(ww);
     string password = Encoding.UTF8.GetString(hash, 0, hash.Length);

     using (Busy.GetTicket())
     {
       var user = await securityUnitOfWork.UserFactory.CreateAsync(); // create the entity

       // add the result to the database
       Guid id = user.Id;

       _securityUnitOfWorkManager.Add(id, securityUnitOfWork);
       user.Username = (ActiveEntity as AccoTrustee).Login;
       user.Password = password;

       await securityUnitOfWork.CommitAsync();

     }


   }
  }
}