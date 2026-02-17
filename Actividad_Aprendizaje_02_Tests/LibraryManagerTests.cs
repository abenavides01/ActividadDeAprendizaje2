using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Actividad_Aprendizaje_02.Services;

namespace Actividad_Aprendizaje_02_Tests
{
    [TestFixture]
    public class LibraryManagerTests
    {
        private LibraryManager _library;

        [SetUp]
        public void Setup()
        {
            _library = new LibraryManager();
        }

        // ---------- AddBook ----------

        [Test]
        public void AddBook_ValidBook_AddsSuccessfully()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            var all = _library.ListAllBooks();
            Assert.That(all.Count, Is.EqualTo(1));
            Assert.That(all[0].Isbn, Is.EqualTo("ISBN-001"));
            Assert.That(all[0].IsLoaned, Is.False);
        }

        [Test]
        public void AddBook_DuplicateIsbn_ThrowsInvalidOperationException()
        {
            _library.AddBook("Book 1", "Author 1", "ISBN-001", 2010);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                _library.AddBook("Book 2", "Author 2", "ISBN-001", 2015));

            Assert.That(ex!.Message, Does.Contain("Ya existe un libro con ese ISBN"));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void AddBook_IsbnNullOrWhiteSpace_ThrowsArgumentException(string invalidIsbn)
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                _library.AddBook("Clean Code", "Robert C. Martin", invalidIsbn, 2008));

            Assert.That(ex!.Message, Does.Contain("ISBN es obligatorio"));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void AddBook_TitleNullOrWhiteSpace_ThrowsArgumentException(string invalidTitle)
        {
            // Esto lo valida Book.ValidateRequired
            Assert.Throws<ArgumentException>(() =>
                _library.AddBook(invalidTitle, "Robert C. Martin", "ISBN-001", 2008));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void AddBook_AuthorNullOrWhiteSpace_ThrowsArgumentException(string invalidAuthor)
        {
            // Esto lo valida Book.ValidateRequired
            Assert.Throws<ArgumentException>(() =>
                _library.AddBook("Clean Code", invalidAuthor, "ISBN-001", 2008));
        }

        [Test]
        public void AddBook_PublicationYearInFuture_ThrowsArgumentOutOfRangeException()
        {
            int future = DateTime.Now.Year + 1;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", future));
        }

        [Test]
        public void AddBook_PublicationYearNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", -1));
        }

        [Test]
        public void AddBook_IsbnTrimsSpaces_StoresTrimmedIsbn()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "   ISBN-001   ", 2008);

            var all = _library.ListAllBooks();
            Assert.That(all[0].Isbn, Is.EqualTo("ISBN-001"));
        }

        // ---------- RemoveBookByIsbn ----------

        [Test]
        public void RemoveBookByIsbn_ExistingBook_ReturnsTrueAndRemoves()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            bool removed = _library.RemoveBookByIsbn("ISBN-001");

            Assert.That(removed, Is.True);
            Assert.That(_library.ListAllBooks().Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveBookByIsbn_NonExistingBook_ReturnsFalse()
        {
            bool removed = _library.RemoveBookByIsbn("NOPE");

            Assert.That(removed, Is.False);
        }

        [TestCase("")]
        [TestCase("   ")]
        public void RemoveBookByIsbn_IsbnEmptyOrWhiteSpace_ReturnsFalse(string isbn)
        {
            Assert.That(_library.RemoveBookByIsbn(isbn), Is.False);
        }

        // ---------- SearchBooksByTitle ----------

        [Test]
        public void SearchBooksByTitle_PartialMatch_ReturnsMatches_CaseInsensitive()
        {
            _library.AddBook("Harry Potter and the Goblet of Fire", "J.K. Rowling", "HP-4", 2000);
            _library.AddBook("Harry Potter and the Chamber of Secrets", "J.K. Rowling", "HP-2", 1998);
            _library.AddBook("The Hobbit", "J.R.R. Tolkien", "HOB-1", 1937);

            var result = _library.SearchBooksByTitle("harry");

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(b => b.Title.Contains("Harry", StringComparison.OrdinalIgnoreCase)), Is.True);
        }

        [Test]
        public void SearchBooksByTitle_NoMatch_ReturnsEmptyList()
        {
            _library.AddBook("The Hobbit", "J.R.R. Tolkien", "HOB-1", 1937);

            var result = _library.SearchBooksByTitle("No existe");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void SearchBooksByTitle_EmptyInput_ReturnsEmptyList(string input)
        {
            _library.AddBook("The Hobbit", "J.R.R. Tolkien", "HOB-1", 1937);

            var result = _library.SearchBooksByTitle(input);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        // ---------- ListAllBooks ----------

        [Test]
        public void ListAllBooks_WhenEmpty_ReturnsEmptyList()
        {
            var books = _library.ListAllBooks();
            Assert.That(books, Is.Not.Null);
            Assert.That(books.Count, Is.EqualTo(0));
        }

        [Test]
        public void ListAllBooks_ReturnsCopy_NotSameReference()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            var list1 = _library.ListAllBooks();
            list1.Clear(); // intentar modificar la lista externa

            var list2 = _library.ListAllBooks();
            Assert.That(list2.Count, Is.EqualTo(1)); // si devuelve copia, no debería afectarse
        }

        // ---------- IsBookAvailable ----------

        [Test]
        public void IsBookAvailable_WhenExistsAndNotLoaned_ReturnsTrue()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            bool available = _library.IsBookAvailable("ISBN-001");

            Assert.That(available, Is.True);
        }

        [Test]
        public void IsBookAvailable_WhenLoaned_ReturnsFalse()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);
            _library.LoanBook("ISBN-001");

            bool available = _library.IsBookAvailable("ISBN-001");

            Assert.That(available, Is.False);
        }

        [Test]
        public void IsBookAvailable_WhenNotExists_ReturnsFalse()
        {
            bool available = _library.IsBookAvailable("NOPE");
            Assert.That(available, Is.False);
        }

        [TestCase("")]
        [TestCase("   ")]
        public void IsBookAvailable_IsbnEmptyOrWhiteSpace_ReturnsFalse(string isbn)
        {
            Assert.That(_library.IsBookAvailable(isbn), Is.False);
        }

        // ---------- LoanBook ----------

        [Test]
        public void LoanBook_AvailableBook_MarksAsLoaned()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            _library.LoanBook("ISBN-001");

            var book = _library.ListAllBooks().Single(b => b.Isbn == "ISBN-001");
            Assert.That(book.IsLoaned, Is.True);
        }

        [Test]
        public void LoanBook_AlreadyLoaned_ThrowsInvalidOperationException()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);
            _library.LoanBook("ISBN-001");

            var ex = Assert.Throws<InvalidOperationException>(() => _library.LoanBook("ISBN-001"));
            Assert.That(ex!.Message, Does.Contain("ya está prestado"));
        }

        [Test]
        public void LoanBook_NonExistingIsbn_ThrowsKeyNotFoundException()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _library.LoanBook("NOPE"));
            Assert.That(ex!.Message, Does.Contain("No se encontró"));
        }

        // ---------- ReturnBook ----------

        [Test]
        public void ReturnBook_LoanedBook_MarksAsAvailable()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);
            _library.LoanBook("ISBN-001");

            _library.ReturnBook("ISBN-001");

            Assert.That(_library.IsBookAvailable("ISBN-001"), Is.True);
        }

        [Test]
        public void ReturnBook_NotLoaned_ThrowsInvalidOperationException()
        {
            _library.AddBook("Clean Code", "Robert C. Martin", "ISBN-001", 2008);

            var ex = Assert.Throws<InvalidOperationException>(() => _library.ReturnBook("ISBN-001"));
            Assert.That(ex!.Message, Does.Contain("no está prestado"));
        }

        [Test]
        public void ReturnBook_NonExistingIsbn_ThrowsKeyNotFoundException()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _library.ReturnBook("NOPE"));
            Assert.That(ex!.Message, Does.Contain("No se encontró"));
        }
    }
}
