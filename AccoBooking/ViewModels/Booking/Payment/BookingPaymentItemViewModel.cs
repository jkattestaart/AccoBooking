using System;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;
using DomainModel;

namespace AccoBooking.ViewModels.Booking
{
  public class BookingPaymentItemViewModel : PropertyChangedBase, IDisposable
  {

    private BookingPayment _item;

    public BookingPaymentItemViewModel(BookingPayment item)
    {
      Debug.Assert(item != null);
      Item = item;
    }

    public BookingPayment Item
    {
      get { return _item; }
      private set
      {
        _item = value;
        _item.EntityAspect.EntityPropertyChanged += ItemPropertyChanged;

        NotifyOfPropertyChange(() => Item);
      }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool CanDelete
    {
      get { return !IsReadOnly && (Item.Booking.BookingPayments.Count > 0); }
    }


    #region IDisposable Members

    public void Dispose()
    {
      _item.EntityAspect.EntityPropertyChanged -= ItemPropertyChanged;
      _item = null;
    }

    #endregion

    private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(e.PropertyName))
        NotifyOfPropertyChange(() => CanDelete);
     
      NotifyOfPropertyChange(() => Item);
      
    }

  }
}
