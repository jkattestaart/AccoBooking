using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Common
{
  public class XPanel : Panel
  {

    #region Dependency properties

    #region AnimationLength property

    public int AnimationLength
    {
      get { return (int)GetValue(AnimationLengthProperty); }
      set { SetValue(AnimationLengthProperty, value); }
    }

    public static readonly DependencyProperty AnimationLengthProperty =
        DependencyProperty.Register("AnimationLength", typeof(int), typeof(XPanel), new PropertyMetadata(0));

    #endregion

    #region Columns property

    public int Columns
    {
      get { return (int)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    public static readonly DependencyProperty ColumnsProperty =
      DependencyProperty.Register("Columns", typeof (int), typeof (XPanel),
                                  new PropertyMetadata(0, OnColumnsChanged));

    private static void OnColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ((int)e.NewValue > 0)
      {
        ((XPanel)obj).Rows = 0;
        ((XPanel)obj).Orientation = Orientation.Horizontal;        
      }
      ((XPanel)obj).AutoWrap = (((XPanel)obj).Columns == 0 && ((XPanel)obj).Rows == 0);
    }

    #endregion

    #region Rows property

    public int Rows
    {
      get { return (int)GetValue(RowsProperty); }
      set { SetValue(RowsProperty, value); }
    }

    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register("Rows", typeof(int), typeof(XPanel),
          new PropertyMetadata(0, OnRowsChanged));

    private static void OnRowsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ((int)e.NewValue > 0)
      {
        ((XPanel)obj).Columns = 0;
        ((XPanel)obj).Orientation = Orientation.Vertical;
      }
      ((XPanel)obj).AutoWrap = (((XPanel)obj).Columns == 0 && ((XPanel)obj).Rows == 0);
    }

    #endregion

    #region Orientation property

    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(XPanel),
          new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ((Orientation)e.NewValue == Orientation.Horizontal)
        ((XPanel)obj).Rows = 0;
      else
        ((XPanel)obj).Columns = 0;
    }

    #endregion

    #region Uniform property

    public bool Uniform
    {
      get { return (bool)GetValue(UniformProperty); }
      set { SetValue(UniformProperty, value); }
    }

    public static readonly DependencyProperty UniformProperty =
      DependencyProperty.Register("Uniform", typeof(bool), typeof(XPanel),
        new PropertyMetadata(false));

    #endregion

    #region ItemWidth property

    public double ItemWidth
    {
      get { return (double)GetValue(ItemWidthProperty); }
      set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty ItemWidthProperty =
      DependencyProperty.Register("ItemWidth", typeof(double), typeof(XPanel),
        new PropertyMetadata(0.0));

    #endregion

    #region ItemHeight property

    public double ItemHeight
    {
      get { return (double)GetValue(ItemHeightProperty); }
      set { SetValue(ItemHeightProperty, value); }
    }

    public static readonly DependencyProperty ItemHeightProperty =
      DependencyProperty.Register("ItemHeight", typeof(double), typeof(XPanel),
        new PropertyMetadata(0.0));

    #endregion

    #region AutoWrap property

    public bool AutoWrap
    {
      get { return (bool)GetValue(AutoWrapProperty); }
      set { SetValue(AutoWrapProperty, value); }
    }

    public static readonly DependencyProperty AutoWrapProperty =
      DependencyProperty.Register("AutoWrap", typeof(bool), typeof(XPanel),
        new PropertyMetadata(true));

    private static void OnAutoWrapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ((bool)e.NewValue == true)
      {
        ((XPanel)obj).Columns = 0;
        ((XPanel)obj).Rows = 0;
      }
    }

    #endregion    

    #endregion

    #region Attached properties

    #region Row attached property

    public static int GetRow(DependencyObject obj)
    {
      return (int)obj.GetValue(RowProperty);
    }

    private static void SetRow(DependencyObject obj, int value)
    {
      obj.SetValue(RowProperty, value);
    }

    public static readonly DependencyProperty RowProperty =
        DependencyProperty.RegisterAttached("Row", typeof(int), typeof(XPanel), new PropertyMetadata(0));

    #endregion

    #region Column attached property

    public static int GetColumn(DependencyObject obj)
    {
      return (int)obj.GetValue(ColumnProperty);
    }

    private static void SetColumn(DependencyObject obj, int value)
    {
      obj.SetValue(ColumnProperty, value);
    }

    public static readonly DependencyProperty ColumnProperty =
        DependencyProperty.RegisterAttached("Column", typeof(int), typeof(XPanel), new PropertyMetadata(0));


    #endregion

    #region XPosition attached property

    public static double GetXPosition(DependencyObject obj)
    {
      return (double)obj.GetValue(XPositionProperty);
    }

    private static void SetXPosition(DependencyObject obj, double value)
    {
      obj.SetValue(XPositionProperty, value);
    }

    public static readonly DependencyProperty XPositionProperty =
        DependencyProperty.RegisterAttached("XPosition", typeof(double), typeof(XPanel), new PropertyMetadata(0.0));

    #endregion

    #region YPosition attached property

    public static double GetYPosition(DependencyObject obj)
    {
      return (double)obj.GetValue(YPositionProperty);
    }

    private static void SetYPosition(DependencyObject obj, double value)
    {
      obj.SetValue(YPositionProperty, value);
    }

    public static readonly DependencyProperty YPositionProperty =
        DependencyProperty.RegisterAttached("YPosition", typeof(double), typeof(XPanel), new PropertyMetadata(0.0));

    #endregion

    #region IsLastCellInRow attached property

    public static bool GetIsLastCellInRow(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsLastCellInRowProperty);
    }

    public static void SetIsLastCellInRow(DependencyObject obj, bool value)
    {
      obj.SetValue(IsLastCellInRowProperty, value);
    }

    public static readonly DependencyProperty IsLastCellInRowProperty =
        DependencyProperty.RegisterAttached("IsLastCellInRow", typeof(bool), typeof(XPanel), new PropertyMetadata(false));

    #endregion

    #region IsLastCellInColumn attached property

    public static bool GetIsLastCellInColumn(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsLastCellInColumnProperty);
    }

    public static void SetIsLastCellInColumn(DependencyObject obj, bool value)
    {
      obj.SetValue(IsLastCellInColumnProperty, value);
    }

    public static readonly DependencyProperty IsLastCellInColumnProperty =
        DependencyProperty.RegisterAttached("IsLastCellInColumn", typeof(bool), typeof(XPanel), new PropertyMetadata(false));

    #endregion

    #endregion


    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.Children == null || this.Children.Count == 0)
        return Size.Empty;

      SetChildProperties(availableSize);

      double width = 0, height = 0;

      foreach (FrameworkElement child in Children)
      {
        width = Math.Max(width, XPanel.GetXPosition(child) + child.DesiredSize.Width);
        height = Math.Max(height, XPanel.GetYPosition(child) + child.DesiredSize.Height);
      }

      return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.Children == null || this.Children.Count == 0)
        return finalSize;

      SetChildProperties(finalSize);

      foreach (UIElement child in Children)
      {
        if (this.AnimationLength > 0)
        {
          TranslateTransform trans = child.RenderTransform as TranslateTransform;

          if (trans == null)
          {
            child.RenderTransformOrigin = new Point(0, 0);
            trans = new TranslateTransform();
            child.RenderTransform = trans;
          }

          child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));

          //@@@ jkt doet het niet moet via storyboard http://www.c-sharpcorner.com/UploadFile/anavijai/silverlight-translatetransform-animation-example/
          //trans.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(XPanel.GetXPosition(child), TimeSpan.FromMilliseconds(this.AnimationLength)), HandoffBehavior.Compose);
          //trans.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(XPanel.GetYPosition(child), TimeSpan.FromMilliseconds(this.AnimationLength)), HandoffBehavior.Compose);
        }
        else
        {
          child.Arrange(new Rect(XPanel.GetXPosition(child), XPanel.GetYPosition(child), child.DesiredSize.Width, child.DesiredSize.Height));
        }
      }

      return finalSize;
    }

    //Set the attached properties for all the childs
    private void SetChildProperties(Size availableSize)
    {

      Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
      int curCol = 0, curRow = 0;
      double curX = 0, curY = 0, rowHeight = 0, colWidth = 0, maxWidth = 0, maxHeight = 0;
      Dictionary<int, int> maxColInRow = new Dictionary<int, int>();
      Dictionary<int, int> maxRowInCol = new Dictionary<int, int>();

      if (this.Uniform)
      {
        foreach (UIElement child in Children)
        {
          child.Measure(infiniteSize);
          maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
          maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }
      }
      else if (this.ItemWidth > 0 || this.ItemHeight > 0)
      {
        foreach (FrameworkElement child in Children)
        {
          if (this.ItemWidth > 0) child.Width = this.ItemWidth;
          if (this.ItemHeight > 0) child.Height = this.ItemHeight;
        }
      }

      foreach (UIElement child in Children)
      {

        child.Measure(infiniteSize);

        if (this.Columns > 0)
        {
          if (curCol == this.Columns)
          {
            curRow += 1;
            curCol = 0;
            curX = 0;
            curY += rowHeight;
            rowHeight = 0;
          }
        }
        else if (this.Rows > 0)
        {
          if (curRow == this.Rows)
          {
            curCol += 1;
            curRow = 0;
            curY = 0;
            curX += colWidth;
            colWidth = 0;
          }
        }
        else if (this.Orientation == Orientation.Horizontal)
        {
          if (curX + child.DesiredSize.Width > availableSize.Width)
          {
            curRow += 1;
            curCol = 0;
            curX = 0;
            curY += rowHeight;
            rowHeight = 0;
          }
        }
        else if (this.Orientation == Orientation.Vertical)
        {
          if (curY + child.DesiredSize.Height > availableSize.Height)
          {
            curCol += 1;
            curRow = 0;
            curY = 0;
            curX += colWidth;
            colWidth = 0;
          }
        }

        XPanel.SetXPosition(child, curX);
        XPanel.SetYPosition(child, curY);
        XPanel.SetRow(child, curRow);
        XPanel.SetColumn(child, curCol);

        if (maxColInRow.ContainsKey(curRow)) maxColInRow[curRow] = curCol; else maxColInRow.Add(curRow, curCol);
        if (maxRowInCol.ContainsKey(curCol)) maxRowInCol[curCol] = curRow; else maxRowInCol.Add(curCol, curRow);

        if (this.Columns > 0 || this.Orientation == Orientation.Horizontal)
        {
          curCol += 1;
          curX += this.Uniform ? maxWidth : child.DesiredSize.Width;
          rowHeight = this.Uniform ? maxHeight : Math.Max(rowHeight, child.DesiredSize.Height);
        }
        else if (this.Rows > 0 || this.Orientation == Orientation.Vertical)
        {
          curRow += 1;
          curY += this.Uniform ? maxHeight : child.DesiredSize.Height;
          colWidth = this.Uniform ? maxWidth : Math.Max(colWidth, child.DesiredSize.Width);
        }

      }

      foreach (UIElement child in Children)
      {
        XPanel.SetIsLastCellInColumn(child, maxRowInCol[XPanel.GetColumn(child)] == XPanel.GetRow(child));
        XPanel.SetIsLastCellInRow(child, maxColInRow[XPanel.GetRow(child)] == XPanel.GetColumn(child));
      }

    }

  }

}
