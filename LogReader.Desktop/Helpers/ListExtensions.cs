using System;
using System.Collections;

namespace LogReader.Desktop.Helpers;

public static class ListExtensions
{
    public static bool SequenceEqual(this IList first, IList second)
    {
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }

        if (second == null)
        {
            throw new ArgumentNullException(nameof(second));
        }

        if (first is ICollection firstCol && second is ICollection secondCol)
        {
            if (firstCol.Count != secondCol.Count)
            {
                return false;
            }

            if (firstCol is IList firstList && secondCol is IList secondList)
            {
                var count = firstCol.Count;
                for (var i = 0; i < count; i++)
                {
                    if (!Equals(firstList[i], secondList[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        var e1 = first.GetEnumerator();
        var e2 = second.GetEnumerator();
        {
            while (e1.MoveNext())
            {
                if (!(e2.MoveNext() && Equals(e1.Current, e2.Current)))
                {
                    return false;
                }
            }

            return !e2.MoveNext();
        }
    }
}