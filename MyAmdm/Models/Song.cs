using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string lyric { get; set; }
        public int NumberOfView { get; set; }
        [Key]
        [ForeignKey("AuthorOf")]
        public int AuthorId { get; set; }
        public Author AuthorOf { get; set; }

        public virtual ICollection<Accord> Accords { get; set; }
        public Song()
        {
            Accords = new List<Accord>();
        }
    }
}