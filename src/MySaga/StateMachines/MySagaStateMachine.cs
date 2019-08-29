using Automatonymous;
using MyModel;
using MyModel.Commands;
using MyModel.Events;
using MySaga.Events;
using MySaga.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySaga.StateMachines
{
    public class MySagaStateMachine : MassTransitStateMachine<SagaState>
    {
        public State Processing { get; private set; }
        public Event<ISagaRequest> SagaRequested { get; private set; }
        public Event<IConsumerProcessed> ConsumerProcessed { get; private set; }

        public MySagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            this.Event(() => this.SagaRequested, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.ConsumerProcessed, x => x.CorrelateById(c => c.Message.CorrelationId));

            Initially(
                When(SagaRequested)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Receving Request from Client CorrelationId = {context.Data.CorrelationId}"))
                    .ThenAsync(context => this.SendConsumerRequest<IConsumerRequest>(context))
                    .TransitionTo(Processing));

            During(Processing,
                When(ConsumerProcessed)
                    .ThenAsync(context => Console.Out.WriteAsync($"Receving event from Consumer CorrelationId = {context.Data.CorrelationId}"))
                    .Publish(context => new SagaProcessed(context.Data.CorrelationId))
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        private async Task SendConsumerRequest<TCommand>(BehaviorContext<SagaState, ISagaRequest> context)
            where TCommand : class, IConsumerRequest
        {
            var sendEndPoint = await context.GetSendEndpoint(new Uri($"rabbitmq://192.168.99.100:5672/saga/consumer-request"));
            await sendEndPoint.Send<IConsumerRequest>(new
            {
                context.Data.CorrelationId,
            });
        }
    }
}
