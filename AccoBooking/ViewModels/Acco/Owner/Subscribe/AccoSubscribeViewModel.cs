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
using System.Globalization;
using System.Text;
using AccoBooking.ViewModels.General;
using Cocktail;
using Common.Actions;
using DomainModel;
using DomainServices;
using DomainServices.Services;
using IdeaBlade.EntityModel;
using Security;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class AccoSubscribeViewModel : BaseScreen<AccoOwner>
  {
    private readonly IAuthenticationService _authenticationService;
    protected ToolbarGroup _toolbarGroup;
    private ISecurityUnitOfWorkManager<ISecurityUnitOfWork> _securityUnitOfWorkManager;
    private ShellViewModel _shellViewModel;

    [ImportingConstructor]
    public AccoSubscribeViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                  IAuthenticationService authenticationService,
                                  ISecurityUnitOfWorkManager<ISecurityUnitOfWork> securityUnitOfWorkManager,
                                  SupportedLanguageListViewModel languageList,
                                  ShellViewModel shellViewModel,
                                  IDialogManager dialogManager
                                 )
      : base(unitOfWorkManager, dialogManager)
    {
      _shellViewModel = shellViewModel;
      _shellViewModel.ExtraMenu.Clear();

      _authenticationService = authenticationService;
      _securityUnitOfWorkManager = securityUnitOfWorkManager;
      LanguageList = languageList;

    }

    public SupportedLanguageListViewModel LanguageList { get; set; }

    public string Email { get; set; }
    public string Password { get; set; }

    protected override IRepository<AccoOwner> Repository()
    {
      return UnitOfWork.AccoOwners;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      LanguageList.Start(0);
      if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "nl" |
          CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "be")
        LanguageList.Description = "Nederlands";
      else
        LanguageList.Description = "English";
    }

    public override BaseScreen<AccoOwner> Start(int entityid)
    {
      //return base.Start(entityid);
      return this;
    }

    public async void Subscribe()
    {
      try
      {
        using (Busy.GetTicket())
        {
          var credential = new LoginCredential("guest", "guest", null);
          await _authenticationService.LoginAsync(credential);

          _unitOfWork = DomainUnitOfWorkManager.Create(); // create a unit of work


          var owner = await _unitOfWork.AccoOwnerFactory.CreateAsync(); // create the entity

          // add the result to the database
          DomainUnitOfWorkManager.Add(owner.AccoOwnerId, _unitOfWork);
          owner.Login = Email;
          owner.Password = Password;
          owner.Email = Email;
          Entity = owner;


          var acco = await _unitOfWork.AccoFactory.CreateAsync();

          // add the result to the database
          DomainUnitOfWorkManager.Add(acco.AccoId, _unitOfWork);
          acco.AccoOwnerId = owner.AccoOwnerId;
          acco.DisplaySequence = 10;

          var language = LanguageList.Description;
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

          owner.Name = "Acco eigenaar";
          owner.Login = Email;
          owner.Password = Password;
          owner.Email = Email;

          owner.LanguageId = 1;
          if (subscribe == "EN")
            owner.LanguageId = 2;
          if (subscribe == "DE")
            owner.LanguageId = 3;

          if (subscribe == "EN")
            owner.Name = "Acco owner";
          if (subscribe == "DE")
            owner.Name = "Acco Eigentümer";
          owner.PublicName = owner.Name;
          owner.PublicEmail = owner.Email;


          acco.Currency = "EURO";
          if (subscribe == "EN")
            acco.Currency = "GBP";

          acco.AccoType = "HOLIDAYHOME";
          acco.Deposit = 200;
          acco.CancelAdministrationCosts = 15;
          acco.ArriveAfter = "15:00";
          acco.DepartureBefore = "10:00";
          acco.DaysToPayDepositBackAfterDeparture = 7;
          acco.DaysToExpire = 14;
          acco.StartWeekdayCalendar = "SATURDAY";
          acco.IsActive = true;
          acco.LicenceExpiration = DateTime.Now.AddDays(31);
          acco.SendWeeklyReminders = false;

          acco.AccoOwner = owner;

          await UnitOfWork.CommitAsync();

          await AccoSubscribeService.ExecuteAsync(acco.AccoId, subscribe);

          //await BuildMailTemplates.ExecuteAsync(acco.AccoId, LanguageList.Description);

          //if (LanguageList.Description == "Nederlands" | LanguageList.Description == "Dutch")
          //  SubscribeNederlands(acco);
          //else
          //  SubscribeEngels(acco);

          var ww = Password;

          byte[] hash = CryptoHelper.GenerateKey(ww);
          string password = Encoding.UTF8.GetString(hash, 0, hash.Length);

          //var encryptedPassword = Encoding.UTF8.GetString(CryptoHelper.GenerateKey(Password), 0, Password.Length);


          var securityUnitOfWork = _securityUnitOfWorkManager.Create(); // create a unit of work

          var user = await securityUnitOfWork.UserFactory.CreateAsync(); // create the entity

          // add the result to the database
          Guid id = user.Id;

          _securityUnitOfWorkManager.Add(id, securityUnitOfWork);
          user.Username = Email;
          user.Password = password; //    SetPassword(Password);

          await securityUnitOfWork.CommitAsync();

          await UnitOfWork.CommitAsync();


          credential = new LoginCredential(Email, Password, null);
          await _authenticationService.LoginAsync(credential);
        }

      }

      catch (Exception)
      {

        throw;
      }

      //wordt door event op loggedin al gedaan!
      //await _shellViewModel.OnLoggedIn();

    }

  }
}