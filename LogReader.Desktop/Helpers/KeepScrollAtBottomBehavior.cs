using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Interactivity;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// A behavior for <see cref="DataGrid"/> to automatically scroll to the bottom when new items are added,
/// but only if the scroll was already at the bottom.
/// </summary>
public class KeepScrollAtBottomBehavior : Behavior<DataGrid>
{
    private const string VerticalScrollBarPartName = "PART_VerticalScrollbar";
    private const double ScrollTolerance = 80.0;

    private bool _isScrolledToBottom = true;
    private INotifyCollectionChanged? _notifyCollectionChanged;

    /// <summary>
    /// Attaches the behavior to the <see cref="DataGrid"/>.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.PropertyChanged += OnDataGridPropertyChanged;

        if (AssociatedObject!.IsLoaded)
        {
            AttachCollectionChanged();
        }
        else
        {
            AssociatedObject!.Loaded += OnLoaded;
        }
    }

    /// <summary>
    /// Detaches the behavior from the <see cref="DataGrid"/>.
    /// </summary>
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.PropertyChanged -= OnDataGridPropertyChanged;
        }

        DetachCollectionChanged();
        DetachScrollChanged();
        base.OnDetaching();
    }

    private void OnDataGridPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(DataGrid.ItemsSource))
        {
            DetachCollectionChanged();
            AttachCollectionChanged();
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        AssociatedObject!.Loaded -= OnLoaded;
        AttachScrollChanged();
    }

    private void AttachCollectionChanged()
    {
        _notifyCollectionChanged = AssociatedObject!.ItemsSource as INotifyCollectionChanged;
        if (_notifyCollectionChanged != null)
        {
            _notifyCollectionChanged.CollectionChanged += ItemsCollectionChanged;
        }
    }

    private void DetachCollectionChanged()
    {
        if (_notifyCollectionChanged != null)
        {
            _notifyCollectionChanged.CollectionChanged -= ItemsCollectionChanged;
            _notifyCollectionChanged = null;
        }
    }

    private void AttachScrollChanged()
    {
        var scrollViewer = GetVerticalScrollBar();
        if (scrollViewer != null)
        {
            scrollViewer.ValueChanged += OnScrollChanged;
        }
    }

    private void DetachScrollChanged()
    {
        var scrollViewer = GetVerticalScrollBar();
        if (scrollViewer != null)
        {
            scrollViewer.ValueChanged -= OnScrollChanged;
        }
    }

    private void OnScrollChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (sender is ScrollBar scrollBar)
        {
            _isScrolledToBottom = scrollBar.Value.ApproximatelyEquals(scrollBar.Maximum, ScrollTolerance);
        }
    }

    private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && _isScrolledToBottom)
        {
            var lastItem = e.NewItems![^1];
            AssociatedObject!.ScrollIntoView(lastItem, null);
        }
    }

    /// <summary>
    /// Retrieves the vertical scroll bar of the <see cref="DataGrid"/>.
    /// </summary>
    /// <returns>The vertical <see cref="ScrollBar"/> of the <see cref="DataGrid"/>, if it exists.</returns>
    private ScrollBar? GetVerticalScrollBar()
    {
        return AssociatedObject?.GetVisualDescendants()
            .OfType<ScrollBar>()
            .FirstOrDefault(s => s.Name == VerticalScrollBarPartName);
    }
}
