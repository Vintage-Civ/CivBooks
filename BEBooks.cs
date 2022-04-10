using System;
using System.IO;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace CivBooks
{
    public class BlockEntityBooks : BlockEntity
    {
        public ICoreClientAPI Capi;
        public ICoreServerAPI Sapi;

        public long bookId;
        public bool original;
        public bool isPaper;

        public BookData Book { 
            get 
            {
                if (Capi != null)
                {
                    booksSystem.ReqBook(bookId);
                    return booksSystem.clientBooks[bookId];
                }
                else
                {
                    return booksSystem.GetBook(bookId);
                }
            }
            set
            {
                if (Sapi != null)
                {
                    booksSystem.SaveBook(value);
                }
            }
        }

        internal BooksAnimationHandler animHandler;

        internal BooksSystem booksSystem;

        public BlockEntityAnimationUtil animUtil
        {
            get { return GetBehavior<BEBehaviorAnimatable>()?.animUtil; }
        }

        public void ExportBook()
        {
            string booksPath = Path.Combine(GamePaths.DataPath, "books");
            Directory.CreateDirectory(booksPath);
            
            var book = Book;

            string authors = "";

            int j = 0;
            foreach (var author in book.authorIds)
            {
                string sepstr = j > 0 ? ", " : "";
                authors += sepstr + Capi.World.PlayerByUid(author)?.PlayerName;
                j++;
            }

            booksPath = Path.Combine(booksPath, string.Format("{0} by {1}.html", book.title, authors));

            Task.Factory.StartNew(() =>
            {
                using (TextWriter tw = new StreamWriter(booksPath))
                {
                    tw.Write("<p>");
                    int foundPages = 0;
                    for (int i = 0; i < book.pages.Count; i++)
                    {
                        string text = book.pages[i].content;
                        string title = book.pages[i].title;

                        if (text == "") continue;
                        if (foundPages > 0) tw.Write("<br>");
                        foundPages++;
                        tw.Write(string.Format("Page {0}:{1}", i + 1, "<br>"));
                        if (title != null)
                        {
                            tw.Write(string.Format("{0}{1}", title, "<br>"));
                        }
                        tw.Write(string.Format("{0}{1}", text, "<br>"));

                    }
                    tw.Write("</p>");
                }
            });
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            return animHandler?.hideDrawModel ?? false;
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            this.Api = api;

            booksSystem = api.ModLoader.GetModSystem<BooksSystem>();

            if (api is ICoreClientAPI capi && !isPaper)
            {
                animHandler = new BooksAnimationHandler(capi, this);
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);

            bookId = tree.GetLong("book-id");
            original = tree.GetBool("book-original");
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);

            tree.SetLong("book-id", bookId);
            tree.SetBool("book-original", original);
        }

        public override void OnBlockBroken(IPlayer byPlayer = null)
        {
            animHandler?.Dispose();
            base.OnBlockBroken(byPlayer);
        }

        public void OnRightClick(IPlayer byPlayer, bool isPaper)
        {
            string controlRW = flag_R;

            ItemSlot hotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

            if (isPaper)
                this.isPaper = isPaper;

            if (byPlayer?.Entity?.Controls?.Sneak == true)
            {
                if (hotbarSlot?.Itemstack?.ItemAttributes?["quillink"].Exists == true)
                {
                    tempStack = hotbarSlot.TakeOut(1);
                    hotbarSlot.MarkDirty();
                    controlRW = flag_W;
                }
                else if (hotbarSlot?.Itemstack?.ItemAttributes?["pen"].Exists == true)
                {
                    tempStack2 = hotbarSlot.TakeOut(1);
                    hotbarSlot.MarkDirty();
                    controlRW = flag_W;
                }
                else
                {
                    tempStack = null;
                    tempStack2 = null;
                }
            }

            if (Api.World is IServerWorldAccessor)
            {
                byte[] data;
                // Server sets author for now:
                if (byPlayer.PlayerUID != "")
                {
                    Author = byPlayer.PlayerName;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write(NetworkName);
                    writer.Write(PageMax);
                    for (int i = 0; i < PageMax; i++)
                    {
                        writer.Write(arText[i]);
                    }
                    writer.Write(Title);
                    writer.Write(controlRW);
                    writer.Write(Unique);
                    writer.Write(Author);

                    data = ms.ToArray();
                }

                ((ICoreServerAPI)Api).Network.SendBlockEntityPacket(
                    (IServerPlayer)byPlayer,
                    Pos.X, Pos.Y, Pos.Z,
                    (int)EnumBookPacketId.OpenDialog,
                    data
                );
            }
        }

        public override void OnReceivedClientPacket(IPlayer player, int packetid, byte[] data)
        {
            if (packetid == (int)EnumBookPacketId.SaveBook)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    BinaryReader reader = new BinaryReader(ms);
                    PageMax = reader.ReadInt32();
                    for (int i = 0; i < PageMax; i++)
                    {
                        arText[i] = reader.ReadString();
                    }
                    Title = reader.ReadString();
                    Unique = reader.ReadBoolean();
                    Author = reader.ReadString();
                }

                // Player as author:
                if (player.PlayerUID != "")
                {
                    Author = player.PlayerName;
                }

                MarkDirty(true);
                Api.World.BlockAccessor.GetChunkAtBlockPos(Pos.X, Pos.Y, Pos.Z).MarkModified();
            }

            if (packetid == (int)EnumBookPacketId.CancelEdit && tempStack != null)
            {
                player.InventoryManager.TryGiveItemstack(tempStack);
            }
            else if (packetid == (int)EnumBookPacketId.CancelEdit && tempStack2 != null)
            {
                player.InventoryManager.TryGiveItemstack(tempStack2);
            }
            else if (tempStack != null)
            {
                if (Api.World.BlockAccessor.GetBlock(new AssetLocation("books:inkwell-empty")) != null)
                {
                    // always give back inkwell to refill
                    ItemStack isInkewellEmpty = new ItemStack(Api.World.GetBlock(new AssetLocation("books:inkwell-empty")), 1);
                    Api.World.SpawnItemEntity(isInkewellEmpty, player.CurrentBlockSelection.Position.ToVec3d());
                }
                if (Api.World.GetItem(new AssetLocation("books:itemquill")) != null)
                {
                    // quill might break 30% of times:
                    int
                        max = 9,
                        min = 0,
                        chance = 7;
                    Random rand = new Random();
                    if (rand.Next(min, max) < chance)
                    {
                        ItemStack isItemquill = new ItemStack(Api.World.GetItem(new AssetLocation("books:itemquill")), 1);
                        Api.World.SpawnItemEntity(isItemquill, player.CurrentBlockSelection.Position.ToVec3d());
                    }
                }
            }
            tempStack = null;
            tempStack2 = null;
        }

        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            // TODO: populate BooksNetworkHandler, sloppy for now:
            if (packetid == (int)EnumBookPacketId.OpenDialog)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    BinaryReader reader = new BinaryReader(ms);

                    string dialogClassName = reader.ReadString();
                    PageMax = reader.ReadInt32();
                    for (int i = 0; i < PageMax; i++)
                    {
                        arText[i] = reader.ReadString();
                    }
                    Title = reader.ReadString();
                    string controlRW = reader.ReadString();
                    bool unique = reader.ReadBoolean();
                    string author = reader.ReadString();

                    IClientWorldAccessor clientWorld = (IClientWorldAccessor)Api.World;

                    if (controlRW.Equals(flag_W))
                    {
                        BooksGui BGuiWrite = new BooksGui(isPaper, unique, Title, arText, PageMax, Api as ICoreClientAPI, IDDialogBookEditor);
                        BGuiWrite.WriteGui(Pos, Api as ICoreClientAPI);
                        BGuiWrite.OnCloseCancel = () =>
                        {
                            animHandler?.Close();

                            (Api as ICoreClientAPI)
                            .Network
                            .SendBlockEntityPacket(
                                Pos.X, Pos.Y, Pos.Z,
                                (int)EnumBookPacketId.CancelEdit,
                                null);
                        };
                        BGuiWrite?.TryOpen();
                    }
                    else
                    {
                        BooksGui BGuiRead = new BooksGui(isPaper, unique, Title, arText, PageMax, Api as ICoreClientAPI, IDDialogBookReader);
                        BGuiRead.ReadGui(Pos, Api as ICoreClientAPI);
                        BGuiRead.OnCloseCancel = () =>
                        {
                            animHandler?.Close();
                            (Api as ICoreClientAPI)
                            .Network
                            .SendBlockEntityPacket(
                                Pos.X, Pos.Y, Pos.Z,
                                (int)EnumBookPacketId.CancelEdit,
                                null);
                        };
                        BGuiRead?.TryOpen();
                    }

                    animHandler?.Open();
                }
            }
        }
    }

    public enum EnumBookPacketId
    {
        OpenDialog = 5301,
        SaveBook = 5302,
        CancelEdit = 5303
    }
}