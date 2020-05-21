using TheXDS.MCART.Networking.Server;
using TheXDS.Triton.Services;
using static TheXDS.Triton.Services.CrudAction;

namespace RelayBaron.Server
{
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
