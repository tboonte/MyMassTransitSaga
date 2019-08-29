using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.RedisIntegration;
using MassTransit.Saga;
using MassTransit.Util;
using MySaga.StateMachines;
using MySaga.States;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace MySaga.Services
{
    public class SagaService : ServiceControl
    {
        private IBusControl _busControl;
        private BusHandle _busHandle;

        public bool Start(HostControl hostControl)
        {
            (this._busControl, this._busHandle) = this.CreateBus();
            return true;
        }

        private (IBusControl, BusHandle) CreateBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(this.ConfigureBus);
            var busHandle = TaskUtil.Await(() => bus.StartAsync());
            return (bus, busHandle);
        }

        private void ConfigureBus(IRabbitMqBusFactoryConfigurator factoryConfigurator)
        {
            var rabbitHost = new Uri("rabbitmq://192.168.99.100:5672/saga");
            var inputQueue = "saga-request";
            var host = factoryConfigurator.Host(rabbitHost, this.ConfigureCredential);
            factoryConfigurator.ReceiveEndpoint(host, inputQueue, this.ConfigureSagaEndPoint);

        }

        private void ConfigureCredential(IRabbitMqHostConfigurator hostConfigurator)
        {
            var user = "guest";
            var password = "guest";

            hostConfigurator.Username(user);
            hostConfigurator.Password(password);
        }

        private void ConfigureSagaEndPoint(IRabbitMqReceiveEndpointConfigurator endpointConfigurator)
        {
            var stateMachine = new MySagaStateMachine();

            try

            {
                var redisConnectionString = "192.168.99.100:6379";
                var redis = ConnectionMultiplexer.Connect(redisConnectionString);

                ///If we switch to RedisSagaRepository and Consumer publish its response too quick,
                ///It seems like the consumer published event reached Saga instance before the state is updated
                ///When it happened, Saga will not process the response event because it is not in the "Processing" state
                //var repository = new RedisSagaRepository<SagaState>(() => redis.GetDatabase());
                var repository = new InMemorySagaRepository<SagaState>();

                endpointConfigurator.StateMachineSaga(stateMachine, repository);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool Stop(HostControl hostControl)
        {
            this._busHandle?.Stop();
            return true;
        }
    }
}
