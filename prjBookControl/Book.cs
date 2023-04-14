using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace prjBookControl
{
    internal class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("edition")]
        public int Edition { get; set; }

        [BsonElement("ISBN")]
        public string Isbn { get; set; }

        [BsonElement("reading")]
        public bool Reading { get; set; }

        [BsonElement("lended")]
        public bool Lended { get; set; }

        [BsonElement("author")]
        public List<Author> Authors { get; set; }

        public Book(string title, int edition, string isbn, bool reading, bool lended, List<Author> authors)
        {
            Title = title;
            Edition = edition;
            Isbn = isbn;
            Reading = reading == null ? false : reading;
            Lended = lended == null ? false : lended;
            Authors = authors;
        }

        public bool EditBook(string title, int edition, string isbn, char reading, char lended, List<Author> authors)
        {
            int countNotEdited = 0;

            if (title != "")
                Title = title;
            else
                countNotEdited++;

            if (edition != 0)
                Edition = edition;
            else
                countNotEdited++;

            if (isbn != "")
                Isbn = isbn;
            else
                countNotEdited++;

            if (reading == 'S' && IsPossibleExchange(true, false))
                Reading = true;
            else 
                if(reading == 'N')
                    Reading = false;
                else
                    countNotEdited++;

            if (lended == 'S' && IsPossibleExchange(false, true))
                Lended = true;
            else 
                if (lended == 'N')
                    Lended = false;
                else
                    countNotEdited++;

            if (authors.Count > 0)
                Authors = authors;
            else
                countNotEdited++;

            if (countNotEdited > 5) return false; return true;
        }

        public bool IsPossibleExchange(bool reading, bool lended)
        {
            if (reading)
            {
                if (this.Lended) 
                {
                    Console.WriteLine("\nVocê não pode ler este livro no momento, pois ele esta emprestado\n");
                    return false;
                }
            }
            if (lended)
            {
                if(this.Reading) 
                {
                    Console.WriteLine("\nVocê não pode emprestar este livro no momento, pois você esta lendo ele\n");
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            string aux = "";
            foreach (Author author in Authors)
            {
                aux += " " + author.ToString() + ",";
            }
            if (aux != "")
            {
                aux = aux.Remove(aux.Length - 1);
            }

            return $"Titulo: {Title}\nEdição: {Edition}\nISBN: {Isbn}\nAutor(es):{aux}\n";
        }

        public string ToBackup()
        {
            string aux = "";
            foreach (Author author in Authors)
            {
                aux += "|" + author.ToString();
            }

            return $"{Title}|{Edition}|{Isbn}|{Reading}|{Lended}{aux}";
        }

        public BsonArray AuthorsToBsonArray()
        {
            BsonArray array = new BsonArray();
            foreach (var author in this.Authors)
            {
                BsonDocument authorBson = new BsonDocument("name", author.ToString());
                array.Add(authorBson);
            }
            return array ;
        }
    }
}
