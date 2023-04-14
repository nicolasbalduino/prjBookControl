using System.ComponentModel.Design;
using System.Security.AccessControl;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Core.Configuration;
using prjBookControl;
using MongoDB.Bson.Serialization;
using static MongoDB.Driver.WriteConcern;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

internal class Program
{
    private static void Main(string[] args)
    {
        var library = new Conect().MongoDB();
        var books = library.GetCollection<BsonDocument>("Books");

        int option;
        do
        {
            option = Menu();

            switch (option)
            {
                case 1:
                    Book newBook;
                    newBook = CreateBook();
                    if (newBook != null)
                        Console.WriteLine("\nLIVRO INSERIDO COM SUCESSO!" +
                            "\n Escolha a opção 5 para salvar e atualizar\n");
                    break;

                case 2:
                    bool catchChoice;
                    Console.Write("Quais livros você gostaria consultar?" +
                        "\n1 - Na Estante" +
                        "\n2 - Lendo" +
                        "\n3 - Emprestado(s)" +
                        "\nSua escolha: ");
                    catchChoice = int.TryParse(Console.ReadLine(), out int choice);
                    PrintShelf(choice);
                    break;

                case 3:
                    string title;
                    Console.Write("\nDigite o titulo do livro a ser editado: ");
                    title = Console.ReadLine();

                    if (EditBook(title))
                        Console.WriteLine("\nLIVRO EDITADO COM SUCESSO!\n");
                    else
                        Console.WriteLine("\nLIVRO NÃO ALTERADO!\n");
                    break;

                case 4:
                    Console.Write("\nDigite o titulo do livro a ser deletado: ");
                    title = Console.ReadLine();

                    if (DeleteBook(title))
                        Console.WriteLine("\nLIVRO REMOVIDO COM SUCESSO!\n Escolha a opção 5 para salvar e atualizar\n");
                    else
                        Console.WriteLine("\nLIVRO NÃO REMOVIDO!\n");
                    break;

                case 5:
                    System.Environment.Exit(0);
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("\nOpção Invalida. Tente novamente\n");
                    break;
            }

        } while (option != 5);


        int Menu()
        {
            Console.WriteLine("MENU DE OPÇÕES" +
                                "\n1 - Inserir Livro" +
                                "\n2 - Imprimir Estante" +
                                "\n3 - Editar Livro" +
                                "\n4 - Remover Livro" +
                                "\n5 - Sair" +
                                "\n\nEscolha uma opção:");

            bool catchOption = int.TryParse(Console.ReadLine(), out int option);

            return option;
        }

        Book CreateBook()
        {
            string title, isbn;
            bool catchEdition;
            Author author;
            List<Author> authors = new List<Author>();

            Console.Write("\nTitulo do livro: ");
            title = Console.ReadLine();

            Console.Write("Edição do livro: ");
            catchEdition = int.TryParse(Console.ReadLine(), out int edition);

            Console.Write("ISBN do livro: ");
            isbn = Console.ReadLine();

            int countAuthors = 0;
            do
            {
                Console.Write((countAuthors + 1) + "o Autor do livro: ");
                author = new Author(Console.ReadLine());
                if (author.Name != "")
                    authors.Add(author);

                countAuthors++;
            } while (author.Name != "");

            Book newBook = new Book(title, edition, isbn, false, false, authors);

            var nb = new BsonDocument
            {
                { "title", newBook.Title },
                { "edition", newBook.Edition },
                { "ISBN", newBook.Isbn },
                { "reading", newBook.Reading },
                { "lended", newBook.Lended },
                {"author", newBook.AuthorsToBsonArray() },
            };

            books.InsertOne(nb);

            return newBook;
        }

        void PrintShelf(int list)
        {
            Console.Clear();

            var builder = Builders<BsonDocument>.Filter;

            if (list == 1)
            {
                var filter = builder.Eq("reading", false) & builder.Eq("lended", false);
                var shelf = books.Find(filter).ToList(); 
                if (shelf.Count > 0)
                {
                    Console.WriteLine("\nNA ESTANTE\n");

                    shelf.ForEach(book => Console.WriteLine(BsonSerializer.Deserialize<Book>(book).ToString()));
                    //foreach (var book in shelf)
                    //{
                    //    Book b = BsonSerializer.Deserialize<Book>(book);
                    //    Console.WriteLine(b.ToString());
                    //}
                }
                else
                    Console.WriteLine("ESTANTE VAZIA!");
            }
            else if (list == 2)
            {
                var filter = builder.Eq("reading", true) & builder.Eq("lended", false);
                var reading = books.Find(filter).ToList();
                if (reading.Count > 0)
                {
                    Console.WriteLine("\nLENDO\n");
                    reading.ForEach(book => Console.WriteLine(BsonSerializer.Deserialize<Book>(book).ToString()));
                }
                else
                    Console.WriteLine("VOCÊ NÃO ESTÁ LENDO NENHUM LIVRO!");
            }
            else if (list == 3)
            {
                var filter = builder.Eq("reading", false) & builder.Eq("lended", true);
                var lended = books.Find(filter).ToList();
                if (lended.Count > 0)
                {
                    Console.WriteLine("\nLIVROS EMPRESTADOS\n");
                    lended.ForEach(book => Console.WriteLine(BsonSerializer.Deserialize<Book>(book).ToString()));
                }
                else
                    Console.WriteLine("NENHUM LIVRO EMPRESTADO!");
            }
            else
            {
                Console.WriteLine("ESCOLHA NÃO ENCONTRADA!");
            }

            Console.WriteLine("Precione um tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }

        Book FindBook(string title)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("title", title);
            var b = books.Find(filter).FirstOrDefault();

            if (b != null)
            {
                Book book = BsonSerializer.Deserialize<Book>(b);
                Console.WriteLine(book.ToString());
                return book;
            }
            return null;
        }

        bool EditBook(string title)
        {
            Book bookToEdit = FindBook(title);
            if (bookToEdit == null)
            {
                Console.WriteLine("\nLIVRO NÃO ENCONTRADO!\n");
                return false;
            }

            string isbn;
            bool catchEdition, catchReading, catchLended;
            Author author;
            List<Author> authors = new List<Author>();

            Console.Write("\nNovo titulo do livro: ");
            title = Console.ReadLine();


            Console.Write("Nova edição do livro: ");
            catchEdition = int.TryParse(Console.ReadLine(), out int edition);


            Console.Write("Novo ISBN do livro: ");
            isbn = Console.ReadLine();

            Console.Write("Lendo o livro [S/N]: ");
            catchReading = char.TryParse(Console.ReadLine().ToUpper(), out char reading);
            
            Console.Write("Livro emprestado [S/N]: ");
            catchLended = char.TryParse(Console.ReadLine().ToUpper(), out char lended);

            int countAuthors = 0;
            int authorNotEdited = 0;
            foreach (Author authorToEdit in bookToEdit.Authors)
            {
                Console.Write((countAuthors + 1) + "o Autor do livro: ");
                author = new Author(Console.ReadLine());
                
                if (author.Name != "")
                    authors.Add(author);
                else
                {
                    authors.Add(authorToEdit);
                    authorNotEdited++;
                }

                countAuthors++;
            }
            if (authorNotEdited == bookToEdit.Authors.Count)
            {
                authors.Clear();
            }

            if (bookToEdit.EditBook(title, edition, isbn, reading, lended, authors))
            {
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq("_id", ObjectId.Parse(bookToEdit.Id));

                var builderUpdate = Builders<BsonDocument>.Update;
                var update = builderUpdate.Set("title", bookToEdit.Title)
                                            .Set("edition", bookToEdit.Edition)
                                            .Set("ISBN", bookToEdit.Isbn)
                                            .Set("reading", bookToEdit.Reading)
                                            .Set("lended", bookToEdit.Lended)
                                            .Set("author", bookToEdit.Authors);
                var updateBook = books.UpdateOne(filter, update);

                if (updateBook.ModifiedCount > 0) return true;
                else return false;
            }

            return false;
        }

        bool DeleteBook(string title)
        {
            Book bookToRemove = FindBook(title);
            if (bookToRemove == null)
            {
                Console.WriteLine("\nLIVRO NÃO ENCONTRADO!\n");
                return false;
            }

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(bookToRemove.Id));
            var br = books.DeleteOne(filter);

            if (br.DeletedCount > 0)
                return true;

            return false;
        }
    }
}