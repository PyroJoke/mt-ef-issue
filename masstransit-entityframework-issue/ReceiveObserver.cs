using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using NLog;

namespace masstransit_entityframework_issue
{
    public class ReceiveObserver : IReceiveObserver
    {
        private readonly bool _logMessageBodyOnPreReceived;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReceiveObserver(bool logMessageBodyOnPreReceived = false)
        {
            _logMessageBodyOnPreReceived = logMessageBodyOnPreReceived;
        }

        public Task PreReceive(ReceiveContext context)
        {
            Logger.Debug($"[MTObserver:PreReceive] input address: {context.InputAddress}; is delivered {context.IsDelivered}; is faulted: {context.IsFaulted}");

            if (_logMessageBodyOnPreReceived)
            {
                Logger.Debug($"[MTObserver:PreReceive] message body: {ReadStreamAsString(context.GetBody())}");
            }

            return Task.FromResult(0);
        }

        public Task PostReceive(ReceiveContext context)
        {
            Logger.Debug($"[MTObserver:PostReceive] input address: {context.InputAddress}; is delivered {context.IsDelivered}; is faulted: {context.IsFaulted}");

            return Task.FromResult(0);
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            Logger.Debug($"[MTObserver:PostConsume] consumer type: {consumerType}; duration: {duration}; message type: {context.Message.GetType().FullName}");

            return Task.FromResult(0);
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            Logger.Error(exception, $"[MTObserver:ConsumeFault] consumer type: {consumerType}; duration: {duration}; message type: {context.Message.GetType().FullName}; exception message: {exception.Message}");

            return Task.FromResult(0);
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            Logger.Error(exception, $"[MTObserver:ReceiveFault] input address: {context.InputAddress}; is delivered {context.IsDelivered}; is faulted: {context.IsFaulted}; exception message: {exception.Message}");

            return Task.FromResult(0);
        }

        private string ReadStreamAsString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}