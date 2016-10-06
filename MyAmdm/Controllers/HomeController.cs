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
            //ParseAuthorBiographyFromAmdm(ParseAuthorBiographyLinksFromAmdm());
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
            SaveParseInformationToDb(ParseAuthorNamesFromAmdm(), ParseAuthorBiographyLinksFromAmdm(), ParseAuthorBiographyFromAmdm(ParseAuthorBiographyLinksFromAmdm())); //тут еще страшно, но это будет работать только один раз, чтобы пропарсить все
        }

        public List<string> ParseAuthorBiographyFromAmdm(List<string> AuthorLinks)
        {
            List<string> authorBiography = new List<string>();
            string htmlUrl = "";

            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            foreach (var authorLink in AuthorLinks)
            {
                htmlUrl = "http:" + authorLink;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//div[@class='artist-profile__bio']");
                if (NoAltElements != null)
                {
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        string outputText = HN.InnerText;
                        if (flag) // считываем каждый второй
                        {
                            authorBiography.Add(outputText); //добавление имени в список
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
            }



            /*for (int i = 1; i <= 10; i++)
            {
                htmlUrl = "http://amdm.ru/chords/page" + i;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td/a[@href]");
                if (NoAltElements != null)
                {
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        string outputText = HN.Attributes["href"].Value;
                        if (flag) // считываем каждый второй
                        {
                            authorBiography.Add(outputText); //добавление имени в список
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
            }*/
            return authorBiography;
        }

        public List<string> ParseAuthorBiographyLinksFromAmdm()
        {
            List<string> authorBiographyLinks = new List<string>();
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
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td/a[@href]");
                if (NoAltElements != null)
                {
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        string outputText = HN.Attributes["href"].Value;
                        if (flag) // считываем каждый второй
                        {
                            authorBiographyLinks.Add(outputText); //добавление имени в список
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
            }
            return authorBiographyLinks;
        }

        public List<string> ParseAuthorNamesFromAmdm()
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

        public void SaveParseInformationToDb(List<string> authorNames, List<string> authorLinks, List<string> authorBiography)
        {
            int i = 0;
            //while (authorNames[i] != authorNames.Last() || authorLinks[i] != authorLinks.Last())
            while(i != 60)
            {
                Author author = new Author
                {
                    Name = authorNames[i],
                    LinkOfBiography = authorLinks[i],
                    Biography = authorBiography[i]
                };
                context.Authors.Add(author);
                i++;
                context.SaveChanges();
            }
            
            
            /*foreach ( var name in authorNames)
            {
                Author author = new Author
                {
                    Name = name
                    
                };
            context.Authors.Add(author);
            }

            context.SaveChanges();*/
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