using System;
using System.Collections.Generic;
using System.Text;

namespace Actividad_Aprendizaje_02.Models
{
    internal class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public int PublicationYear { get; set; }
        public bool IsLoaned { get; private set; }
    }
}
