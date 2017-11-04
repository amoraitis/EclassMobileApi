using System;
using Flurl.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Specialized;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace EclassMobileApi
{
    class Program
    {
        static string _auebEclass = "https://eclass.aueb.gr",
             _Login = "/modules/mobile/mlogin.php",
            _portfolio = "/modules/mobile/mportfolio.php",
            _UserCourses = "/modules/mobile/mcourses.php",
            _OpenCourses = "/modules/mobile/mcourses.php",
            _tools = "/modules/mobile/mtools.php?course=",
            _PortfolioPage = "/main/portfolio.php";
        static string LoginResult { get; set; }
        public static void Main(string[] args)
        {
            string login = "", portfolio = "", opencourses = "", userCourses = "", loginurl = _auebEclass + _Login, rss = "";
            string username = "p3140138"; //Console.ReadLine();
            string password = "noveplef97"; //Console.ReadLine();
            string tools = "", portfolioPage = "", UID="", AnnouncementToken="";
            Task.Run(async () =>
            {
                login = await (loginurl).PostUrlEncodedAsync(new { uname = username, pass = password }).ReceiveString();
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                portfolio = await (_auebEclass + _portfolio).PostUrlEncodedAsync(new { token = login }).ReceiveString();
            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                userCourses = await (_auebEclass + _UserCourses).PostUrlEncodedAsync(new { token = login }).ReceiveString();

            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                opencourses = await (_auebEclass + _OpenCourses).GetStringAsync();

            }).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                tools = await (_auebEclass + _tools + "INF261").PostUrlEncodedAsync(new { token = login }).ReceiveString();

            }).GetAwaiter().GetResult();
            Task.Run(async () => {portfolioPage= await (_auebEclass+_PortfolioPage).PostUrlEncodedAsync(new { token = login }).ReceiveString(); }).GetAwaiter().GetResult();
            AnnouncementToken = GetToken("INF261", login);
            Console.WriteLine("Login: "+loginurl + Environment.NewLine + LoginResult);
            Console.WriteLine("Portfolio: " + Environment.NewLine + portfolio);
            Console.WriteLine("User Courses:" + Environment.NewLine + userCourses);
            Console.WriteLine("Open courses:" + Environment.NewLine + opencourses);
            Console.WriteLine("Tools:" + Environment.NewLine + tools);
            UID = GetUID(portfolioPage);
            //Console.WriteLine(portfolioPage);
            Console.WriteLine("Portfolio:" + Environment.NewLine + UID);
            Console.WriteLine("Token for course with code INF261:" + AnnouncementToken);
            System.Console.ReadLine();
            
        }
        static string GetUID(string page)
        {
            List<string> hrefs = new List<string>();
            String uid="";
            HtmlDocument portfolioDocumentPage = new HtmlDocument(); portfolioDocumentPage.LoadHtml(page);
            portfolioDocumentPage.DocumentNode.SelectNodes("//a[@href]").ToList().ForEach(node => hrefs.Add(node.Attributes["href"].Value));
            uid = hrefs.Where(href => href.Contains("uid")).ToList().First().Split("&amp;".ToCharArray()).Last().Split('=').Last();
            return uid;
        }
        static string GetToken(string courseUID, string loginToken)
        {
            var CourseAnnouncementsHtml = "";
            Task.Run(async () => { CourseAnnouncementsHtml = await ("https://eclass.aueb.gr/modules/announcements/?course=" + courseUID).PostUrlEncodedAsync(new { token = loginToken }).ReceiveString(); }).GetAwaiter().GetResult();
            var doc = new HtmlDocument(); doc.LoadHtml(CourseAnnouncementsHtml);


            var value = doc.DocumentNode.Descendants("a").Where(x => x.Attributes.Contains("href"));
            var myval = value.Where(y => y.Attributes["href"].Value.Contains("/modules/announcements/rss.php"));
            return(myval.First().Attributes["href"].Value.Split('&').GetValue(2).ToString()).Split('=').Last().ToString();
        }

    }
}
