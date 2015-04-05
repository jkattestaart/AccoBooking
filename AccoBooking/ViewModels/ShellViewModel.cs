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
using System.ComponentModel.Composition;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccoBooking.ViewModels.Acco;
using AccoBooking.Views;
using Caliburn.Micro;
using Cocktail;
using Common.Actions;
using Common.Workspace;
using DomainModel;
using DomainServices.Services;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using Security.Messages;
using AccoBooking.ViewModels.Login;

namespace AccoBooking.ViewModels
{
  [Export]
  public class ShellViewModel : Conductor<object>, IDiscoverableViewModel, IHarnessAware,
    IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
  {
    private readonly IAuthenticationService _authenticationService;
    private readonly ExportFactory<LoginViewModel> _loginFactory;
    private readonly INavigator _navigator;
    private readonly IEnumerable<IWorkspace> _workspaces;
    private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
    private string savedUser;
    private string savedPassword;

    [ImportingConstructor]
    public ShellViewModel([ImportMany] IEnumerable<IWorkspace> workspaces,
      AccoListViewModel accoList,
      LoginViewModel login,
      MenuViewModel menu,
      MenuViewModel extramenu,
      IAuthenticationService authenticationService,
      ExportFactory<LoginViewModel> loginFactory)
    {
      EventFns.Subscribe(this);

      SessionManager.UserName = "guest";
      Menu = menu;
      ExtraMenu = extramenu;
      DockedLogin = login;
      AccoList = accoList;
      AccoList.PropertyChanged += AccoListPropertyChanged;

      _workspaces = workspaces;
      _authenticationService = authenticationService;
      _loginFactory = loginFactory;
      _navigator = new Navigator(this);

      appSettings.TryGetValue("AccoUser", out savedUser);
      appSettings.TryGetValue("AccoPassword", out savedPassword);
    }

    public string Facebook
    {
      get
      {
        return
          "http://www.facebook.com/plugins/like.php?href=http%3A%2F%2Faccobooking.com&width=48&layout=standard&action=like&show_faces=true&share=true&height=80";
      }
    }

    public string Twitter
    {
      get { return "twitter.html"; }
    }

    private async void AccoListPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ItemId")
      {
        var entityManagerProvider = new EntityManagerProvider<EntityManager>();
        var unitOfWorkAcco = new UnitOfWork<DomainModel.Acco>(entityManagerProvider);
        var unitOfWorkOwner = new UnitOfWork<AccoOwner>(entityManagerProvider);

        SessionManager.CurrentAcco = await unitOfWorkAcco.Entities.WithIdFromDataSourceAsync(AccoList.ItemId);

        SessionManager.CurrentOwner =
          await unitOfWorkOwner.Entities.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoOwnerId);
        Start(false);
      }
    }

    public AccoListViewModel AccoList { get; set; }
    public MenuViewModel Menu { get; private set; }
    public MenuViewModel ExtraMenu { get; private set; }
    public LoginViewModel DockedLogin { get; private set; }

    public bool IsLoginVisible
    {
      get
      {
        AccoList.IsVisible = IsLoggedIn && SessionManager.UserName != "guest";
        return !IsLoggedIn && string.IsNullOrEmpty(savedUser);                        //|| SessionManager.UserName == "guest"; 

      }
    }

    public bool IsLoggedIn
    {
      get { return _authenticationService.IsLoggedIn; }
    }

    public async Task OnLoggedIn()
    {
      await SessionManager.InitializeAsync(_authenticationService.Principal.Identity.Name);
      await SystemCodeService.LoadSystemCodesAsync();
      if (SessionManager.UserName == "guest")
        return;      //werkt dit?????
      Start(true);
    }

    #region IHandle<LoggedInMessage> Members

    public async void Handle(LoggedInMessage message)
    {
      NotifyOfPropertyChange(() => IsLoggedIn);
      NotifyOfPropertyChange(() => IsLoginVisible);
      if (IsLoggedIn)
      { 
        await OnLoggedIn();
      }
    }

    #endregion

    #region IHandle<LoggedOutMessage> Members

    public void Handle(LoggedOutMessage message)
    {
      savedUser = "";
      savedPassword = "";
      appSettings.Remove("AccoUser");
      appSettings.Remove("AccoPassword");
      appSettings.Save();

      NotifyOfPropertyChange(() => IsLoggedIn);
      NotifyOfPropertyChange(() => IsLoginVisible);
    }

    #endregion

    #region IHarnessAware Members

    /// <summary>
    ///   Provides the setup logic to be run before the ViewModel is activated inside of the development harness.
    /// </summary>
    public void Setup()
    {
#if HARNESS
            Start();
#endif
    }

    #endregion

    public void BuildMenu(string group)
    {

      ExtraMenu.Clear();

      var extraGroup = new MenuGroup(0, group);

      _workspaces.OrderBy(w => w.Sequence)
        .Where(x => x.IsPublic == (SessionManager.UserName == "guest") && x.GroupName == group && x.IsTrustee == SessionManager.IsTrustee)
        .ForEach(
          w => extraGroup.Add(new MenuAction(this, w.DisplayName, async () => await NavigateToWorkspace(w))));

      ExtraMenu.AddGroup(extraGroup);
    }

    public ShellViewModel Start(bool setlist)
    {
      StartAsync(setlist);
      return this;
    }

    private async void StartAsync(bool setList)
    {
      if (!IsLoggedIn && !string.IsNullOrEmpty(savedUser))
      {
        

        var credential = new LoginCredential(savedUser, savedPassword, null);
        try
        {
          await _authenticationService.LoginAsync(credential);
        }
        catch (Exception)
        {
          //remove saved info and start again
          appSettings.Remove("AccoUser");
          appSettings.Remove("AccoPassword");
          savedUser = "";
          savedPassword = "";
          NotifyOfPropertyChange(() => IsLoginVisible);
          Start(false);
          return;
        }
       
        NotifyOfPropertyChange(() => IsLoginVisible);
        return;
      }

      if (setList && SessionManager.CurrentOwner != null)
      {
        NotifyOfPropertyChange(() => IsLoginVisible);
        AccoList.Start(SessionManager.CurrentOwner.AccoOwnerId);
        AccoList.ItemId = SessionManager.CurrentAcco.AccoId;
      }
      Menu.Clear();
      var s = SessionManager.UserName;

      var mainGroup = new MenuGroup(0, "Main");

      if (IsLoggedIn && SessionManager.UserName == "guest")
        mainGroup.Add(new MenuAction(this, Resources.AccoBooking.ws_PUBLIC, Logout));

      _workspaces.OrderBy(w => w.Sequence)
        .Where(x => x.IsPublic == (SessionManager.UserName == "guest") && IsLoggedIn && x.IsTrustee == SessionManager.IsTrustee &&
                    (x.GroupName == "Main"  ) || (SessionManager.UserName == "Admin" && x.GroupName == "Admin")
              )
        .ForEach(w => mainGroup.Add(new MenuAction(this, w.DisplayName, async () => await NavigateToWorkspace(w))));

      if (IsLoggedIn && SessionManager.UserName != "guest")
        mainGroup.Add(new MenuAction(this, Resources.AccoBooking.but_LOGOFF, Logout));

      Menu.AddGroup(mainGroup);

      NotifyOfPropertyChange(() => IsLoginVisible);

      var publicScreen = GetPublicScreen();

      if (!IsLoggedIn)
      {
        if (publicScreen != null)
          await NavigateToWorkspace(publicScreen);
      }
      else if (SessionManager.UserName == "guest")
      {
        await NavigateToPublic();        
      }
      else if (SessionManager.IsTrustee)
      {
        await NavigateToTrusteeBooking();
      }
      else
      {
        await NavigateToAvailability();
      }
    }

    public async Task Login()
    {
      try
      {
        //await _loginFactory.CreateExport().Value.ShowAsync();
      }
      catch (TaskCanceledException)
      {
        TryClose();
      }
    }

    public async void Logout()
    {
      BuildMenu("");
      SessionManager.UserName = "guest";
      SessionManager.CurrentOwner = null;
      SessionManager.CurrentAcco = null;
      var home = GetPublicScreen();
      LogFns.DebugWriteLineIf(home == null, Resources.AccoBooking.mes_NO_DEFAULT_WORKSPACE);
      if (home == null)
        return;

      await NavigateToWorkspace(home);

      await _authenticationService.LogoutAsync();

      //await Login();
      Start(false);
    }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      Start(false);

#if !SILVERLIGHT
            DisplayName = "AccoBooking for WPF";
#endif
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

      // set the correct language, needed for SL4 is niet de juiste plek in viewmodel?
      //Todo: @@@ JKT Testen of het nog relevant is in SL5
      var shellview = view as ShellView;
      if (shellview != null)
        shellview.Language = System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

      // Launch login dialog
      //await Login();
    }

    private IWorkspace GetPublicScreen()
    {
      return _workspaces.FirstOrDefault(w => w.IsDefault);
    }

    private async Task NavigateToWorkspace(IWorkspace workspace)
    {
      // Break if the workspace is already active.
      if (ActiveItem != null && ActiveItem.GetType() == workspace.ViewModelType)
        await _navigator.NavigateToAsync(GetPublicScreen().ViewModelType);

      await _navigator.NavigateToAsync(workspace.ViewModelType);
    }

    public async Task NavigateToTrusteeBooking()
    {
      await NavigateToWorkspace(GetPublicScreen());
      var searchbooking = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_SEARCH_BOOKING &&
                                                     w.IsTrustee);
      await NavigateToWorkspace(searchbooking);     
    }

    public async Task NavigateToAvailability()
    {
      await NavigateToWorkspace(GetPublicScreen());
      var calender = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_DISPLAY_AVAILABILITY);
      await NavigateToWorkspace(calender);
    }

    public async Task NavigateToPublic()
    {
      await NavigateToWorkspace(GetPublicScreen());
      var publicWorkSpace = _workspaces.FirstOrDefault(w => w.DisplayName == Resources.AccoBooking.ws_VIDEO);
      await NavigateToWorkspace(publicWorkSpace);
    }

    public async Task NavigateToWorkSpace(string wsName)
    {
      var ws = _workspaces.FirstOrDefault(w => w.DisplayName == wsName);
      await NavigateToWorkspace(ws);
    }
  }
}