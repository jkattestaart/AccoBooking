﻿// ====================================================================================================================
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
  public class AccoLicenseDetailViewModel : BaseDetailViewModel<AccoOwner>
  {
    [ImportingConstructor]
    public AccoLicenseDetailViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager,
                                      AccoLicenseSummaryViewModel licenseSummary,
                                      IDialogManager dialogManager)
      : base(unitOfWorkManager, null, licenseSummary, dialogManager)
    {

    }

    protected override IRepository<AccoOwner> Repository()
    {
      return UnitOfWork.AccoOwners;
    }

    protected override IFactory<AccoOwner> Factory(IAccoBookingUnitOfWork unitOfWork)
    {
      return unitOfWork.AccoOwnerFactory;
    }

  }

}
