using HtmlAgilityPack;
using MyAmdm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ParseInformation();
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

        public List<string> ParseInformation()
        {
            List<string> authorNames = new List<string>();
            string html = "http://amdm.ru/chords/";
            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };
            HD = web.Load(html);

            // Собственно, здесь и производится выборка интересующих нодов
            // В данном случае выбираем блочные элементы с классом eTitle
            HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td[@class='artist_name']");

            if (NoAltElements != null)
            {
                foreach (HtmlNode HN in NoAltElements)
                {
                    //Получаем строчки
                   authorNames.Add(HN.InnerText); //добавление имени в список
                }
            }
            return authorNames;
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