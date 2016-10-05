using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Name { get; set; }
        public string lyric { get; set; } 
        public int AuthorId { get; set; }
    }
}