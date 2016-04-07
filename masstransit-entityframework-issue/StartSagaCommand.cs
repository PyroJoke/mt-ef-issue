using System;

namespace masstransit_entityframework_issue
{
    public interface StartSagaCommand
    {
        Guid UniqueProcessingId { get; }
    }
}