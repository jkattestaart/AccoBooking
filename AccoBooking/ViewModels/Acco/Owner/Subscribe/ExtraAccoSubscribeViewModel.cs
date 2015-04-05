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
using Cocktail;
using Common.Actions;
using DomainModel;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class ExtraAccoSubscribeViewModel : BaseScreen<AccoOwner>
  {
    protected ToolbarGroup _toolbarGroup;
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public ExtraAccoSubscribeViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
      ShellViewModel shellViewModel,
      IDialogManager dialogManager
      )
      : base(unitOfWorkManager, dialogManager)
    {
      _shellViewModel = shellViewModel;
      _shellViewModel.ExtraMenu.Clear();

    }

    public string Description { get; set; }

    protected override IRepository<AccoOwner> Repository()
    {
      return UnitOfWork.AccoOwners;
    }

    public async void Subscribe()
    {

      try
      {

        using (Busy.GetTicket())
        {
          _unitOfWork = DomainUnitOfWorkManager.Create();

          SessionManager.CurrentOwner =
            await UnitOfWork.AccoOwners.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoOwnerId);
          SessionManager.CurrentAcco =
            await UnitOfWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId);

          var acco = await UnitOfWork.AccoFactory.CreateAsync();

          // add the result to the database
          int id = acco.AccoId;
          DomainUnitOfWorkManager.Add(id, UnitOfWork);

          acco.AccoOwnerId = SessionManager.CurrentOwner.AccoOwnerId;

          acco.Description = Description;
          acco.Currency = SessionManager.CurrentAcco.Currency;
          acco.ArriveAfter = SessionManager.CurrentAcco.ArriveAfter;
          acco.DepartureBefore = SessionManager.CurrentAcco.DepartureBefore;
          acco.IsActive = true;
          acco.LicenceExpiration = DateTime.Now.AddDays(31);
          acco.DisplaySequence = 99;

          await UnitOfWork.CommitAsync();

          var language = SessionManager.CurrentOwner.Language.Description;
          string subscribe = "NL";

          //taalvolgorde: Nedelands/Dutch/Hollandisch = NL, DE, EN
          //taalvolgorde: Duits/German/Deutsch        = DE, NL, EN
          //taalvolgorde: Engels/English/Englisch     = EN, DE, NL
          if (language == "Nederlands" | language == "Dutch" | language == "Niederlandisch")
            subscribe = "NL";
          if (language == "Engels" | language == "English" | language == "Englisch")
            subscribe = "EN";
          if (language == "Duits" | language == "German" | language == "Deutsch")
            subscribe = "DE";

          await DuplicateAccoService.ExecuteAsync(SessionManager.CurrentAcco.AccoId, acco.AccoId, subscribe);

          //await BuildMailTemplates.ExecuteAsync(acco.AccoId, language);

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