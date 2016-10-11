using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ConsoleApplicationForMvcProject
{
    class Program
    {
        List<string> authorBiographyLinks = new List<string>();

        string htmlUrl = "";
        HtmlDocument HD = new HtmlDocument();
        HtmlWeb web = new HtmlWeb
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.UTF8,
        };

        static void Main(string[] args)
        {
            Program obj = new Program();
            obj.ParseAuthorBiographySongLinksAndSongNames();
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


    }
