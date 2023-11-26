using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.

namespace LogReader.Desktop.Models;

public record FileViewModelSettings
(
    string Name,
    List<int> SelectedRecordIndices
)
{
    public FileViewModelSettings DeepCopy()
    {
        return new(Name, new(SelectedRecordIndices));
    }

    public virtual bool Equals(FileViewModelSettings? other)
    {
        return other != null &&
               Name == other.Name &&
               SelectedRecordIndices.SequenceEqual(other.SelectedRecordIndices);
    }
}