using ProtoBuf;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace CivBooks
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public struct BookData
    {
        public const int maxPages = 5000;
        public long pageCount;
        public long id;
        public string title;
        public Dictionary<int, BookPage> pages;
        public HashSet<string> authorIds;

        internal BookData(long pageCount, long id, string title, Dictionary<int, BookPage> pages, params string[] authors)
        {
            this.pageCount = pageCount;
            this.id = id;
            this.title = title;
            this.pages = pages;
            authorIds = new HashSet<string>(authors);
        }

        internal BookData(long id)
        {
            this.pageCount = 0;
            this.id = id;
            this.title = "";
            this.pages = new Dictionary<int, BookPage>();
            authorIds = new HashSet<string>();
        }

        internal static BookData FromBytes(byte[] bytes)
        {
            return new BookData(bytes);
        }

        internal BookData(byte[] bytes)
        {
            this = SerializerUtil.Deserialize<BookData>(bytes);
        }

        internal byte[] Serialize()
        {
            return SerializerUtil.Serialize(this);
        }
    }
}