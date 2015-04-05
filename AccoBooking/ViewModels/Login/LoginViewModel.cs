//  ====================================================================================================================
//    Copyright (c) 2012 IdeaBlade
//  ====================================================================================================================
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//    WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//    OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//    OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//  ====================================================================================================================
//    USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//    http://cocktail.ideablade.com/licensing
//  ====================================================================================================================

using System;
using System.ComponentModel.Composition;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Cocktail;
using DomainServices.Repositories;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels.Login
{
  [Export]
  public class LoginViewModel : Screen
  {
    private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
    private readonly IAuthenticationService _authenticationService;
    private readonly IWindowManager _windowManager;
    private readonly IGlobalCache _globalCache;
    private string _failureMessage;
    //private IDialogUICommand<DialogResult> _loginCommand;
    private string _password;
    private string _username;
    private bool _isGuest;

    [ImportingConstructor]
    public LoginViewModel(IAuthenticationService authenticationService, IWindowManager windowManager,
      [Import(AllowDefault = true)] IGlobalCache globalCache)
    {
      Busy = new BusyWatcher();
      _authenticationService = authenticationService;
      _windowManager = windowManager;
      _globalCache = globalCache;
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = "";
      // ReSharper restore DoNotCallOverridableMethodsInConstructor

#if DEBUG
      _username = "Admin";
      _password = "password";
#endif
    }

    public IBusyWatcher Busy { get; private set; }

    public bool IsGuest
    {
      get { return _authenticationService.IsLoggedIn; }
          set  
      {
        _isGuest = value;
        if (_isGuest)
        {
          Username = "guest";
          Password = "guest";
        }
        else
        {
          _username = "Admin";
          _password = "password";
        }
        NotifyOfPropertyChange(() => Username);
        NotifyOfPropertyChange(() => Password);
        NotifyOfPropertyChange(() => CanLogin);
        NotifyOfPropertyChange(() => IsGuest);

      }
    }

    public bool SaveCredentials { get; set; }

    public string Username
    {
      get { return _username; }
      set
      {
        _username = value;
        NotifyOfPropertyChange(() => Username);
        NotifyOfPropertyChange(() => CanLogin);
      }
    }

    public string Password
    {
      get { return _password; }
      set
      {
        _password = value;
        NotifyOfPropertyChange(() => Password);
        NotifyOfPropertyChange(() => CanLogin);
      }
    }

    public string FailureMessage
    {
      get { return _failureMessage; }
      set
      {
        _failureMessage = value;
        NotifyOfPropertyChange(() => FailureMessage);
        NotifyOfPropertyChange(() => FailureMessageVisible);
      }
    }

    public bool FailureMessageVisible
    {
      get { return !string.IsNullOrWhiteSpace(_failureMessage); }
    }

    public bool CanLogin
    {
      get
      {
        return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
      }
    }

    public async void LoginGuest()
    {
      IsGuest = true;
      await LoginAsync();
    }

    public async void Login()
    {
      await LoginAsync();
    }

    public async Task LoginAsync() // was LoginAsync
    {
      if (Username != "guest")
      {
        appSettings.Remove("AccoUser");
        appSettings.Remove("AccoPassword");
        if (SaveCredentials)
        {
          appSettings.Add("AccoUser", Username);
          appSettings.Add("AccoPassword", Password);
        }
          appSettings.Save();
      }

      using (Busy.GetTicket())
      {
        FailureMessage = "";

        var credential = new LoginCredential(Username, Password, null);
        // Clear username and password fields
        Username = "";
        Password = "";

        try
        {
          await _authenticationService.LoginAsync(credential);

          if (_globalCache != null)
          {
            try
            {
              await _globalCache.LoadAsync();
            }
            catch (Exception e)
            {
              throw new Exception(Resources.AccoBooking.mes_LOAD_GLOBAL_CACHE_FAILED, e);
            }
          }
        }
        catch (Exception e)
        {
          FailureMessage = e.Message;
        }

      }
    }

    //        public Task ShowAsync()
    //        {
    //            var commands = new List<IDialogUICommand<DialogResult>>();
    //            _loginCommand = new DialogUICommand<DialogResult>("Login", DialogResult.Ok, true);
    //            _loginCommand.Invoked += async (sender, args) =>
    //                {
    //                    args.Cancel(); // Cancel command, we'll take it from here.

    //                    await LoginAsync();

    //                    if (_authenticationService.IsLoggedIn)
    //                        args.DialogHost.TryClose(_loginCommand.DialogResult);
    //                };
    //            commands.Add(_loginCommand);

    //#if !SILVERLIGHT
    //            var closeCommand = new DialogUICommand<DialogResult>("Close", DialogResult.Cancel, false, true);
    //            commands.Add(closeCommand);
    //#endif

    //            UpdateCommands();
    //            return _dialogManager.ShowDialogAsync(commands, this);
    //        }

    //private void UpdateCommands()
    //{
    //  _loginCommand.Enabled = CanLogin;
    //}

    public async void KeyDown(KeyEventArgs args)
    {
      if (args.Key != Key.Enter)
        return;

      await LoginAsync();
    }
  }
}