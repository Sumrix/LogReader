using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;
using System;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// A value converter that modifies the behavior of another converter to act as a combination of <see cref="BindingMode.OneTime"/>
/// and <see cref="BindingMode.OneWayToSource"/> binding modes.
/// </summary>
/// <remarks>
/// It performs conversion using the child converter only once and then caches the result for subsequent conversions.
/// For the ConvertBack operation, it always uses the child converter.
/// </remarks>
public class OneTimeOneWayToSourceConverter : IValueConverter
{
    private bool _isFirstConversion = true;
    private object? _cachedValue;

    /// <summary>
    /// Gets or sets the child converter that is actually used to perform the conversion.
    /// </summary>
    public required IValueConverter ChildConverter { get; set; }

    /// <summary>
    /// Converts the <paramref name="value"/> using the <see cref="ChildConverter"/> only once and caches the result.
    /// Subsequent conversions return the cached value without invoking the <see cref="ChildConverter"/>.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The converted value or the cached value after the first conversion.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (_isFirstConversion)
        {
            _isFirstConversion = false;
            return _cachedValue = ChildConverter.Convert(value, targetType, parameter, culture);
        }

        return _cachedValue;
    }

    /// <summary>
    /// Converts the <paramref name="value"/> back using the <see cref="ChildConverter"/> and updates the cached value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>The converted value.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        _cachedValue = value;
        return ChildConverter.ConvertBack(value, targetType, parameter, culture);
    }
}