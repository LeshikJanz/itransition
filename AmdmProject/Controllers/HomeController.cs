using AmdmProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HtmlAgilityPack;
using System.Text;
using PagedList.Mvc;
using PagedList;
using Hangfire;
using System.Diagnostics;
using System.Activities;
using System.IO;

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

        string htmlUrl = "";
        HtmlDocument HD = new HtmlDocument();
        HtmlWeb web = new HtmlWeb
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8,
        };

        string pathToFileWithSortedData = @"C:\Users\a2.tereshko\Documents\Visual Studio 2015\Projects\AmdmProject\AmdmProject\SongNamesSorted.txt";

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

        struct NumberOfViewInEachAuthorId
            {
            int id;
            public int numberOfView;

            public int getId()
            {
                return id;
            }

            public NumberOfViewInEachAuthorId(int _id, int _numberOfView)
            {
                id = _id;
                numberOfView = _numberOfView;
            }
            };

        public ActionResult Index(string sortByNames, string sortByAccordSelection, string sortByNumberOfView, int? page, string fullPage)
        {
           // ParseAuthorsSongsAccords(); // Uncomment This function to start parsing site

            Response.Cache.SetExpires(DateTime.Now.AddSeconds(30));
            Response.Cache.SetCacheability(HttpCacheability.Server);
            connectTables();
            IEnumerable<Author> Authors = context.Authors;
            ViewBag.SortedByNames = "True";
            ViewBag.SortedByAccordSelection = "True";
            ViewBag.SortedByNumberOfView = "True";
            if (sortByNames == "True")
            {
                Authors = context.Authors.OrderBy(a => a.Name);
                sortByNames = "";
                ViewBag.SortedByNames = "False";
            }
            else if (sortByNames == "False")
            {
                Authors = context.Authors.OrderByDescending(a => a.Name);
                sortByNames = "";
                ViewBag.SortedByNames = "True";
            }


            if (sortByAccordSelection == "True")
            {
                Authors = context.Authors.OrderBy(a => a.Songs.Count);
                sortByAccordSelection = "";
                ViewBag.SortedByAccordSelection = "False";
            }
            else if (sortByAccordSelection == "False")
            {
                Authors = context.Authors.OrderByDescending(a => a.Songs.Count);
                sortByAccordSelection = "";
                ViewBag.SortedByAccordSelection = "True";
            }

            IEnumerable<Song> Songs = context.Songs;
            ViewBag.Authors = Authors;
            int[] viewCounts = new int[Authors.Count()];
            int i = 0;

            List<NumberOfViewInEachAuthorId> numberOfViewInEachAuthorId = new List<NumberOfViewInEachAuthorId>();
            foreach (var author in Authors) { 
                {
                    int count = 0;
                     foreach(var song in author.Songs)
                    {
                        count += song.NumberOfView;
                    }
                    viewCounts[i++] = count;
                    NumberOfViewInEachAuthorId temp = new NumberOfViewInEachAuthorId(author.AuthorId, count);
                    numberOfViewInEachAuthorId.Add(temp);
                }
            }
            ViewBag.ViewCounts = viewCounts;

            List<Author> authorList = new List<Author>();
            if (sortByNumberOfView == "True")
            {
                i = 0;
                numberOfViewInEachAuthorId = numberOfViewInEachAuthorId.OrderByDescending(n => n.numberOfView).ToList();
                foreach (var numberOfViewInEachAuthor in numberOfViewInEachAuthorId)
                {
                    authorList.Add(context.Authors.Find(numberOfViewInEachAuthor.getId()));
                    viewCounts[i++] = numberOfViewInEachAuthor.numberOfView;
                }
                sortByNumberOfView = "";
                ViewBag.SortedByNumberOfView = "False";
                Authors = authorList;
                ViewBag.Authors = Authors;
                ViewBag.ViewCounts = viewCounts;
            }
            else if (sortByNumberOfView == "False")
            {
                i = 0;
                numberOfViewInEachAuthorId = numberOfViewInEachAuthorId.OrderBy(n => n.numberOfView).ToList();
                foreach (var numberOfViewInEachAuthor in numberOfViewInEachAuthorId)
                {
                    authorList.Add(context.Authors.Find(numberOfViewInEachAuthor.getId()));
                    viewCounts[i++] = numberOfViewInEachAuthor.numberOfView;
                }
                sortByNumberOfView = "";
                ViewBag.SortedByNumberOfView = "True";
                Authors = authorList;
                ViewBag.Authors = Authors;
                ViewBag.ViewCounts = viewCounts;
            }

            ViewBag.UsingPagedDirectives = "";
            ViewBag.PagesButtons = "";
            if (page == null) page = 1;
            ViewBag.pageNumber = page;

            if (fullPage == "True" || fullPage == null)
            {
                ViewBag.FullPage = "False";
                return View(Authors);
            }
            else
            {
                ViewBag.FullPage = "True";
                int pageSize = 3;
                int pageNumber = (page ?? 1);
                //return View(Authors);
                IEnumerable<Author> myAuthors = Authors;
                return View(myAuthors.OrderBy(k => k.AuthorId).ToPagedList(page ?? 1, 50));
            }

            
            //return View(Authors.ToPagedList(pageNumber, pageSize));
        }

        public void ParseAuthorsSongsAccords()
        {
            ParseAuthorBiographySongLinksAndSongNames();

            htmlUrl = "";

            int count = authorBiographyLinks.Count;
            for (int i = 0; i < count; i++)
            {

                string author_Biography = "";
                htmlUrl = authorBiographyLinks[i];
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
                            author_Biography = outputText;
                            flag = false;
                            continue;
                        }
                        flag = true;
                    }
                }
                Author author = new Author();
                // author.AuthorId = i;
                author.Name = authorNames[i];
                author.Biography = author_Biography;
                author.LinkOfBiography = authorBiographyLinks[i];
                authors.Add(author);

                if (NoAltElements2 != null)
                {
                    int t = 0;
                    foreach (HtmlNode HN in NoAltElements2)
                    {
                        t++;
                        if (t == 10) // каждые t песен задержка
                        {
                            System.Threading.Thread.Sleep(20000);
                            t = 0;
                        }
                        string outputText = "http:" + HN.Attributes["href"].Value;
                        songLinks.Add(outputText); //добавление имени в список
                        string song_LinkOfSong = outputText;
                        outputText = HN.InnerHtml;
                        songNames.Add(outputText);
                        string song_Name = outputText;
                        string song_lyric = "";
                        //----------------------------
                        htmlUrl = "";
                        // HtmlDocument HDP = new HtmlDocument();
                        web = new HtmlWeb
                        {
                            AutoDetectEncoding = false,
                            OverrideEncoding = Encoding.UTF8,
                        };
                        HD = web.Load(song_LinkOfSong);
                        List<Accord> accordsOfOneAuthor = new List<Accord>();
                        HtmlNodeCollection NoAltElements3 = HD.DocumentNode.SelectNodes("//div[@id='song_chords']/img");
                        HtmlNodeCollection NoAltElements4 = HD.DocumentNode.SelectNodes("//div/pre");
                        if (NoAltElements3 != null)
                        {

                            foreach (HtmlNode HN3 in NoAltElements3)
                            {
                                Accord accordSearch = null;
                                string accord_Name = HN3.Attributes["alt"].Value;
                                if (accords.Count > 0) accordSearch = accords.Find(a => a.AccordName == accord_Name);//Where(a => a.AccordName == accord_Name).First();
                                if (accordSearch == null)
                                {
                                    Accord accord = new Accord();
                                    accord.AccordName = accord_Name;
                                    accord.Img64 = "http:" + HN3.Attributes["src"].Value;
                                    //   accord.AccordId = j++;
                                    accords.Add(accord);
                                    accordSearch = accord;
                                }
                                accordsOfOneAuthor.Add(accordSearch);
                            }
                        }

                        if (NoAltElements4 != null)
                        {
                            bool flag = true; //выбираем каждый второй
                            foreach (HtmlNode HN4 in NoAltElements4)
                            {
                                outputText = HN4.InnerHtml;
                                if (flag) // считываем каждый второй
                                {
                                    songLyrics.Add(outputText); //добавление имени в список
                                    song_lyric = outputText;
                                    flag = false;
                                    continue;
                                }
                                flag = true;
                            }
                        }
                        Song song = new Song();
                        song.Name = song_Name;
                        song.LinkOfSong = song_LinkOfSong;
                        song.lyric = song_lyric;
                        song.Author = author;
                        song.Accords = accordsOfOneAuthor;
                        songs.Add(song);
                        //----------------------------
                    }
                }
            }
            //----------------------------------------------------
            SaveAuthorModelsToDb();
            SaveAccordModelsToDb();
            SaveSongModelsToDb();
        }

        [HttpGet]
        public ActionResult Author(int id, string sortByNames, string sortByViews)
        {
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(30));
            Response.Cache.SetCacheability(HttpCacheability.Server);
            connectTables();
            Author author = context.Authors.Find(id);
            ViewBag.singleAuthor = author;
            IEnumerable<Song> songs = author.Songs;

            ViewBag.SortedByNames = "True";
            ViewBag.SortedByViews = "True";
            if (sortByNames == "True")
            {
                songs = songs.OrderBy(a => a.Name);
                sortByNames = "";
                ViewBag.SortedByNames = "False";
            }
            else if (sortByNames == "False")
            {
                songs = songs.OrderByDescending(a => a.Name);
                sortByNames = "";
                ViewBag.SortedByNames = "True";
            }

            if (sortByViews == "True")
            {
                songs = songs.OrderBy(a => a.NumberOfView);
                sortByNames = "";
                ViewBag.SortedByViews = "False";
            }
            else if (sortByViews == "False")
            {
                songs = songs.OrderByDescending(a => a.NumberOfView);
                sortByNames = "";
                ViewBag.SortedByViews = "True";
            }

            
         

            using (StreamWriter sw = new StreamWriter(pathToFileWithSortedData, false, System.Text.Encoding.Default))
            {
                foreach (var song in songs)
                {
                    sw.WriteLine(song.Name);
                }
                if (sw != null)
                    sw.Close();
            }
            ViewBag.Songs = songs;
            return View();
        }


        [HttpGet]
        public ActionResult Song(int id, string flag, string songName)
        {
            connectTables();
            string[] massNames = new string[1000];
            int i = 0;
            int sizeOfMass = 0;
            Song song;
            StreamReader sr = new StreamReader(pathToFileWithSortedData, System.Text.Encoding.Default);
                while (true)
                {
                    string temp = sr.ReadLine();
                    if (temp == null)
                {
                    sr.Close();
                    sizeOfMass = i;
                    break;
                }
                    massNames[i++] = temp;
                }

            for( i = 0; i < 1000; i++)
            {
                if (massNames[i] == songName) break;
            }
            IEnumerable<Song> songList = context.Songs;
            if (i == 0 || i == sizeOfMass)
            {
                song = context.Songs.Find(id);
            }
            else if (flag == "0")
            {
                if (songName == massNames[i - 1]) //Если имя у песни одинаковое, но подбор аккордов другой
                {
                    song = context.Songs.Find(id);
                }else
                {
                    string prevSong = massNames[i - 1];
                    song = songList.Where(s => s.Name == prevSong).First();
                }
                
            }
            else if (flag == "1")
            {
                if (songName == massNames[i + 1]) 
                {
                    song = context.Songs.Find(id);
                }
                else
                {
                    string prevSong = massNames[i + 1];
                    song = songList.Where(s => s.Name == prevSong).First();
                }
            }
            else
            {
                song = context.Songs.Find(id);
            }
            song.NumberOfView = song.NumberOfView + 1;
            context.Entry(song).State = EntityState.Modified;
            context.SaveChanges();

            ViewBag.singleSong = song;
            ViewBag.Accords = song.Accords;
            ViewBag.AuthorId = song.AuthorId;
            ViewBag.AuthorName = song.Author.Name;
            return View();
        }

        public bool SaveChangesOfSong(string songId, string accords, string SongValue, string authorName)
        {
            SongValue = SongValue.Replace('$', '<');
            accords = accords.Substring(0, accords.Length - 1);
            var massAccords = accords.Split(',');
            int id = Convert.ToInt32(songId);

            Song song = context.Songs.Find(id);
            List<Accord> Accords = new List<Accord>();
            Author author = context.Authors.Where(a => a.Name == authorName).First();
            song.lyric = SongValue; // изменим название
            song.Author = author;
            var existingSong = context.Songs.Include("Accords").Where(s => s.SongId == id).First();
            foreach (var accordName in massAccords)
            {
                var accordN = "Аккорд " + accordName;
                Accord Accord = context.Accords.Where(a => a.AccordName == accordN).First();
                Accords.Add(Accord);
            }
            var deletedAccords = song.Accords.ToList<Accord>();
            var addedAccords = Accords.ToList<Accord>();
            deletedAccords.ForEach(c => existingSong.Accords.Remove(c));
            foreach (Accord c in addedAccords)
            {
                if (context.Entry(c).State == EntityState.Detached)
                    context.Accords.Attach(c);
                existingSong.Accords.Add(c);
            }

            //Сохранем в базу данных изменения
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

        public void SaveSongModelsToDb()
        {
            foreach (var song in songs)
            {
                context.Songs.Add(song);
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
            return authorBiographyLinks;
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
