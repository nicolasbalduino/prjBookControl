using MongoDB.Driver;

namespace prjBookControl
{
    internal class Conect
    {
        public string Url { get; set; }
        public string Port { get; set; }
        public string DbName { get; set; }

        public Conect()
        {
            this.Url = "localhost";
            this.Port = "27017";
            this.DbName = "Library";
        }

        public IMongoDatabase MongoDB()
        {
            string urlConection = "mongodb://" + this.Url + ":" + this.Port;
            MongoClient mongo = new MongoClient(urlConection);
            var library = mongo.GetDatabase(this.DbName);

            return library;
        }
    }
}
