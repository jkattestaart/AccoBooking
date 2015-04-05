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
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class AccoRentManagementViewModel : BaseMasterViewModel<AccoRentSearchViewModel,AccoRentDetailViewModel, AccoRentListItem, AccoRent>
                                        
  {
    private ShellViewModel _shell;

    [ImportingConstructor]
    public AccoRentManagementViewModel(ExportFactory<AccoRentSearchViewModel> searchFactory,
                                              ExportFactory<AccoRentDetailViewModel> detailFactory,
                                              IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                              ShellViewModel shellViewModel,
                                              IDialogManager dialogManager,
                                              ToolbarViewModel toolbar,
                                              ToolbarViewModel bottomToolbar)
      : base(searchFactory, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
      _shell = shellViewModel;
      UseCopy = true;
    }

    protected override IRepository<AccoRent> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoRents;
    }

    protected override async void OnDelete(IAccoBookingUnitOfWork unitOfWork, AccoRent entity)
    {
      if (SessionManager.CurrentAcco.BaseRentId == entity.AccoRentId)
      {
        await
          _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_RENT_IS_BASERENT,
          new[] {Resources.AccoBooking.but_OK});
      }
      else if (entity.AccoSeasons.Count > 0)
      {
        await
          _dialogManager.ShowMessageAsync(Resources.AccoBooking.mes_RENT_INUSE_SEASONS,
            new[] { Resources.AccoBooking.but_OK });       
      }
      else
      {
        base.OnDelete(unitOfWork, entity);       
      }
    }

    public override void Copy()
    {
      _shell.NavigateToWorkSpace(Resources.AccoBooking.ws_COPY_RENT);
    }
  }
}