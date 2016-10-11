using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

using System.Diagnostics;
using System.Threading;
using System.Data.Entity;
using AmdmProject.Models;

namespace ConsoleApplicationForMvcProject
{
    class Program
    {
        static SongContext context = new SongContext();

        static List<string> authorBiographyLinks = new List<string>();
        static List<string> authorBiography = new List<string>();
        static List<string> authorNames = new List<string>();

        static List<string> songNames = new List<string>();
        static List<string> songLinks = new List<string>();
        static List<string> songLyrics = new List<string>();

        static List<string> accordNames = new List<string>();
        static List<string> accordPicturesLinks = new List<string>();

        static List<Author> authors = new List<Author>();
        static List<Song> songs = new List<Song>();
        static List<Accord> accords = new List<Accord>();

        static string htmlUrl = "";
        static HtmlDocument HD = new HtmlDocument();
        static HtmlWeb web = new HtmlWeb
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8,
        };

        public struct AuthorNameAndSongs
        {
            public string name;
            public List<string> songName;

            public AuthorNameAndSongs(string _name, List<string> _songName)
            {
                name = _name;
                songName = _songName;
            }
        };

        static List<AuthorNameAndSongs> authorNameAndSongs = new List<AuthorNameAndSongs>();

        static public void connectTables()
        {
            var Authors = context.Authors
                 .Include(s => s.Songs)
                 .ToList();
        }

            static void Main(string[] args)
        {
            IEnumerable<AmdmProject.Models.Author> AuthorsFromDb = context.Authors.ToList();
            
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ParseAuthorBiographySongLinksAndSongNames();
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Время работы программы: " + elapsedTime);
            Console.WriteLine();
            Console.WriteLine();
            connectTables();
            //IEnumerable<Author> AuthorsFromDb = context.Authors;
            
            foreach (var author in AuthorsFromDb)
            {
                int j = 0;
                AuthorNameAndSongs currentAuthorFromList = new AuthorNameAndSongs();
                int counterChangedSongs = 0;
                foreach (var authorFromList in authorNameAndSongs)
                {
                    if (authorFromList.name == author.Name)
                    {
                        currentAuthorFromList = authorFromList;
                        break;
                    }
                    j++;
                }
                if (currentAuthorFromList.name == null && currentAuthorFromList.name == null)
                {
                    Console.WriteLine("Добавлен новый автор: " + author.Name);
                }
                int k = 0; 
               
                foreach (var songName in author.Songs)
                {
                    string currentSongForTesting = "";
                    foreach (var songNameFromList in currentAuthorFromList.songName)
                    {
                        if (songNameFromList == songName.Name)
                        {
                            currentSongForTesting = songNameFromList;
                            break;
                        }
                        j++;
                        
                    }
                    if (currentSongForTesting == "")
                    {
                        counterChangedSongs++;
                    }
                    
                }
                Console.WriteLine("Группа " + author.Name + ": " + counterChangedSongs + " обновлены");
                //if (i == 10) break;
            }
            Console.ReadKey();
        }

        static public List<string> ParseAuthorBiographySongLinksAndSongNames()
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
            int t = 110; //
            int count = authorBiographyLinks.Count;
            for (int i = 0; i < count; i++)
            {                        t++;
                        if (t == 10) // каждые t песен задержка
                        {
                            System.Threading.Thread.Sleep(10000);
                            //t = 0;
                        }
                htmlUrl = authorBiographyLinks[i];
                HD = web.Load(htmlUrl);

                List<string> songsOfCurrentAuthor = new List<string>();
                HtmlNodeCollection NoAltElements = HD.DocumentNode.SelectNodes("//td/a[@class='g-link']");
                if (NoAltElements != null)
                {
                    
                    foreach (HtmlNode HN in NoAltElements)
                    {

                        string outputText = "http:" + HN.Attributes["href"].Value;
                        outputText = HN.InnerHtml;
                        songNames.Add(outputText);
                        songsOfCurrentAuthor.Add(outputText);
                    }
                }
                AuthorNameAndSongs authorNameAndSongsTemp = new AuthorNameAndSongs(authorNames[i], songsOfCurrentAuthor);
                authorNameAndSongs.Add(authorNameAndSongsTemp);
            }
            return authorBiographyLinks;
        }
    }
}

