using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace CivBooks
{
    internal class BooksNetworkHandler
    {
        private ICoreClientAPI capi;
        private ICoreServerAPI sapi;

        private IServerNetworkChannel sChannel;
        private IClientNetworkChannel cChannel;
        private BooksSystem booksSystem;

        private const string netName = @"civbooksNet";

        public BooksNetworkHandler(ICoreAPI api)
        {
            capi = api as ICoreClientAPI;
            sapi = api as ICoreServerAPI;

            cChannel = capi?.Network.RegisterChannel(netName);
            sChannel = sapi?.Network.RegisterChannel(netName);

            cChannel?.RegisterMessageType<BookData>();
            sChannel?.RegisterMessageType<BookData>();

            cChannel?.SetMessageHandler<BookData>(OnReceiveServerPacket);
            sChannel?.SetMessageHandler<BookData>(OnReceiveClientPacket);
            
            booksSystem = api.ModLoader.GetModSystem<BooksSystem>();
        }

        public void SendBookPacket(BookData packet)
        {
            cChannel?.SendPacket(packet);
        }

        public void SendBookPacket(IServerPlayer toPlayer, BookData packet)
        {
            sChannel?.SendPacket(packet, toPlayer);
        }

        public void OnReceiveServerPacket(BookData packet)
        {
            booksSystem.clientBooks[packet.id] = packet;
        }

        public void OnReceiveClientPacket(IServerPlayer fromPlayer, BookData packet)
        {
            packet = booksSystem.GetBook(packet.id);
            SendBookPacket(fromPlayer, packet);
        }
    }
}