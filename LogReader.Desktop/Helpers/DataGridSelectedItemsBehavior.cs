using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// A behavior that synchronizes the selected items of a <see cref="DataGrid"/> with a bindable <see cref="IList{T}"/> property.
/// This allows for two-way binding of the <see cref="DataGrid"/>'s selected items.
/// </summary>
/// <typeparam name="T">The type of the items in the selection list.</typeparam>
public class DataGridSelectedItemsBehavior<T> : Behavior<DataGrid>
{
    /// <summary>
    /// Defines an Avalonia attached property for <see cref="SelectedItems"/>.
    /// </summary>
    public static readonly AttachedProperty<IList<T>?> SelectedItemsProperty =
        AvaloniaProperty.RegisterAttached<DataGridSelectedItemsBehavior<T>, DataGrid, IList<T>?>(
            "SelectedItems", defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the list of selected items.
    /// </summary>
    public IList<T>? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    /// <summary>
    /// Attaches the behavior to the <see cref="DataGrid"/> and subscribes to the <see cref="DataGrid.SelectionChanged"/> event.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.SelectionChanged += OnSelectionChanged;
    }

    /// <summary>
    /// Detaches the behavior from the <see cref="DataGrid"/> and unsubscribes from the <see cref="DataGrid.SelectionChanged"/> event.
    /// </summary>
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }
        base.OnDetaching();
    }

    /// <summary>
    /// Synchronizes the <see cref="SelectedItems"/> property with the <see cref="DataGrid"/>'s selected items when the selection changes.
    /// </summary>
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedItems = AssociatedObject?.SelectedItems;
        if (selectedItems != null && SelectedItems != null && !selectedItems.SequenceEqual((IList)SelectedItems))
        {
            SelectedItems = selectedItems.OfType<T>().ToList();
        }
    }

    /// <summary>
    /// Responds to changes in the associated Avalonia property and updates the <see cref="DataGrid"/>'s selection.
    /// </summary>
    /// <param name="change">Details of the property change.</param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedItemsProperty)
        {
            UpdateDataGridSelection(change.NewValue as IList);
        }
    }

    /// <summary>
    /// Updates the <see cref="DataGrid"/>'s selected items based on a new selection.
    /// </summary>
    /// <param name="newSelection">The new selection to apply to the <see cref="DataGrid"/>.</param>
    private void UpdateDataGridSelection(IList? newSelection)
    {
        var selectedItems = AssociatedObject?.SelectedItems;
        if (selectedItems == null || newSelection == null || selectedItems.SequenceEqual(newSelection))
        {
            return;
        }
        
        // Unsubscribe from the update event to avoid recursion.
        AssociatedObject!.SelectionChanged -= OnSelectionChanged;
        
        selectedItems.Clear();
        foreach (var item in newSelection)
        {
            selectedItems.Add(item);
        }
        
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }
}