using TheXDS.MCART.Networking.Legacy.Server;
using TheXDS.Triton.Services;
using static TheXDS.Triton.Services.CrudAction;

namespace RelayBaron.Server
{
    /// <summary>
    /// Protocolo de notificación de Crud que reenvía información sobre la
    /// acción crud a todos los clientes conectados.
    /// </summary>
    public class RelayBaronProtocol : ManagedCommandProtocol<CrudAction, CrudAction>
    {
        static RelayBaronProtocol()
        {
            ScanTypeOnCtor = false;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="RelayBaronProtocol"/>.
        /// </summary>
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
