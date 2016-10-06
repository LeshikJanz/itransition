using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class Accord
    {
        public int AccordId { get; set; }
        public string AccordName { get; set; }
        public string Img64 { get; set; }


        public virtual ICollection<Song> Songs { get; set; }
        public Accord()
        {
            Songs = new List<Song>();
        }
    }
}