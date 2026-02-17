using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Actividad_Aprendizaje_02.Models;


namespace Actividad_Aprendizaje_02.Services
{
    public class LibraryManager
    {
        private readonly List<Book> _books = new();

        //Agregar un libro

        public void AddBook(string title, string author, string isbn, int publicationYear)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("El ISBN es obligatorio.", nameof(isbn));

            string normalizedIsbn = isbn.Trim();

            if (_books.Any(b => b.Isbn.Equals(normalizedIsbn, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ya existe un libro con ese ISBN.");

            var book = new Book(title, author, normalizedIsbn, publicationYear);
            _books.Add(book);
        }

        // 2) Eliminar un libro por ISBN
        public bool RemoveBookByIsbn(string isbn)
        {
            var book = GetByIsbnOrNull(isbn);
            if (book == null) return false;

            _books.Remove(book);
            return true;
        }

        // 3) Buscar por título (coincidencia parcial)
        public List<Book> SearchBooksByTitle(string partialTitle)
        {
            if (string.IsNullOrWhiteSpace(partialTitle))
                return new List<Book>();

            string query = partialTitle.Trim();

            return _books
                .Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // 4) Listar todos los libros
        public List<Book> ListAllBooks()
        {
            // Devolvemos copia para proteger la lista interna
            return _books.ToList();
        }

        // 5) Verificar disponibilidad (existe y no está prestado)
        public bool IsBookAvailable(string isbn)
        {
            var book = GetByIsbnOrNull(isbn);
            return book != null && !book.IsLoaned;
        }

        // 6) Prestar un libro
        public void LoanBook(string isbn)
        {
            var book = GetByIsbnOrThrow(isbn);
            book.Loan();
        }

        // 7) Devolver un libro
        public void ReturnBook(string isbn)
        {
            var book = GetByIsbnOrThrow(isbn);
            book.Return();
        }

        // Helpers privados
        private Book? GetByIsbnOrNull(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return null;

            string normalizedIsbn = isbn.Trim();

            return _books.FirstOrDefault(b =>
                b.Isbn.Equals(normalizedIsbn, StringComparison.OrdinalIgnoreCase));
        }

        private Book GetByIsbnOrThrow(string isbn)
        {
            var book = GetByIsbnOrNull(isbn);
            if (book == null)
                throw new KeyNotFoundException("No se encontró un libro con ese ISBN.");
            return book;
        }
    }
}
