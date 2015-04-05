using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.General
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class CountrySearchViewModel : BaseSearchViewModel<Country, CountryListItem>
  {

    [ImportingConstructor]
    public CountrySearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    protected override Task<IEnumerable<CountryListItem>> ExecuteQuery()
    {
      return UnitOfWork.CountrySearchService.FindCountriesAsync(_parentid, CancellationToken.None);
    }

    public void MoveUp()
    {
      if (CurrentItem == null)
        return;
      for (int i = 0; i < Items.Count; i++)
      {
        if (Items.ElementAt(i) == CurrentItem)
        {
          if (i == 0)
            break;

          var prevItem = Items.ElementAt(i - 1);
          var curItem = Items.ElementAt(i);
          Items.RemoveAt(i - 1);
          Items.RemoveAt(i - 1);
          Items.Insert(i - 1, curItem);
          Items.Insert(i, prevItem);
          CurrentItem = curItem;
          break;
        }
      }

      NotifyOfPropertyChange(() => Items);
      NotifyOfPropertyChange(() => CurrentItem);
    }


    public void MoveDown()
    {
      if (CurrentItem == null)
        return;
      for (int i = 0; i < Items.Count; i++)
      {
        if (Items.ElementAt(i) == CurrentItem)
        {
          if (i == Items.Count - 1)
            break;

          var nextItem = Items.ElementAt(i + 1);
          var curItem = Items.ElementAt(i);
          Items.RemoveAt(i);
          Items.RemoveAt(i);
          Items.Insert(i, curItem);
          Items.Insert(i, nextItem);
          CurrentItem = curItem;
          break;
        }
      }
      NotifyOfPropertyChange(() => Items);
      NotifyOfPropertyChange(() => CurrentItem);
    }

    public async void Save()
    {
      var seq = 0;
      foreach (var item in Items)
      {
        seq = seq + 10;
        item.DisplaySequence = seq;
      }

      try
      {

        using (Busy.GetTicket())
        {
          foreach (var item in Items)
          {

            var country = await UnitOfWork.Countries.WithIdFromDataSourceAsync(item.Id);

            if (country.DisplaySequence != item.DisplaySequence)
              country.DisplaySequence = item.DisplaySequence;
          }

          await UnitOfWork.CommitAsync();
        }
      }
      catch (Exception)
      {

        throw;
      }

    }


  }
}