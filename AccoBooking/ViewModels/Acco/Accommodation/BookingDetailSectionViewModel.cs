using System.ComponentModel.Composition;

#if HARNESS
using DomainServices.SampleData;
#endif

namespace AccoBooking.ViewModels.Acco
{
  [Export(typeof(IBaseDetailSection<DomainModel.Acco>)), PartCreationPolicy(CreationPolicy.NonShared)]
  public class BookingDetailSectionViewModel : BaseSectionViewModel<DomainModel.Acco, BookingSummaryViewModel>
  {
    [ImportingConstructor]
    public BookingDetailSectionViewModel(BookingSummaryViewModel bookingSummary)
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      DisplayName = Resources.AccoBooking.tab_BOOKING;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
      Section = bookingSummary;
    }

    #region IBaseDetailSection Members

    public override int Index
    {
      get { return 10; }
    }

    #endregion

    public override BaseSectionViewModel<DomainModel.Acco, BookingSummaryViewModel> Start(int accoid)
    {
      ActivateItem(Section.Start(accoid));
      return this;
    }

    public override BaseSectionViewModel<DomainModel.Acco, BookingSummaryViewModel> Start(DomainModel.Acco acco)
    {
      ActivateItem(Section.Start(acco));
      return this;
    }

  }
}