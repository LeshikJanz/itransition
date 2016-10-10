using AmdmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HtmlAgilityPack;
using System.Text;

namespace AmdmProject.Controllers
{
    public class HomeController : Controller
    {
        SongContext context = new SongContext();

        List<string> authorBiographyLinks = new List<string>();
        List<string> authorBiography = new List<string>();
        List<string> authorNames = new List<string>();

        List<string> songNames = new List<string>();
        List<string> songLinks = new List<string>();
        List<string> songLyrics = new List<string>();

        List<string> accordNames = new List<string>();
        List<string> accordPicturesLinks = new List<string>();

        List<Author> authors = new List<Author>();
        List<Song> songs = new List<Song>();
        List<Accord> accords = new List<Accord>();

        public void connectTables()
        {
            var Authors = context.Authors
                 .Include(s => s.Songs)
                 .ToList();
            var Songs = context.Songs
                .Include(x => x.Author)
                .Include(x => x.Accords)
                .ToList();
            var Accords = context.Accords
                .Include(x => x.Songs)
                .ToList();
        }
        public ActionResult Index()
        {
            connectTables();
            Console.WriteLine("Hello");
            IEnumerable<Author> Authors = context.Authors;
            IEnumerable<Song> Songs = context.Songs;
            ViewBag.Authors = Authors;
            int[] viewCounts = new int[Authors.Count()];
            int i = 0;
            foreach(var author in Authors) { 
                {
                    int count = 0;
                     foreach(var song in author.Songs)
                    {
                        count += song.NumberOfView;
                    }
                    viewCounts[i++] = count;
                }
            }
            ViewBag.ViewCounts = viewCounts;
            //ParseAuthorNamesAndBiographyLinks();
            //ParseAuthorBiographySongLinksAndSongNames();
            //GenerateAuthorListOfModels();
            
            //SaveAuthorModelsToDb();
            //GenerateAccordsListOfModelsAndParseSongLyrics();

            //SaveAccordModelsToDb();
            //GenerateSongsListOfModels();


            return View();
        }


        [HttpGet]
        public ActionResult Author(int id, string sortByNames, string sortByViews)
        {
            connectTables();
            if (sortByNames == null || sortByNames == "False")
            {
                ViewBag.SortedByNames = "True";
            }
            else ViewBag.SortedByNames = "False";

            if (sortByViews == null || sortByViews == "False")
            {
                ViewBag.SortedByViews = "True";
            }
            else ViewBag.SortedByViews = "False";
            Author author = context.Authors.Find(id);
            ViewBag.singleAuthor = author;
            //if (sortBy == "views")
            //{
                List<Song> songs = new List<Song>();
                List<Song> songsSorted = new List<Song>();
            foreach (var song in author.Songs)
                {
                    songs.Add(song);
                }
            if (sortByNames == "True")
            {
                var sortedSongs = songs.OrderByDescending(s => s.Name);
                songs = new List<Song>();
                foreach (Song s in sortedSongs)
                {
                    songs.Add(s);
                }
            }
            if (sortByViews == "True")
            {
                var sortedSongs = songs.OrderByDescending(s => s.NumberOfView);
                songs = new List<Song>();
                foreach (Song s in sortedSongs)
                {
                    songs.Add(s);
                }
            }

            ViewBag.Songs = songs;
            return View();
        }

        [HttpGet]
        public ActionResult Song(int id)
        {
            connectTables();
            Song song = context.Songs.Find(id);
            ViewBag.singleSong = song;
            ViewBag.Accords = song.Accords;
            ViewBag.AuthorId = song.AuthorId;
            ViewBag.AuthorName = song.Author.Name;
            return View();
        }

        public bool SaveChangesOfSong(string songId, string accords, string SongValue)
        {
            SongValue = SongValue.Replace('$', '<');
            accords = accords.Substring(0, accords.Length - 1);
            var massAccords = accords.Split(',');
            int id = Convert.ToInt32(songId);
            Song song = context.Songs.Find(id);
            List<Accord> Accords = new List<Accord>();

            song.Accords.Clear();
            //context.SaveChanges();
            foreach (var accordName in massAccords)
            {
                var accordN = "Аккорд " + accordName;
                Accord Accord = context.Accords.Where(a => a.AccordName == accordN).First();
                //Accords.Add(Accord);
               // song.Accords.Add(Accord);
            }
            context.SaveChanges();
            song.lyric = SongValue; // изменим название
            context.Entry(song).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }




        public void SaveAuthorModelsToDb()
        {
            foreach (var author in authors)
            {
                context.Authors.Add(author);
                context.SaveChanges();
            }
        }

        public void SaveAccordModelsToDb()
        {
            foreach (var accord in accords)
            {
                context.Accords.Add(accord);
                context.SaveChanges();
            }
        }

        public List<Author> GenerateAuthorListOfModels()
        {
            int i = 1;
            foreach (var authorName in authorNames)
            {
                Author author = new Author();
                author.AuthorId = i;
                author.Name = authorNames[i - 1];
                author.Biography = authorBiography[i - 1];
                author.LinkOfBiography = authorBiographyLinks[i - 1];
                authors.Add(author);
                i++;
            }
            return authors;
        }

        public List<string> ParseAuthorBiographySongLinksAndSongNames()
        {

            string htmlUrl = "";

            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            int t = 0;
            foreach (var authorLink in authorBiographyLinks)
            {
                t++;
                if (t == 10)
                {
                    System.Threading.Thread.Sleep(10000);
                    t = 0;
                }
                htmlUrl = authorLink;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//div[@class='artist-profile__bio']");
                HtmlNodeCollection NoAltElements2 = HD.DocumentNode.SelectNodes("//td/a[@class='g-link']");
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

                if (NoAltElements2 != null)
                {
                    foreach (HtmlNode HN in NoAltElements2)
                    {
                        string outputText = "http:" + HN.Attributes["href"].Value;
                        songLinks.Add(outputText); //добавление имени в список
                        outputText = HN.InnerHtml;
                        songNames.Add(outputText);
                    }
                }
            }
            return authorBiography;
        }

        public List<string> ParseAuthorNamesAndBiographyLinks()
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
                HtmlNodeCollection NoAltElements2 = HD.DocumentNode.SelectNodes("//td/a[@href]");
                if (NoAltElements != null)
                {
                    int j = 1; //счетчик количества песен на странице
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        if (HN.InnerHtml == "RinaOnish") break;
                        authorNames.Add(HN.InnerText); //добавление имени в список
                        j++;
                    }
                    int n = j;
                    j = 1;
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements2)
                    {
                        string outputText = "http:" + HN.Attributes["href"].Value;
                        if (flag) // считываем каждый второй
                        {
                            if (j == n) break;
                            authorBiographyLinks.Add(outputText); //добавление имени в список
                            flag = false;
                            j++;
                            continue;
                        }
                        flag = true;
                    }
                }
            }
            return authorNames;
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
            int j = 0;
            IEnumerable<Accord> accords = context.Accords;
            int t = 0;
            foreach (var authorLink in authorBiographyLinks)
            {
                t++;
                if (t == 10)
                {
                    System.Threading.Thread.Sleep(10000);
                    t = 0;
                }
                htmlUrl = authorLink;
                HD = web.Load(htmlUrl);
                Author author = context.Authors.First(a => a.LinkOfBiography == authorLink);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td/a[@class='g-link']");
                if (NoAltElements != null)
                {
                    
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        Song song = new Song();
                        song.Name = songNames[j];
                        song.LinkOfSong = songLinks[j];
                        song.lyric = songLyrics[j];
                        song.Author = author;
                        List<Accord> accordsOfOneAuthor = new List<Accord>();
                        foreach (var accord in accords)
                        {
                            if (accord.SongName == songNames[j])
                            {
                                accordsOfOneAuthor.Add(accord);
                            }
                        }
                        song.Accords = accordsOfOneAuthor;
                        j++;
                        context.Songs.Add(song);
                        songs.Add(song);
                    }
                }
                i++;
                context.SaveChanges();
            }
            
            return songs;
        }

        public List<Accord> GenerateAccordsListOfModelsAndParseSongLyrics()
        {
            string htmlUrl = "";

            HtmlDocument HD = new HtmlDocument();
            var web = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8,
            };

            int i = 1;
            int t = 0;
            foreach (var authorLink in songLinks)
            {
                t++;
                if (t == 10)
                {
                    System.Threading.Thread.Sleep(10000);
                    t = 0;
                }
                htmlUrl = authorLink;
                HD = web.Load(htmlUrl);
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//div[@id='song_chords']/img");
                HtmlNodeCollection NoAltElements2 = HD.DocumentNode.SelectNodes("//div/pre");
                if (NoAltElements != null)
                {
                    int j = 1;
                    foreach (HtmlNode HN in NoAltElements)
                    {
                        Accord accord = new Accord();
                        string outputText = HN.Attributes["alt"].Value;
                        accord.AccordName = outputText;
                        outputText = HN.Attributes["src"].Value;
                        accord.Img64 = "http:" + outputText;
                        accord.AccordId = j++;
                        int k = 0;
                        foreach (var songLink in songLinks)
                        {
                            if (songLink == authorLink)
                            {
                                accord.SongName = songNames[k];
                                k = 0;
                            }
                            k++;
                        }
                        accords.Add(accord);
                    }
                }
                i++;
                
                if (NoAltElements2 != null)
                {
                    bool flag = true; //выбираем каждый второй
                    foreach (HtmlNode HN in NoAltElements2)
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
            return accords;
        }
    }
}
