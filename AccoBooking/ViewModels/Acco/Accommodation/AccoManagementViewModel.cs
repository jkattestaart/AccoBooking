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

using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using DomainServices;
using DomainServices.Services;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class AccoManagementViewModel : BaseManagementViewModel<AccoDetailViewModel, AccoListItem, DomainModel.Acco>
  {
    private ShellViewModel _shellViewModel;

    // Search is dummy navigation! 
    [ImportingConstructor]
    public AccoManagementViewModel(AccoSearchViewModel searchPane,
                                   ExportFactory<AccoDetailViewModel> detailFactory,
                                   IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                   ShellViewModel shellViewModel,
                                   IDialogManager dialogManager,
                                   ToolbarViewModel toolbar,
                                   ToolbarViewModel bottomToolbar)
      : base(searchPane, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
      _shellViewModel = shellViewModel; 
      _shellViewModel.ExtraMenu.Clear();
    }

    // Acco lezen
    protected async override void OnActivate()
    {

      base.OnActivate();

      try
      {
        var detail = ActiveDetail ?? _detailFactory.CreateExport().Value;

        await _navigator.NavigateToAsync(detail.GetType(),
          target => (target as AccoDetailViewModel).Start(SessionManager.CurrentAcco.AccoId)
          );
      }
      catch (TaskCanceledException)
      {       
        UpdateCommands();
      }
    }

    public async override Task OnSave()
    {
      var acco = (DomainModel.Acco)ActiveDetail.Entity;
      var address = acco.Address1 + " " 
                  + acco.Address2 + " " 
                  + acco.Address3;
      address = address.Trim(' ');
      if (!string.IsNullOrEmpty(address))
      {
        var coordinates = await GeoCodingService.ExecuteAsync(address);
        if (!string.IsNullOrEmpty(coordinates))
        {
          acco.Latitude = decimal.Parse(coordinates.Split(';')[0]);
          acco.Longitude = decimal.Parse(coordinates.Split(';')[1]);
        }
      }
      await base.OnSave();
    }

    public override async Task OnPostSave(bool isDelete)
    {
      //using (Busy.GetTicket())
      {
        var unitofWork = _unitOfWorkManager.Create();

        SessionManager.CurrentAcco =
          await unitofWork.Accoes.WithIdFromDataSourceAsync(SessionManager.CurrentAcco.AccoId);
      }
    }

    public override BaseManagementViewModel<AccoDetailViewModel, AccoListItem, DomainModel.Acco> Start()
    {
      //_shellViewModel.BuildMenu("Settings");
      return base.Start();
    }

    protected override IRepository<DomainModel.Acco> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.Accoes;
    }

  }
}