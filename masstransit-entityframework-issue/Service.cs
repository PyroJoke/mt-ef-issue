using System.Configuration;
using Automatonymous;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using MassTransit.EntityFrameworkIntegration;
using MassTransit.EntityFrameworkIntegration.Saga;
using MassTransit.NLogIntegration;
using Topshelf;

namespace masstransit_entityframework_issue
{
    class Service : ServiceControl
    {
        private IBusControl _bus;
        public bool Start(HostControl hostControl)
        {
            _bus = ConfigureReadBus();
            _bus.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _bus?.Stop();

            return true;
        }

        private static IBusControl ConfigureReadBus()
        {
            var expertTaggingStateMachine = new SampleSaga();

            SagaDbContextFactory sagaDbContextFactory =
                () => new SagaDbContext<SampleSagaState, SampleSagaMapping>("default");

            var stateMachineRepository = new EntityFrameworkSagaRepository<SampleSagaState>(sagaDbContextFactory);

            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.UseNLog();

                IServiceBusHost host = cfg.Host(ConfigurationManager.AppSettings["azureServiceBus:ConnectionString"],
                    hcfg => { });

                cfg.ReceiveEndpoint(host, "sample_queue", ecfg =>
                {
                    ecfg.StateMachineSaga(expertTaggingStateMachine, stateMachineRepository);
                });

            });

            var observer = new ReceiveObserver(true);
            busControl.ConnectReceiveObserver(observer);

            return busControl;
        }
    }
}