using System.ComponentModel.Design;
using prjBookControl;

internal class Program
{
    private static void Main(string[] args)
    {
        List<Book> shelf = new List<Book>();
        List<Book> readings = new List<Book>();
        List<Book> lendeds = new List<Book>();

        LoadBackup();

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
                        shelf.Add(newBook);
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
                    Console.Write("\nDigite o titulo do livro a ser editado: ");
                    title = Console.ReadLine();

                    if(DeleteBook(title))
                        Console.WriteLine("LIVRO REMOVIDO COM SUCESSO!");
                    else
                        Console.WriteLine("LIVRO NÃO REMOVIDO!");
                    break;

                case 5:
                    Console.Clear();
                    if (SaveShelf())
                        Console.WriteLine("\nESTANTE SALVA!\n");
                    else
                        Console.WriteLine("\nFALHA AO SALVAR A ESTANTE!\n");
                    break;

                case 6:
                    System.Environment.Exit(0);
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("\nOpção Invalida. Tente novamente\n");
                    break;
            }
            
        } while (option != 6);


        int Menu()
        {
            Console.WriteLine("MENU DE OPÇÕES" +
                                "\n1 - Inserir Livro" +
                                "\n2 - Imprimir Estante" +
                                "\n3 - Editar Livro" +
                                "\n4 - Remover Livro" +
                                "\n5 - Criar backup" +
                                "\n6 - Sair" +
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

            return new Book(title, edition, isbn, false, false, authors);
        }

        void PrintShelf(int list)
        {
            Console.Clear();

            if(list == 1) 
            {
                if (shelf.Count > 0)
                {
                    Console.WriteLine("\nNA ESTANTE\n");
                    foreach (Book book in shelf)
                        Console.WriteLine(book.ToString());
                }
                else
                    Console.WriteLine("ESTANTE VAZIA!");
            }
            else if(list == 2)
            {
                if (readings.Count > 0)
                {
                    Console.WriteLine("\nLENDO\n");
                    foreach (Book book in readings)
                        Console.WriteLine(book.ToString());
                }
                else
                    Console.WriteLine("VOCÊ NÃO ESTÁ LENDO NENHUM LIVRO!");
            }
            else if( list == 3)
            {
                if (lendeds.Count > 0)
                {
                    Console.WriteLine("\nLIVROS EMPRESTADOS\n");
                    foreach (Book book in lendeds)
                        Console.WriteLine(book.ToString());
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
            Book serchBook = new Book(title, 0, null, false, false, null);
            for (int book = 0; book < shelf.Count; book++)
                if(serchBook.Title == shelf[book].Title)
                {
                    return shelf[book];
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

            //int countAuthors = 0;
            //do
            //{
            //    Console.Write((countAuthors + 1) + "o Autor do livro: ");
            //    author = new Author(Console.ReadLine());
            //    if (author.Name != "")
            //        authors.Add(author);

            //    countAuthors++;
            //} while (author.Name != "");

            int countAuthors = 0;
            foreach (Author authorToEdit in bookToEdit.Authors)
            {
                Console.Write((countAuthors + 1) + "o Autor do livro: ");
                author = new Author(Console.ReadLine());
                
                if (author.Name != "")
                    authors.Add(author);
                else
                    authors.Add(authorToEdit);

                countAuthors++;
            }

            return bookToEdit.EditBook(title, edition, isbn, reading, lended, authors);
        }

        bool DeleteBook(string title)
        {
            Book bookToEdit = FindBook(title);
            if (bookToEdit == null)
            {
                Console.WriteLine("\nLIVRO NÃO ENCONTRADO!\n");
                return false;
            }

            return shelf.Remove(bookToEdit);
        }

        bool SaveShelf()
        {
            StreamWriter sw = new StreamWriter("shelf.txt");

            sw.WriteLine("titulo|edicao|ISBN|lendo|emprestado|autores");

            foreach (Book book in shelf)
                sw.WriteLine(book.ToBackup());

            sw.Close();

            return true;
        }

        bool LoadBackup()
        {
            if (!File.Exists("shelf.txt"))
            {
                Console.WriteLine("\nESTANTE NÃO EXISTE!\n");
                return false;
            }

            StreamReader sr = new StreamReader("shelf.txt");
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                List<Author> authors = new List<Author>();

                string properties = sr.ReadLine();
                string[] property = properties.Split('|');

                string title = property[0];
                int edition = int.Parse(property[1]);
                string isbn = property[2];
                bool reading = bool.Parse(property[3]);
                bool lended = bool.Parse(property[4]);
                for(int i = 5; i < property.Length; i++)
                {
                    Author author = new(property[i]);
                    authors.Add(author);
                }

                Book book = new(title, edition, isbn, reading, lended, authors);

                if(reading)
                    readings.Add(book);
                else if(lended)
                    lendeds.Add(book);
                else
                    shelf.Add(book);
            }
            sr.Close();
            Console.WriteLine("\nESTANTE CARREGADA!\n");
            return true;
        }
    }
}