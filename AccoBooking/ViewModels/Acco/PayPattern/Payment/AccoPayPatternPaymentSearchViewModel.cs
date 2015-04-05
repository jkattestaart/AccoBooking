using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using DomainModel;
using DomainModel.Projections;
using DomainServices;


namespace AccoBooking.ViewModels.Acco
{
  [Export, PartCreationPolicy(CreationPolicy.NonShared)]
  public class AccoPayPatternPaymentSearchViewModel :
    BaseSearchViewModel<AccoPayPatternPayment, AccoPayPatternPaymentListItem>
  {

    [ImportingConstructor]
    public AccoPayPatternPaymentSearchViewModel(IUnitOfWorkManager<IAccoBookingUnitOfWork> unitOfWorkManager)
      : base(unitOfWorkManager)
    {

    }

    public string TotalOk { get; set; }

    public override BindableCollection<AccoPayPatternPaymentListItem> Items
    {
      get { return base.Items; }
      set
      {
        base.Items = value;
        if (Items != null)
        {
          decimal totalPercentage = 0;

          foreach (var item in Items)
            totalPercentage += item.PaymentPercentage;

          TotalOk = "";
          //TotalOk = SessionManager.GetString("TOTAL_NOT_OK");
          if (Decimal.Compare(totalPercentage, 100) != 0)
            TotalOk =  Resources.AccoBooking.mes_TOTAL_100PCT;

          NotifyOfPropertyChange(() => TotalOk);
        }
      }
    }

    protected override Task<IEnumerable<AccoPayPatternPaymentListItem>> ExecuteQuery()
    {
      return UnitOfWork.AccoPayPatternPaymentSearchService.FindAccoPayPatternPaymentsAsync(_parentid, CancellationToken.None);
    }

  }

}
