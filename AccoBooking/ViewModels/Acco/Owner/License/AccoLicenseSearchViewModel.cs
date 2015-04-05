using System;
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
  public class AccoLicenseSearchViewModel : BaseSearchViewModel<AccoOwner, AccoOwnerListItem>
  {

    [ImportingConstructor]
    public AccoLicenseSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {
      
      Search();

    }


    //public bool CanSave
    //{
    //  get { return _unitOfWork.HasChanges(); }
    //}

    public async void Save()
    {
      try
      {

        using (Busy.GetTicket())
        {
          foreach (var item in Items)
          {
            var owner = await UnitOfWork.AccoOwners.WithIdFromDataSourceAsync(item.Id);

            //owner.Acco.LicenceExpiration = item.LicenseExpiration;

            await UnitOfWork.CommitAsync();

          }
        }
      }
      catch (Exception)
      {

        throw;
      }

    }

    protected override Task<IEnumerable<AccoOwnerListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoOwnerSearchService.FindAccoOwnersAsync("", CancellationToken.None);
    }

  }
}
