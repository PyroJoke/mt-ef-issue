using System;
using System.Configuration;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using MassTransit.NLogIntegration;
using Topshelf;

namespace masstransit_entityframework_issue
{
    class Program
    {
        static int Main(string[] args)
        {
            IBus writeBus = ConfigureWriteBus();
            string serviceBusEndpoing = ConfigurationManager.AppSettings["azureServiceBus:Endpoint"];
            ISendEndpoint ep = writeBus.GetSendEndpoint(new Uri(serviceBusEndpoing + "sample_queue")).Result;

            for (int i = 0; i < 50; i++)
            {
                ep.Send<StartSagaCommand>(new { UniqueProcessingId = Guid.NewGuid() }).Wait();
            }

            if (args.Length > 1 && args[0] == "setonly") return 0;

            return (int) HostFactory.Run(cfg => cfg.Service(x => new Service()));
        }

        private static IBusControl ConfigureWriteBus()
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.UseNLog();

                cfg.Host(ConfigurationManager.AppSettings["azureServiceBus:ConnectionString"],
                    hcfg => { });
            });

            var observer = new ReceiveObserver(true);
            busControl.ConnectReceiveObserver(observer);

            return busControl;
        }
    }
}
