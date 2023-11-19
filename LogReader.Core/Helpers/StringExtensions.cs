namespace LogReader.Core.Helpers;

public static class StringExtensions
{
    //private const string Ellipsis = "...";
    private const string Ellipsis = "…";

    /// <summary>
    /// Truncates a string to the specified length.
    /// </summary>
    /// <param name="value">The string to be truncated.</param>
    /// <param name="length">The maximum length.</param>
    /// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
    /// <returns>Truncated string.</returns>
    public static string TruncateRight(this string? value, int length, bool ellipsis = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value ?? string.Empty;
        }

        var ellipsisLength = ellipsis ? Ellipsis.Length : 0;
        if (length <= ellipsisLength)
        {
            return Ellipsis[..length];
        }

        value = value.Trim();

        if (value.Length > length)
        {
            return ellipsis
                ? string.Concat(value.AsSpan(0, length - ellipsisLength), Ellipsis)
                : value[..length];
        }

        return value;
    }

    /// <summary>
    /// Truncates a string to the specified length.
    /// </summary>
    /// <param name="value">The string to be truncated.</param>
    /// <param name="length">The maximum length.</param>
    /// <param name="ellipsis"><c>true</c> to add ellipsis to the truncated text; otherwise, <c>false</c>.</param>
    /// <returns>Truncated string.</returns>
    public static string TruncateLeft(this string? value, int length, bool ellipsis = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value ?? string.Empty;
        }

        var ellipsisLength = ellipsis ? Ellipsis.Length : 0;
        if (length <= ellipsisLength)
        {
            return Ellipsis[..length];
        }

        value = value.Trim();

        if (value.Length > length)
        {
            return ellipsis
                ? string.Concat(Ellipsis, value.AsSpan(value.Length - length + ellipsisLength))
                : value[..length];
        }

        return value;
    }
}