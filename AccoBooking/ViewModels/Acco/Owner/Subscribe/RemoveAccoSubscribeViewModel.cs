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
using Cocktail;
using Common.Actions;
using DomainModel;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class RemoveAccoSubscribeViewModel : BaseScreen<AccoOwner>
  {
    protected ToolbarGroup _toolbarGroup;
    private readonly ISecurityUnitOfWorkManager<ISecurityUnitOfWork> _securityUnitOfWorkManager;
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public RemoveAccoSubscribeViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      ISecurityUnitOfWorkManager<ISecurityUnitOfWork> securityUnitOfWorkManager,
      ShellViewModel shellViewModel,
      IDialogManager dialogManager
      )
      : base(unitOfWorkManager, dialogManager)
    {
      _securityUnitOfWorkManager = securityUnitOfWorkManager;
      _shellViewModel = shellViewModel;
      _shellViewModel.ExtraMenu.Clear();
      Description = SessionManager.CurrentAcco.Description;
    }

    public string Description { get; set; }

    protected override IRepository<AccoOwner> Repository()
    {
      return UnitOfWork.AccoOwners;
    }

    public async void Remove()
    {
      try
      {

        SessionManager.CurrentOwner =
          await UnitOfWork.AccoOwners.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoOwnerId);

        var accoes = await UnitOfWork.Accoes.FindInDataSourceAsync(ac => ac.AccoOwnerId == SessionManager.CurrentOwner.AccoOwnerId);

        var acco = await UnitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId);

        if (accoes.Count() == 1)
        {
          var dialogresult =
            await
              DialogManager.ShowMessageAsync(Resources.AccoBooking.mes_TERMINATE_SUBSCRIBTION, new[] {Resources.AccoBooking.but_YES, Resources.AccoBooking.but_NO});
              //DialogResult.Yes,
              //DialogResult.None, DialogButtons.YesNo);

          if (dialogresult == Resources.AccoBooking.but_YES)
          {
            using (Busy.GetTicket())
            {
              acco.IsActive = false;

              await UnitOfWork.CommitAsync();
              {
                //TODO: inactivate account
                var securityUnitOfWork = _securityUnitOfWorkManager.Create(); // create a unit of work
                var user = await securityUnitOfWork.Users.WithIdAsync(SessionManager.CurrentOwner.Login);
                user.Username = "$Terminated$" + user.Username;
                await securityUnitOfWork.CommitAsync();
                Busy.RemoveWatch();

              }
            }
            if (dialogresult == Resources.AccoBooking.but_NO)
              UnitOfWork.Rollback();

          }
        }
        else
        {
          using (Busy.GetTicket())
          {
            acco.IsActive = false;

            await UnitOfWork.CommitAsync();

          }

        }

      }
      catch (Exception)
      {

        throw;
      }

      await _shellViewModel.OnLoggedIn();

      //_shellViewModel.Start(true);
    }
  }


}

