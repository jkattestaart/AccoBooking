using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocktail;

namespace UnitTestAccoBooking
{
  public class TestCompositionProvider : ICompositionProvider
  {
    private readonly Dictionary<Type, object> _container;
    
    public TestCompositionProvider()
    {
      _container = new Dictionary<Type, object>();
    }
    
    public void AddOrUpdateInstance<T>(T instance)
    {
      _container[typeof(T)] = instance;
    }
    
    /// <summary>
    /// Returns a lazy instance of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the requested instance. </typeparam>
    public Lazy<T> GetInstance<T>() where T : class
    {
      return new Lazy<T>(() => (T)_container[typeof(T)]);
    }
    
    /// <summary>
    /// Returns an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the requested instance. </typeparam>
    /// <returns>
    /// Null if instance is not present in the container. 
    /// </returns>
    public T TryGetInstance<T>() where T : class
    {
      object instance;
      if (!_container.TryGetValue(typeof(T), out instance))
        return null;

      return (T)instance;
    }

    /// <summary>
    /// Returns all instances of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the requested instances. </typeparam>
    public IEnumerable<T> GetInstances<T>() where T : class
    {
      return _container.Keys.Where(x => x == typeof(T)).Select(x => _container[x]).OfType<T>();
    }

    /// <summary>
    /// Returns a lazy instance that matches the specified name and type.
    /// </summary>
    /// <param name="serviceType">The type to match.</param><param name="contractName">The name to match.</param>
    public Lazy<object> GetInstance(Type serviceType, string contractName)
    {
      return new Lazy<object>(() => _container[serviceType]);
    }
    /// <summary>
    /// Returns an instance that matches the specified name and type.
    /// </summary>
    /// <param name="serviceType">The type to match.</param><param name="contractName">The name to match.</param>
    /// <returns>
    /// Null if instance is not present in the container. 
    /// </returns>
    public object TryGetInstance(Type serviceType, string contractName)
    {
      object instance;
      if (!_container.TryGetValue(serviceType, out instance))
        return null;

      return instance;
    }

    /// <summary>
    /// Returns all instances that match the specified name and type.
    /// </summary>
    /// <param name="serviceType">The type to match.</param><param name="contractName">The name to match.</param>
    public IEnumerable<object> GetInstances(Type serviceType, string contractName)
    {
      return _container.Keys.Where(x => x == serviceType).Select(x => _container[x]);
    }

    /// <summary>
    /// Returns a factory that creates new instances of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of instance the factory creates. </typeparam>
    public ICompositionFactory<T> GetInstanceFactory<T>() where T : class
    {
      return new ActivatorFactory<T>();
    }

    /// <summary>
    /// Returns a factory that creates new instances of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of instance the factory creates. </typeparam>
    /// <returns>
    /// Null if the container cannot provide a factory for the specified type. 
    /// </returns>
    public ICompositionFactory<T> TryGetInstanceFactory<T>() where T : class
    {
      return GetInstanceFactory<T>();
    }

    /// <summary>
    /// Manually performs property dependency injection on the provided instance.
    /// </summary>
    /// <param name="instance">The instance needing property injection. </param>
    public void BuildUp(object instance)
    {
      // Noop
    }
  }

  public class ActivatorFactory<T> : ICompositionFactory<T>
     where T : class
  {
    /// <summary>
    /// Creates new instance.
    /// </summary>
    public T NewInstance()
    {
      return Activator.CreateInstance<T>();
    }
  }
}
