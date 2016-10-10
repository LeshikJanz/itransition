using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AmdmProject.Models
{
    public class SongContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Accord> Accords { get; set; }

    }
}