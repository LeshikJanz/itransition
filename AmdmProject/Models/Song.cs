using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmdmProject.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string lyric { get; set; }
        public string LinkOfSong { get; set; }
        public int NumberOfView { get; set; }

        public int? AuthorId { get; set; }
        public Author Author { get; set; }

        public ICollection<Accord> Accords { get; set; }
        public Song()
        {
            Accords = new List<Accord>();
        }
    }
}