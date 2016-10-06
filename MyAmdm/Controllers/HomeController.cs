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
        SongContext db = new SongContext();

        private SqlConnection connect = null;

        public void OpenConnection(string connectionString) //Открытие соединения с БД
        {
            connect = new SqlConnection(connectionString);
            connect.Open();
        }

        public void CloseConnection()   //Закрытие соединения с БД
        {
            connect.Close();
        }

        public void InsertDataInDB()
        {

        }

        public ActionResult Index()
        {
            //IEnumerable<Author> authors = db.Authors;
            //ViewBag.Authors = authors;

            /////////////////////////////Работа с БД, переделать на кнопку
            //saveParseInformationToDB(ParseInformationFromAmdm());
            ////////////////////////////
            return View();
        }

        public ActionResult ShowAuthorsInformationPartial()  //Обработчик кнопки "Подгрузить данные". Вычитываем информацию из базы данных
        {
            IEnumerable<Author> authors = db.Authors; //
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

            string sqlRequest = GenerateAuthorInformationSqlRequest(ParseInformationFromAmdm());
            ExecuteSQL(sqlRequest);
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
            string sql = string.Format("DELETE FROM AUTHORS");
            ExecuteSQL(sql);
            ShowAuthorsInformationPartial();
        }

        public string GenerateAuthorInformationSqlRequest(List<string> authorNames)
        {
            string sql = "";
            foreach (var name in authorNames)
            {
                sql += string.Format("Insert Into Authors" +
                " Values(N'" + name + "', null);");
            }
            return sql;
        }

        public void ExecuteSQL(string sql)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["SongContext"].ToString();
            OpenConnection(connectionString);
            
            using (SqlCommand cmd = new SqlCommand(sql, this.connect))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("SQL error", ex);
                    throw error;
                }
            }

            CloseConnection();
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