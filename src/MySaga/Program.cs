using MySaga.Services;
using System;
using Topshelf;

namespace MySaga
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => x.Service<SagaService>());
        }
    }
}
