using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Cocktail;

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class MailSettingsViewModel : Screen
  {
    private readonly IDialogManager _dialogManager;
    private string _email;
    private string _password;
    private IDialogUICommand<DialogResult> _okCommand;
    private bool _rememberSettings;

    [ImportingConstructor]
    public MailSettingsViewModel(IDialogManager dialogManager, SystemCodeListViewModel providers)
    {
      _dialogManager = dialogManager;

      Providers = providers;
      Providers.PropertyChanged += Providers_PropertyChanged;
      Providers.Start("PROVIDER");
    }

    void Providers_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "ShortName")
      {
      }  
    }

    public SystemCodeListViewModel Providers { get; set; }
    

    public string Email
    {
      get { return _email; }
      set
      {
        _email = value;
        NotifyOfPropertyChange(() => Email);
        UpdateCommands();
      }
    }


    public string Password
    {
      get { return _password; }
      set
      {
        _password = value;
        NotifyOfPropertyChange(() => Password);
        UpdateCommands();
      }
    }

    public bool RememberSettings
    {
      get { return _rememberSettings; }
      set
      {
        _rememberSettings = value;
        NotifyOfPropertyChange(() => RememberSettings);
        UpdateCommands();
      }
    }

    private bool IsComplete
    {
      get { return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password); }
    }

    public Task<DialogResult> ShowDialogAsync()
    {
      _okCommand = new DialogUICommand<DialogResult>(DialogResult.Ok, true) {Enabled = IsComplete};
      var cancelCommand = new DialogUICommand<DialogResult>(DialogResult.Cancel, false, true);

      return _dialogManager.ShowDialogAsync(new[] {_okCommand}, this);
    }

    public override void CanClose(Action<bool> callback)
    {
      var dialogHost = DialogHost.GetCurrent(this);
      if (!dialogHost.DialogResult.Equals(DialogResult.Cancel))
      {
        callback(IsComplete);
      }
      else
        base.CanClose(callback);
    }

    private void UpdateCommands()
    {
      if (_okCommand != null)
        _okCommand.Enabled = IsComplete;
    }

   
  }
}