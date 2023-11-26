using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// A behavior that allows for two-way binding of a <see cref="Window"/>'s position to a <see cref="PixelPoint"/>,
/// as well as its <see cref="PixelPoint.X"/> and <see cref="PixelPoint.Y"/> coordinates independently.
/// </summary>
public class WindowPositionBehavior : Behavior<Window>
{
    private bool _isUpdatingFromCode;

    /// <summary>
    /// Defines an Avalonia attached property for the position of the window.
    /// </summary>
    public static readonly AttachedProperty<PixelPoint> PositionProperty =
        AvaloniaProperty.RegisterAttached<WindowPositionBehavior, Window, PixelPoint>(
            "Position", defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines an Avalonia attached property for the X coordinate of the window's position.
    /// </summary>
    public static readonly AttachedProperty<int> PositionXProperty =
        AvaloniaProperty.RegisterAttached<WindowPositionBehavior, Window, int>(
            "PositionX", defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines an Avalonia attached property for the Y coordinate of the window's position.
    /// </summary>
    public static readonly AttachedProperty<int> PositionYProperty =
        AvaloniaProperty.RegisterAttached<WindowPositionBehavior, Window, int>(
            "PositionY", defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the window's position.
    /// </summary>
    public PixelPoint Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate of the window's position.
    /// </summary>
    public int PositionX
    {
        get => GetValue(PositionXProperty);
        set => SetValue(PositionXProperty, value);
    }

    /// <summary>
    /// Gets or sets the Y coordinate of the window's position.
    /// </summary>
    public int PositionY
    {
        get => GetValue(PositionYProperty);
        set => SetValue(PositionYProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.PositionChanged += OnPositionChanged;
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.PositionChanged -= OnPositionChanged;
        }
        base.OnDetaching();
    }

    /// <summary>
    /// Synchronizes the <see cref="Position"/>, <see cref="PositionX"/>, and <see cref="PositionY"/> properties
    /// with the <see cref="Window"/>'s position when it changes.
    /// </summary>
    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (_isUpdatingFromCode || AssociatedObject?.Position is not { } position || position == Position)
        {
            return;
        }

        _isUpdatingFromCode = true;
        Position = AssociatedObject.Position;
        PositionX = AssociatedObject.Position.X;
        PositionY = AssociatedObject.Position.Y;
        _isUpdatingFromCode = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_isUpdatingFromCode || AssociatedObject == null)
        {
            return;
        }

        if (change.Property == PositionProperty)
        {
            UpdateWindowPosition((PixelPoint)change.NewValue!);
        }
        else if (change.Property == PositionXProperty)
        {
            UpdateWindowPosition(AssociatedObject.Position.WithX((int)change.NewValue!));
        }
        else if (change.Property == PositionYProperty)
        {
            UpdateWindowPosition(AssociatedObject.Position.WithY((int)change.NewValue!));
        }
    }

    private void UpdateWindowPosition(PixelPoint newPosition)
    {
        if (AssociatedObject!.Position == newPosition)
        {
            return;
        }

        _isUpdatingFromCode = true;
        AssociatedObject.Position = newPosition;
        _isUpdatingFromCode = false;
    }
}