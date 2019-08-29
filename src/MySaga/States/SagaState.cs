using Automatonymous;
using MassTransit.RedisIntegration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySaga.States
{
    public class SagaState : SagaStateMachineInstance, IVersionedSaga
    {
        public SagaState(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public int Version { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
