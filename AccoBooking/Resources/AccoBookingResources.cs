namespace AccoBooking.Resources
{ 
  // Wrapper class to load the resource (resource has internal constructor XAML cannot access it)
  public class AccoBookingResources
  {
    private static AccoBooking accobookingResource = new AccoBooking();
    public AccoBooking AccoBooking { get { return accobookingResource; } }
  }
}
