using System;
using Actividad_Aprendizaje_02.Services;

namespace Actividad_Aprendizaje_02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var library = new LibraryManager();
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Gestor de Biblioteca (CRUD) ===");
                Console.WriteLine("1) Agregar libro");
                Console.WriteLine("2) Eliminar libro por ISBN");
                Console.WriteLine("3) Buscar libros por título");
                Console.WriteLine("4) Listar todos los libros");
                Console.WriteLine("5) Verificar disponibilidad por ISBN");
                Console.WriteLine("6) Prestar libro");
                Console.WriteLine("7) Devolver libro");
                Console.WriteLine("0) Salir");
                Console.Write("\nSeleccione una opción: ");

                string option = Console.ReadLine() ?? "";

                try
                {
                    switch (option)
                    {
                        case "1":
                            AddBookFlow(library);
                            break;
                        case "2":
                            RemoveBookFlow(library);
                            break;
                        case "3":
                            SearchByTitleFlow(library);
                            break;
                        case "4":
                            ListAllFlow(library);
                            break;
                        case "5":
                            CheckAvailabilityFlow(library);
                            break;
                        case "6":
                            LoanBookFlow(library);
                            break;
                        case "7":
                            ReturnBookFlow(library);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Opción inválida.");
                            Pause();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Pause();
                }
            }
        }

        static void AddBookFlow(LibraryManager library)
        {
            Console.Write("Título: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Autor: ");
            string author = Console.ReadLine() ?? "";

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            Console.Write("Año de publicación: ");
            bool okYear = int.TryParse(Console.ReadLine(), out int year);
            if (!okYear) throw new ArgumentException("El año debe ser un número entero.");

            library.AddBook(title, author, isbn, year);
            Console.WriteLine("\n✅ Libro agregado.");
            Pause();
        }

        static void RemoveBookFlow(LibraryManager library)
        {
            Console.Write("ISBN a eliminar: ");
            string isbn = Console.ReadLine() ?? "";

            bool removed = library.RemoveBookByIsbn(isbn);
            Console.WriteLine(removed ? "\n✅ Libro eliminado." : "\n⚠️ No se encontró el libro.");
            Pause();
        }

        static void SearchByTitleFlow(LibraryManager library)
        {
            Console.Write("Título (parcial): ");
            string partial = Console.ReadLine() ?? "";

            var results = library.SearchBooksByTitle(partial);

            Console.WriteLine($"\nResultados: {results.Count}");
            foreach (var b in results)
            {
                Console.WriteLine($"- {b.Title} | {b.Author} | {b.Isbn} | {b.PublicationYear} | {(b.IsLoaned ? "Prestado" : "Disponible")}");
            }
            Pause();
        }

        static void ListAllFlow(LibraryManager library)
        {
            var books = library.ListAllBooks();

            if (books.Count == 0)
            {
                Console.WriteLine("\n(No hay libros registrados)");
                Pause();
                return;
            }

            Console.WriteLine("\nListado completo:");
            foreach (var b in books)
            {
                Console.WriteLine($"- {b.Title} | {b.Author} | {b.Isbn} | {b.PublicationYear} | {(b.IsLoaned ? "Prestado" : "Disponible")}");
            }
            Pause();
        }

        static void CheckAvailabilityFlow(LibraryManager library)
        {
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            bool available = library.IsBookAvailable(isbn);
            Console.WriteLine(available ? "\n✅ Disponible" : "\n❌ No disponible (no existe o está prestado)");
            Pause();
        }

        static void LoanBookFlow(LibraryManager library)
        {
            Console.Write("ISBN a prestar: ");
            string isbn = Console.ReadLine() ?? "";

            library.LoanBook(isbn);
            Console.WriteLine("\n✅ Libro prestado.");
            Pause();
        }

        static void ReturnBookFlow(LibraryManager library)
        {
            Console.Write("ISBN a devolver: ");
            string isbn = Console.ReadLine() ?? "";

            library.ReturnBook(isbn);
            Console.WriteLine("\n✅ Libro devuelto.");
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }
    }
}

