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
        public OrderedDictionary<int, BookPage> pages;
        public HashSet<string> authorIds;

        internal BookData(long pageCount, long id, string title, OrderedDictionary<int, BookPage> pages, params string[] authors)
        {
            this.pageCount = pageCount;
            this.id = id;
            this.title = title;
            this.pages = pages;
            authorIds = new HashSet<string>(authors);
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