using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoAdditionSearchViewModel : BaseSearchViewModel<AccoAddition, AccoAdditionListItem>
  {
    [ImportingConstructor]
    public AccoAdditionSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
    //  Busy.AddWatch();
    //  _unitOfWork.SystemCodeSearchService.FindSystemCodesByGroupAsync("UNIT")
    //             .ContinueWith(op =>
    //             {
    //               if (op.CompletedSuccessfully)
    //               {
    //                 _systemcodes = op.Result;
    //                 Search();
    //               }
    //               if (op.HasError)
    //                 _errorHandler.HandleError(op.Error);

    //               Busy.RemoveWatch();
    //             });
    }

    protected override Task<IEnumerable<AccoAdditionListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoAdditionSearchService.FindAccoAdditionsAsync(_parentid, CancellationToken.None);
    } 

  }
}
