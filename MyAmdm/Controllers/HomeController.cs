using HtmlAgilityPack;
using MyAmdm.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MyAmdm.Controllers
{
    public class HomeController : Controller
    {
        SongContext context = new SongContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowAuthorsInformationPartial()  //Обработчик кнопки "Подгрузить данные". Вычитываем информацию из базы данных
        {
            ParseAndSaveHandler();
            IEnumerable<Author> authors = context.Authors; //
            ViewBag.Authors = authors; //Сделать передачу через модель
            
            return PartialView();
        }

        /*[HttpGet]
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
        }*/

        

        public void ParseAndSaveHandler() {  // ?????? как назвать такую функцию
            SaveParseInformationToDb(ParseInformationFromAmdm());
        }

        public List<string> ParseInformationFromAmdm()
        {
            List<string> authorNames = new List<string>();
            string htmlUrl = "";
            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            for (int i = 1; i <= 10; i++)
            {
                htmlUrl = "http://amdm.ru/chords/page" + i;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td[@class='artist_name']/a");
                if (NoAltElements != null)
                {
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        //Получаем строчки
                        authorNames.Add(HN.InnerText); //добавление имени в список
                    }
                }
            }
            return authorNames;
        }

        public void CleanDbRequest()
        {
            foreach (var author in context.Authors)
            {
               context.Authors.Remove(author);
            }
            context.SaveChanges();
        }

        public void SaveParseInformationToDb(List<string> authorNames)
        {
            foreach ( var name in authorNames)
            {
                Author author = new Author
                {
                    Name = name
                };
            context.Authors.Add(author);
            }

            context.SaveChanges();
        }


        /*[HttpGet]
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
        }*/
    }
}