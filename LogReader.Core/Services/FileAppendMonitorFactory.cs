using LogReader.Core.Contracts.Services;

namespace LogReader.Core.Services;

public class FileAppendMonitorFactory : IFileUpdateNotifierFactory
{
    public IFileUpdateNotifier Create()
    {
        return new FileAppendMonitor();
    }
}