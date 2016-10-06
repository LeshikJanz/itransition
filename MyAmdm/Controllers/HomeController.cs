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
        List<string> authorBiographyLinks = new List<string>();
        List<string> authorBiography = new List<string>();
        List<string> authorNames = new List<string>();

        List<Author> authors = new List<Author>();
        List<Song> songs = new List<Song>();

        List<string> songLinks = new List<string>();
        List<string> songLyrics = new List<string>();
        string[,] songNames = new string[100,3000]; //[id-автора, список песен - также номер, это id-песни]


        public ActionResult Index()
        {
            //ParseAuthorBiographyFromAmdm(ParseAuthorBiographyLinksFromAmdm());

            ParseAuthorBiographyLinksFromAmdm();
            ParseAuthorNamesFromAmdm();
            ParseAuthorBiographyFromAmdm();
            ParseSongLinks();
            ParseSongLyrics();
            

            GenerateSongsListOfModels();
            GenerateAuthorListOfModels();
            return View();
        }

        public ActionResult ShowAuthorsInformationPartial()  //Обработчик кнопки "Подгрузить данные". Вычитываем информацию из базы данных
        {
            ParseAndSaveHandler();
            IEnumerable<Author> authors = context.Authors; //
            ViewBag.Authors = authors; //Сделать передачу через модель
            
            return PartialView();
        }

        public List<Author> GenerateAuthorListOfModels()
        {
            int i = 1;
            foreach (var authorName in authorNames)
            {
                Author author = new Author();
                author.AuthorId = i;
                author.Name = authorNames[i];
                author.Biography = authorBiography[i];
                author.LinkOfBiography = authorBiographyLinks[i];
                i++;
            }
            return authors;
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
            //SaveParseInformationToDb(ParseAuthorNamesFromAmdm(), ParseAuthorBiographyLinksFromAmdm(), ParseAuthorBiographyFromAmdm(ParseAuthorBiographyLinksFromAmdm())); //тут еще страшно, но это будет работать только один раз, чтобы пропарсить все
        }

        public List<string> ParseSongLyrics()
        {

            string htmlUrl = "";

            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            foreach (var authorLink in songLinks)
            {
                htmlUrl = "http:" + authorLink;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//div/pre");
                if (NoAltElements != null)
                {
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        string outputText = HN.InnerHtml;
                        if (flag) // считываем каждый второй
                        {
                            songLyrics.Add(outputText); //добавление имени в список
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
            }
            return songLyrics;
        }

        public List<string> ParseAuthorBiographyFromAmdm()
        {
            
            string htmlUrl = "";

            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            foreach (var authorLink in authorBiographyLinks)
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

        public List<string> ParseSongLinks()
        {

            string htmlUrl = "";
            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };


                foreach (var link in authorBiographyLinks)
                {
                    htmlUrl = "http:" + link;
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
                            songLinks.Add(outputText); //добавление имени в список
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
            }
            return songLinks;
        }

        public List<Song> GenerateSongsListOfModels()  //считывает все песни
        {
            string htmlUrl = "";
            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };
            int i = 1; 
            foreach (var authorLink in authorBiographyLinks)
            {
                htmlUrl = "http:" + authorLink;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td/a");
                if (NoAltElements != null)
                {
                    int j = 1;
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        string outputText = HN.InnerText;
                        Song song = new Song();
                        song.AuthorId = i;
                        song.SongId = j;
                        song.Name = outputText;
                        song.LinkOfSong = "http:" + songLinks[j];
                        song.lyric = songLyrics[j++];
                        songs.Add(song);
                    }
                }
                i++;
            }
            return songs;
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
            while(i != 350)
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