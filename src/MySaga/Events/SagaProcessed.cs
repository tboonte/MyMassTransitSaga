using MyModel.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySaga.Events
{
    public class SagaProcessed : ISagaProcessed
    {
        public SagaProcessed(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
