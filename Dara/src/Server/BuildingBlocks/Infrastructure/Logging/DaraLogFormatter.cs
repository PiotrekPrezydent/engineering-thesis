using Dara.Shared.Logging;

namespace Dara.BuildingBlocks.Infrastructure.Logging;

public class DaraLogFormatter : SharedLogFormatter
{
    public DaraLogFormatter() : base("DaraLogFormatter")
    {
        _eventTypes.Add((LogsIdsRanges.ServerStart, LogsIdsRanges.ServerEnd), "SEVER");
        _eventTypes.Add((LogsIdsRanges.CoreStart, LogsIdsRanges.CoreEnd), "CORE");
    }
}