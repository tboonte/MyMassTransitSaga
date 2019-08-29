using MyConsumer.Services;
using System;
using Topshelf;

namespace MyConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(X => X.Service<ConsumerService>());
        }
    }
}
