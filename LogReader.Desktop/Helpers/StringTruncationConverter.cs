using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using LogReader.Core.Helpers;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// Provides converters for truncating a string from the left or the right side.
/// </summary>
public static class StringTruncationConverter
{
    public static readonly IValueConverter Left = new LeftConverter();
    public static readonly IValueConverter Right = new RightConverter();

    /// <summary>
    /// Converter to truncate a string from the left.
    /// </summary>
    private class LeftConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string str && parameter is int maxLength 
                ? str.TruncateLeft(maxLength, true) 
                : value ?? BindingNotification.Null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingNotification.UnsetValue;
        }
    }

    /// <summary>
    /// Converter to truncate a string from the right. If a newline character is found,
    /// it truncates to the newline or the specified maximum length, whichever is shorter.
    /// </summary>
    private class RightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && parameter is int maxLength)
            {
                var eolIndex = str.IndexOfAny(new[] {'\r', '\n'});
                var newLength = eolIndex > 0 ? Math.Min(eolIndex, maxLength) : maxLength;
                return str.TruncateRight(newLength, true);
            }

            return value ?? BindingNotification.Null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingNotification.UnsetValue;
        }
    }
}