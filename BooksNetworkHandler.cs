using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace CivBooks
{
    internal class BooksNetworkHandler
    {
        private ICoreClientAPI Capi;
        private ICoreServerAPI Sapi;

        public BooksNetworkHandler(ICoreClientAPI capi)
        {
            this.Capi = capi;
        }

        public BooksNetworkHandler(ICoreServerAPI sapi)
        {
            this.Sapi = sapi;
        }

        public void SendToClient(ICoreClientAPI capi)
        {
            Capi = capi;
        }

        public void OnReceive()
        {
        }
    }
}