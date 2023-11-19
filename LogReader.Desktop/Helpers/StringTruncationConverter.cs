using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using LogReader.Core.Helpers;

namespace LogReader.Desktop.Helpers;

public static class StringTruncationConverter
{
    public static readonly IValueConverter Left = new LeftConverter();
    public static readonly IValueConverter Right = new RightConverter();

    private class LeftConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && parameter is int maxLength)
            {
                return str.TruncateLeft(maxLength, true);
            }

            return value ?? BindingNotification.Null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingNotification.UnsetValue;
        }
    }

    private class RightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && parameter is int maxLength)
            {
                var eolIndex = str.IndexOfAny(new[]{'\r', '\n'});
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