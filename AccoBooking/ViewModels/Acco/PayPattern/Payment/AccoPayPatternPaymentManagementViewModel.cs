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
using DomainModel.Projections;
using DomainServices;

namespace AccoBooking.ViewModels.Acco
{
  [Export]
  public class AccoPayPatternPaymentManagementViewModel : BaseManagementViewModel<AccoPayPatternPaymentDetailViewModel, AccoPayPatternPaymentListItem, DomainModel.AccoPayPatternPayment>
                                        
  {
   
    [ImportingConstructor]
    public AccoPayPatternPaymentManagementViewModel(AccoPayPatternPaymentSearchViewModel searchPane,
                                                    ExportFactory<AccoPayPatternPaymentDetailViewModel> detailFactory,
                                                    IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                                    IDialogManager dialogManager,
                                                    ToolbarViewModel toolbar,
                                                    ToolbarViewModel bottomToolbar
                                                   )
      : base(searchPane, detailFactory, unitOfWorkManager, dialogManager, toolbar, bottomToolbar)
    {
    
    }
    
    protected override IRepository<DomainModel.AccoPayPatternPayment> Repository(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoPayPatternPayments;
    }
   
  }
}