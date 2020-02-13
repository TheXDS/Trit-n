using System;
using System.Threading;
using TheXDS.MCART.Networking.Server;
using TheXDS.Triton.Services;
using static TheXDS.Triton.Services.CrudAction;

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

    internal class RelayBaronProtocol : ManagedCommandProtocol<Client, CrudAction, CrudAction>
    {
        static RelayBaronProtocol()
        {
            ScanTypeOnCtor = false;
        }

        public RelayBaronProtocol()
        {
            WireUp(Create, Relay);
            WireUp(Read, Relay);
            WireUp(Update, Relay);
            WireUp(Delete, Relay);
        }

        private void Relay(Request request)
        {
            request.Respond(Commit);
            request.Broadcast(request.Command, request.Reader.BaseStream);
        }
    }
}
