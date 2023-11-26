using System;

namespace LogReader.Desktop.Helpers;

/// <summary>
/// Provides extension methods for double precision floating-point numbers.
/// </summary>
public static class DoubleExtensions
{
    /// <summary>
    /// Determines whether two double precision floating-point numbers are approximately equal
    /// considering a specified tolerance.
    /// </summary>
    /// <param name="first">The first double number to compare.</param>
    /// <param name="second">The second double number to compare.</param>
    /// <param name="tolerance">The maximum difference between the numbers for them to be considered equal.</param>
    /// <returns><c>true</c> if the numbers are approximately equal within the tolerance; otherwise, <c>false</c>.</returns>
    public static bool ApproximatelyEquals(this double first, double second, double tolerance)
    {
        return Math.Abs(first - second) < tolerance;
    }
}