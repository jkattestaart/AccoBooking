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
using System.Diagnostics;
using Caliburn.Micro;
using DomainModel;

namespace AccoBooking.ViewModels.Acco
{
    public class AccoAdditionDescriptionItemViewModel : PropertyChangedBase, IDisposable
    {
        private AccoAdditionDescription _item;

        public AccoAdditionDescriptionItemViewModel(AccoAdditionDescription item)
        {
            Debug.Assert(item != null);
            Item = item;
        }

        public AccoAdditionDescription Item
        {
            get { return _item; }
            private set
            {
                _item = value;
                NotifyOfPropertyChange(() => Item);
            }
        }



        public void Dispose()
        {
          _item = null;
        }
    }
}