using System;
using Automatonymous;

namespace masstransit_entityframework_issue
{
    public class SampleSaga : MassTransitStateMachine<SampleSagaState>
    {
        public State Started { get; set; }
        public State InProgress { get; set; }

        public Event<StartSagaCommand> StartEvent { get; set; }
        public Event<SagaContinueCommand> Continue { get; set; }

        public SampleSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => StartEvent, x => x.CorrelateBy(
                instance => instance.UniqueProcessingId,
                context => (Guid?)context.Message.UniqueProcessingId)
                .SelectId(context => Guid.NewGuid()));

            Event(() => Continue, x => x.CorrelateById(c => c.Message.CorrelationId));

            Initially(
                When(StartEvent)
                    .Then(c =>
                    {
                        c.Instance.UniqueProcessingId = c.Data.UniqueProcessingId;
                        c.Instance.StartTime = DateTime.UtcNow;
                    })
                    .TransitionTo(InProgress)
                );

            During(InProgress,

                When(Continue)
                    .Finalize()

                );

            //SetCompletedWhenFinalized();
        }
    }
}