using Caliburn.Micro;

namespace AccoBooking.ViewModels
{
  /// <summary>
  /// Interface for the section in the program (used to select the tabs)
  /// </summary>
  /// <typeparam name="TEntity">Entity for the interface</typeparam>
  public interface IBaseDetailSection<TEntity> : IScreen where TEntity : class
  {
    /// <summary>
    /// Index for the section (ordering of the tabs)
    /// </summary>
    int Index { get; }

    bool IsVisible { get; set; }

    /// <summary>
    /// Start the program with the id of the entity
    /// </summary>
    /// <param name="entityid">the id of the entity</param>
    void Start(int entityid);

    /// <summary>
    /// Start the program with the entity already in memory
    /// </summary>
    /// <param name="entity">the entity used</param>
    void Start(TEntity entity);

  }
}