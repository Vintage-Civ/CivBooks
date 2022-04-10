using ProtoBuf;
using System.Linq;
using System.Runtime.InteropServices;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModInfo("CivBooks",
    Description = "Adds ink, paper, books, pen, quill and more to write with!",
    Side = "Universal",
    Authors = new[] { "Christoph Clouser", "Novocain", "SouthernBloc" },
    Version = "1.3.4")]

namespace CivBooks
{
    public class BBooks : ModSystem
    {
        const string librarySize = @"books-librarysize";

        public ICoreClientAPI capi { get; private set; }

        public ICoreServerAPI sapi { get; private set; }

        public BookData GetBook(int id)
        {
            return sapi.WorldManager.SaveGame.GetData<BookData>(@"books-data" + id);
        }

        public void SaveBook(BookData book)
        {
            if (LibrarySize < book.id) SetLibrarySize(book.id);

            sapi.WorldManager.SaveGame.StoreData(@"books-data-" + book.id, book);
        }

        public long LibrarySize
        {
            get => sapi.WorldManager.SaveGame.GetData(librarySize, 0L);
        }

        private void SetLibrarySize(long size)
        {
            
            sapi.WorldManager.SaveGame.StoreData(librarySize, size);
        }

        public override void Start(ICoreAPI api)
        {
            capi = api as ICoreClientAPI;
            sapi = api as ICoreServerAPI;

            api.RegisterBlockClass("BlockBooks", typeof(BlockBooks));
            api.RegisterBlockEntityClass("BlockEntityBooks", typeof(BlockEntityBooks));
            api.RegisterBlockClass("BlockPaper", typeof(BlockPaper));
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
        }
    }
}