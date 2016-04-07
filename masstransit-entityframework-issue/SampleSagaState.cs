using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Automatonymous;

namespace masstransit_entityframework_issue
{
    public class SampleSagaState : SagaStateMachineInstance
    {
        [Key]
        public Guid CorrelationId { get; set; }

        [Index]
        public Guid? UniqueProcessingId { get; set; }

        public string CurrentState { get; set; }
        public DateTime? StartTime { get; set; }
    }
}