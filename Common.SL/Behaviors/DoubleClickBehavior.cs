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
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Interactivity;
  using Caliburn.Micro;

namespace Common.Behaviors
{
  public static class DoubleClickEvent
  {
    public static readonly DependencyProperty AttachActionProperty =
      DependencyProperty.RegisterAttached(
        "AttachAction",
        typeof (string),
        typeof (DoubleClickEvent),
        new PropertyMetadata(OnAttachActionChanged));

    public static void SetAttachAction(DependencyObject d, string attachText)
    {
      d.SetValue(AttachActionProperty, attachText);
    }

    public static string GetAttachAction(DependencyObject d)
    {
      return d.GetValue(AttachActionProperty) as string;
    }

    private static void OnAttachActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;

      var text = e.NewValue as string;
      if (string.IsNullOrEmpty(text))
        return;

      AttachActionToTarget(text, d);
    }

    private static void AttachActionToTarget(string text, DependencyObject d)
    {
      var actionMessage = Parser.CreateMessage(d, text);

      var trigger = new ConditionalEventTrigger
        {
          EventName = "MouseLeftButtonUp",
          Condition = e => DoubleClickCatcher.IsDoubleClick(d, e)
        };
      trigger.Actions.Add(actionMessage);

      Interaction.GetTriggers(d).Add(trigger);
    }

    public class ConditionalEventTrigger : System.Windows.Interactivity.EventTrigger
    {
      public Func<EventArgs, bool> Condition { get; set; }

      protected override void OnEvent(EventArgs eventArgs)
      {
        if (Condition(eventArgs))
          base.OnEvent(eventArgs);
      }
    }

    private static class DoubleClickCatcher
    {
      private const int DoubleClickSpeed = 400;
      private const int AllowedPositionDelta = 6;

      private static Point clickPosition;
      private static DateTime lastClick = DateTime.Now;
      private static bool firstClickDone;

      internal static bool IsDoubleClick(object sender, EventArgs args)
      {
        var element = sender as UIElement;
        var clickTime = DateTime.Now;

        var e = args as MouseEventArgs;
        if (e == null)
          throw new ArgumentException("MouseEventArgs expected");

        var span = clickTime - lastClick;

        if (span.TotalMilliseconds > DoubleClickSpeed || firstClickDone == false)
        {
          clickPosition = e.GetPosition(element);
          firstClickDone = true;
          lastClick = DateTime.Now;
          return false;
        }

        firstClickDone = false;
        var position = e.GetPosition(element);
        if (Math.Abs(clickPosition.X - position.X) < AllowedPositionDelta &&
            Math.Abs(clickPosition.Y - position.Y) < AllowedPositionDelta)
          return true;
        return false;
      }
    }
  }
}