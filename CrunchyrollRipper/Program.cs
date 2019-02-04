using HtmlAgilityPack;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CrunchyrollRipper {
   class Program {

      ////*[@id="showview_content_videos"]/ul/li/ul

      static void Main(string[] args) {
         var url = "https://www.crunchyroll.com/de/black-clover";
         HtmlDocument doc = LoadHtmlDocument(url);

         Console.WriteLine(doc.DocumentNode.SelectSingleNode("//*[@id=\"container\"]/h1/span").InnerText);

         //TODO: Check for multiple seasons

         //Console.WriteLine(doc.DocumentNode.SelectNodes("//*[@id=\"showview_content_videos\"]/ul/li/ul/li")[0].ChildNodes[1].ChildNodes[1].XPath);

         foreach (var item in doc.DocumentNode.SelectNodes("//*[@id=\"showview_content_videos\"]/ul/li/ul/li")) {
            var aTag = item.ChildNodes[1].ChildNodes[1];
            var EpisodeThumbnail = aTag.ChildNodes[1].GetAttributeValue("src", "");
            var episodeTitle = aTag.ChildNodes[5].InnerText.Trim();
            var shortDesc = aTag.ChildNodes[7].InnerText.Trim();
            Console.WriteLine("https://www.crunchyroll.com" + aTag.GetAttributeValue("href", ""));
            LoadHtmlDocument("https://www.crunchyroll.com" + aTag.GetAttributeValue("href", "")); // Cache links
         }

         Console.ReadLine();
      }

      public static HtmlDocument LoadHtmlDocument(string url) {
         HtmlDocument doc = null;
         if (File.Exists($"Cache/{CalculateMD5Hash(url)}.html")) {
            doc = new HtmlDocument();
            doc.Load($"Cache/{CalculateMD5Hash(url)}.html");
            Console.WriteLine($"Loaded {url} from cache");
         }
         else {
            var web = new HtmlWeb();
            Console.WriteLine(web.UserAgent);
            doc = web.Load(url);
            web = null;
            //Console.WriteLine("Patching missing https://");
            //doc.Text = doc.Text.Replace("href=\"//", "href=\"https://");
            //Console.WriteLine("Patching missing domain");
            //doc.Text = doc.Text.Replace("href=\"/", "href=\"https://www.crunchyroll.com/");
            doc.Save(File.Open($"Cache/{CalculateMD5Hash(url)}.html", FileMode.OpenOrCreate));
            Console.WriteLine($"Cached {url}");
         }
         return doc;
      }

      public static string CalculateMD5Hash(string input) {
         MD5 md5 = MD5.Create();
         byte[] inputBytes = Encoding.ASCII.GetBytes(input);
         byte[] hash = md5.ComputeHash(inputBytes);
         StringBuilder sb = new StringBuilder();
         for (int i = 0; i < hash.Length; i++) {
            sb.Append(hash[i].ToString("x2"));
         }
         return sb.ToString();
      }


   }
}
