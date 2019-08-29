using System;
using System.Collections.Generic;
using System.Text;

namespace MyModel
{
    public interface IMessage
    {
        Guid CorrelationId { get; }
    }
}
