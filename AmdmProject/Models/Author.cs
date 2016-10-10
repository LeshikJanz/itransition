using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmdmProject.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string LinkOfBiography { get; set; }

        public ICollection<Song> Songs { get; set; }
        public Author()
        {
            Songs = new List<Song>();
        }
    }
}