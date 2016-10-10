using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmdmProject.Models
{
    public class Accord
    {
        public int AccordId { get; set; }
        public string AccordName { get; set; }
        public string Img64 { get; set; }
        public string SongName { get; set; }
        public ICollection<Song> Songs { get; set; }
        public Accord()
        {
            Songs = new List<Song>();
        }
    }
}