using MassTransit;
using MyModel.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleClient.Consumers
{
    public class ConsoleClientConsumer : IConsumer<ISagaProcessed>
    {
        public async Task Consume(ConsumeContext<ISagaProcessed> context)
        {
            Console.WriteLine($"Consuming event from MySaga CorrelationId = {context.CorrelationId}");
        }
    }
}
