using ProtoBuf;

namespace CivBooks
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public struct BookPage
    {
        public string title;
        public string content;

        public BookPage(string title, string content)
        {
            this.title = title;
            this.content = content;
        }
    }
}