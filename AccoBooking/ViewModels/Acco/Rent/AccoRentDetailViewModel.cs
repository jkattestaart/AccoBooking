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
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainServices;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoRentDetailViewModel : BaseDetailViewModel<AccoRent>
  {
    private int currentYear;

    [ImportingConstructor]
    public AccoRentDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                          AccoRentSummaryViewModel rentSummary,
                                          IDialogManager dialogManager)
      : base(unitOfWorkManager, null, rentSummary, dialogManager)
    {
      currentYear = DateTime.Now.Year;
    }

    protected override IRepository<AccoRent> Repository()
    {
      return UnitOfWork.AccoRents;
    }

    protected override IFactory<AccoRent> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoRentFactory;
    }

    protected override void OnCreateEntity(AccoRent entity, int parentid)
    {
      base.OnCreateEntity(entity, parentid);
      entity.AccoId = parentid;
      entity.RentYear = currentYear;
    }

  }

}
