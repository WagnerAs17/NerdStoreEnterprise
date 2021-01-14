using System;

namespace NSE.Core.Messages.Integrations
{
    public class PedidoRegistradoIntegrationEvent : IntegrationEvent
    {
        public Guid ClienteId { get; private set; }
        public PedidoRegistradoIntegrationEvent(Guid clienteId)
        {
            ClienteId = clienteId;
        }
    }
}
