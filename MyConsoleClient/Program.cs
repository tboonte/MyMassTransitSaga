using MassTransit;
using MassTransit.Util;
using MyConsoleClient.Consumers;
using MyModel.Commands;
using System;

namespace MyConsoleClient
{
    class Program
    {
        private static bool _continueRunning = true;

        static void Main(string[] args)
        {
            var bus = CreateBus();
            Console.WriteLine("Starting Console Client Saga Requester");
            Console.ReadLine();
            while (_continueRunning)
            {
                Guid correlationId = Guid.NewGuid();
                Console.WriteLine($"Requesting CorrelationId = {correlationId}");
                bus.Publish<ISagaRequest>(new { CorrelationId = correlationId });
                Console.WriteLine($"CorrealtionId = {correlationId}");
                string input = Console.ReadLine();
                if (input == "q")
                {
                    _continueRunning = false;
                }
            }
        }
        private static IBus CreateBus()
        {
            var rabbitHost = new Uri("rabbitmq://192.168.99.100:5672/saga");
            var user = "guest";
            var password = "guest";
            var inputQueue = "console-client-request";
            var bus = Bus.Factory.CreateUsingRabbitMq(configurator =>
            {
                var host = configurator.Host(rabbitHost, h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                configurator.ReceiveEndpoint(host, inputQueue, c =>
                {
                    c.Consumer(() => new ConsoleClientConsumer());
                });
            });

            TaskUtil.Await(() => bus.StartAsync());
            return bus;
        }
    }
}
