using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace CivBooks
{
    internal class BooksNetworkHandler
    {
        internal class BooksPacket
        {

        }

        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;

        private IServerNetworkChannel sChannel;
        private IClientNetworkChannel cChannel;
        
        public BooksNetworkHandler(ICoreAPI api)
        {
            capi = api as ICoreClientAPI;
            sapi = api as ICoreServerAPI;

            cChannel = capi?.Network.RegisterChannel("civbooksNet");
            sChannel = sapi?.Network.RegisterChannel("civbooksNet");
            
            cChannel?.RegisterMessageType<BooksPacket>();
            sChannel?.RegisterMessageType<BooksPacket>();

            cChannel?.SetMessageHandler<BooksPacket>(OnReceiveServerPacket);
            sChannel?.SetMessageHandler<BooksPacket>(OnReceiveClientPacket);
        }

        public void OnReceiveServerPacket(BooksPacket packet)
        {

        }

        public void OnReceiveClientPacket(IServerPlayer fromPlayer, BooksPacket packet)
        {

        }
    }
}