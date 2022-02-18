using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModInfo("CivBooks",
    Description = "Adds ink, paper, books, pen, quill and more to write with!",
    Side = "Universal",
    Authors = new[] { "Christoph Clouser", "Novocain", "SouthernBloc" },
    Version = "1.3.1")]

namespace CivBooks
{
    public class BBooks : ModSystem
    {
        public ICoreClientAPI capi { get; private set; }

        public ICoreServerAPI sapi { get; private set; }

        public override void Start(ICoreAPI api)
        {
            capi = api as ICoreClientAPI;
            sapi = api as ICoreServerAPI;

            api.RegisterBlockClass("BlockBooks", typeof(BlockBooks));
            api.RegisterBlockEntityClass("BlockEntityBooks", typeof(BlockEntityBooks));
            api.RegisterBlockClass("BlockPaper", typeof(BlockPaper));
        }
    }
}