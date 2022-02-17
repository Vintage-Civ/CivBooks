﻿using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModInfo("CivBooks",
    Description = "Adds ink, paper, books, pen, quill and more to write with!",
    Side = "Universal",
    Authors = new[] { "Christoph Clouser", "Novocain", "SouthernBloc" },
    Version = "1.3.0")]

namespace CivBooks
{
    public class BBooks : ModSystem
    {
        // Client:
        public ICoreClientAPI capi { get; private set; }

        public IClientNetworkChannel CChannel { get; private set; }

        // Server:
        public ICoreServerAPI sapi { get; private set; }

        public IServerNetworkChannel SChannel { get; private set; }

        public override bool ShouldLoad(EnumAppSide side)
        {
            return true;
        }

        public override void Start(ICoreAPI api)
        {
            api.RegisterBlockClass("BlockBooks", typeof(BlockBooks));
            api.RegisterBlockEntityClass("BlockEntityBooks", typeof(BlockEntityBooks));
            api.RegisterBlockClass("BlockPaper", typeof(BlockPaper));
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            this.sapi = sapi;
        }
    }
}