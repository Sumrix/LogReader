namespace LogReader.Core.Contracts.Services;

/// <summary>
/// A factory for creating instances of <see cref="IFileUpdateNotifier"/>.
/// </summary>
public interface IFileUpdateNotifierFactory
{
    /// <summary>
    /// Creates a new instance of an <see cref="IFileUpdateNotifier"/>.
    /// </summary>
    /// <returns>A new <see cref="IFileUpdateNotifier"/> instance.</returns>
    IFileUpdateNotifier Create();
}