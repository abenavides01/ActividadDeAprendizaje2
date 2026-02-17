using System;
using System.Collections.Generic;
using System.Text;

namespace Actividad_Aprendizaje_02.Models
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public int PublicationYear { get; set; }
        public bool IsLoaned { get; private set; }

        public Book(string title, string author, string isbn, int publicationYear)
        {
            Title = ValidateRequired(title, nameof(title));
            Author = ValidateRequired(author, nameof(author));
            Isbn = ValidateRequired(isbn, nameof(isbn));
            PublicationYear = ValidateYear(publicationYear);
            IsLoaned = false;
        }
        public void Loan()
        {
            if (IsLoaned) throw new InvalidOperationException("El libro ya está prestado.");
            IsLoaned = true;
        }

        public void Return()
        {
            if (!IsLoaned) throw new InvalidOperationException("El libro no está prestado.");
            IsLoaned = false;
        }

        private static string ValidateRequired(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Este campo es obligatorio.", paramName);

            return value.Trim();
        }

        private static int ValidateYear(int year)
        {
            int currentYear = DateTime.Now.Year;
            if (year < 0 || year > currentYear)
                throw new ArgumentOutOfRangeException(nameof(year), $"El año debe estar entre 0 y {currentYear}.");
            return year;
        }

    }
}
