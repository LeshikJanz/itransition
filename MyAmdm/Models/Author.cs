using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public string LinkOfBiography { get; set; }

        List<Song> song = new List<Song>();
    }
}