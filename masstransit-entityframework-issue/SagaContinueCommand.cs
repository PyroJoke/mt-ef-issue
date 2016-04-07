using System;

namespace masstransit_entityframework_issue
{
    public interface SagaContinueCommand
    {
        Guid CorrelationId { get; }
        DateTime EventTime { get; }
    }
}