using MassTransit;
using MyModel.Commands;
using MyModel.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyConsumer.Consumers
{
    public class LeafConsumer : IConsumer<IConsumerRequest>
    {
        public async Task Consume(ConsumeContext<IConsumerRequest> context)
        {
            ///If MySaga project is using RedisSagaRepository, uncomment await Task.Delay() below
            ///Otherwise, it seems that the Publish message from Consumer will not be processed
            ///If using InMemorySagaRepository, code will work without needing Task.Delay
            ///Maybe I am doing something wrong here with these projects
            ///Or in real life, we probably have code in Consumer that will take a few milliseconds to complete
            ///However, we cannot predict latency between Saga and Redis
            //await Task.Delay(1000);

            Console.WriteLine($"Consuming CorrelationId = {context.Message.CorrelationId}");
            await context.Publish<IConsumerProcessed>(new
            {
                context.Message.CorrelationId,
            });
        }
    }
}
