using System;
using System.Threading;
using TheXDS.MCART.Networking.Server;

namespace RelayBaron.Server
{
    internal static class Program
    {
        private static readonly Server<Client> _srv = new Server<Client>(new RelayBaronProtocol(), 61440);

        private static void Main()
        {
            _srv.Start();
            Console.CancelKeyPress += OnExit;
            while (_srv.IsAlive)
            {
                Thread.Sleep(1000);
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            _srv.Stop();   
            Environment.Exit(0);
        }
    }
}
