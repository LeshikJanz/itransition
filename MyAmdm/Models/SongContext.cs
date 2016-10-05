using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyAmdm.Models
{
    public class SongContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Accord> Accords { get; set; }
    }
}