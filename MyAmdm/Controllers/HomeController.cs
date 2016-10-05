using MyAmdm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyAmdm.Controllers
{
    public class HomeController : Controller
    {
        SongContext db = new SongContext();

        public ActionResult Index()
        {
            IEnumerable<Author> authors = db.Authors;
            ViewBag.Authors = authors;

            return View();
        }

        [HttpGet]
        public ActionResult OpenAuthorSongList(int id)
        {
            IEnumerable<Song> songs = db.Songs;
            List<Song> choicedSongs = new List<Song>();
            foreach (var song in songs)
            {
                if (song.AuthorId == id)
                {
                    choicedSongs.Add(song);
                    
                }
            }

            ViewBag.Songs = choicedSongs;

            return View();
        }

        [HttpGet]
        public ActionResult OpenSongLyric(int id)
        {
            IEnumerable<Song> songs = db.Songs;
            foreach (var song in songs)
            {
                if (song.SongId == id)
                {
                    ViewBag.Song = song;
                    break;
                }
            }
            return View();
        }
    }
}