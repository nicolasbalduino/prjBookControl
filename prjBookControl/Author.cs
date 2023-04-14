using MongoDB.Bson.Serialization.Attributes;

namespace prjBookControl
{
    internal class Author
    {
        [BsonElement("name")]
        public string Name { get; set; }

        public Author(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
