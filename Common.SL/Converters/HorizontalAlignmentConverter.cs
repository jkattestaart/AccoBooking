//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Common.Converters
{
  public class HorizontalAlignmentConverter : IValueConverter
  {
    /// <exception cref="ArgumentException">TargetType must be Visibility</exception> 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is int))
        throw new ArgumentException("Source must be of type integer");

      if (targetType != typeof(HorizontalAlignment))
        throw new ArgumentException("TargetType must be HorizontalAlignment");

      int v = (int)value;

      if (v == 1)
        return HorizontalAlignment.Center;
      if (v == 2)
        return HorizontalAlignment.Left;
      if (v == 3)
        return HorizontalAlignment.Right;
      if (v == 4)
        return HorizontalAlignment.Stretch;
      return HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  } 

}
