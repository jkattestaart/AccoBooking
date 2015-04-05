// ====================================================================================================================
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Common.Errors;
using DomainServices;
using IdeaBlade.EntityModel;

namespace AccoBooking.ViewModels
{

  //public delegate void ItemSelectedEventHandler(object sender, EventArgs e);

  /// <summary>
  /// Base class for a list object, like combo-boxes
  /// </summary>
  /// <typeparam name="TEntity">Entity in dit programma</typeparam>
  public abstract class RichTextBoxViewModel : Screen 
  {
    protected string _description;
    protected string _shortName;
    protected int _itemid;
    protected readonly IDomainUnitOfWork _unitOfWork;
       protected readonly IErrorHandler _errorHandler;
    protected readonly IDomainUnitOfWorkManager<IDomainUnitOfWork> _domainUnitOfWorkManager;
    protected bool _isEnabled = true;
    private string _text;


    /// <summary>
    /// Constructor for de list class
    /// </summary>
    /// <param name="unitOfWorkManager">Unit of work manager used</param>
    /// <param name="errorHandler">Handler for errors</param>
    protected RichTextBoxViewModel(IDomainUnitOfWorkManager<IDomainUnitOfWork> unitOfWorkManager,
                                   IErrorHandler errorHandler)
    {
      _domainUnitOfWorkManager = unitOfWorkManager;
      _errorHandler = errorHandler;
      _unitOfWork = _domainUnitOfWorkManager.Create();
    }

    public virtual bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        _isEnabled = value;
        NotifyOfPropertyChange(() => IsEnabled);
      }
    }
    
    //public event ItemSelectedEventHandler ItemSelected;

    //// Invoke the ItemSelected event; called wheneve list is doubleclicked
    //protected virtual void OnItemSelected(EventArgs e)
    //{
    //  if (ItemSelected != null)
    //    ItemSelected(this, e);
    //}

    ////Bound method from the view
    //public void DoubleClicked()
    //{
    //  OnItemSelected(EventArgs.Empty);
    //}

    /// <summary>
    /// Text
    /// </summary>
    public string Text 
    {
      get { return _text; }
      set
      {
        _text = value;
        NotifyOfPropertyChange(() => Text);
      }
    }

    /// <summary>
    /// Start the listitem, must be coded
    /// </summary>
    /// <param name="selection">Extra selection on the items to create the predicate</param>
    /// <example>For systemcodes a shortname for the group can be specified</example>
    /// <returns></returns>
    public virtual RichTextBoxViewModel Start(int selection)
    {
      return this;
    }

    public virtual RichTextBoxViewModel Start(string selection)
    {
      return this;
    }

  }


}