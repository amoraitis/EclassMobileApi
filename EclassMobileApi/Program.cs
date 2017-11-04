using System;
using Flurl.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace EclassMobileApi
{
    class Program
    {
        static string _auebEclass = "https://eclass.aueb.gr",
             _Login = "/modules/mobile/mlogin.php",
            _portfolio = "/modules/mobile/mportfolio.php",
            _UserCourses = "/modules/mobile/mcourses.php",
            _OpenCourses = "/modules/mobile/mcourses.php",
            _tools = "/modules/mobile/mtools.php?course=";
        static string loginResult { get; set; }
        public static void Main(string[] args)
        {
            string login = "", portfolio = "", opencourses = "", userCourses = "", loginurl = _auebEclass + _Login, rss = "";
            string username = "p3140138"; //Console.ReadLine();
            string password = "noveplef97"; //Console.ReadLine();
            string tools = "", ansd = "";
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
            Task.Run(async () => {ansd= await "https://eclass.aueb.gr/modules/announcements/?course=INF261".PostUrlEncodedAsync(new { token = login }).ReceiveString(); }).GetAwaiter().GetResult();

            Console.WriteLine("Login: "+loginurl + Environment.NewLine + loginResult);
            Console.WriteLine("Portfolio: " + Environment.NewLine + portfolio);
            Console.WriteLine("User Courses:" + Environment.NewLine + userCourses);
            Console.WriteLine("Open courses:" + Environment.NewLine + opencourses);
            Console.WriteLine("Tools:" + Environment.NewLine + tools);
            Console.WriteLine("aAnnadsafd:" + Environment.NewLine + ansd);
            Console.WriteLine(rss);
            System.Console.ReadLine();
            
        }
        static async Task<string> GetFeed()
        {
            String rss;
            try
            {
                rss = await "https://eclass.aueb.gr/modules/announcements/rss.php?c=INF261".GetAsync().ReceiveString();
                return rss;
            }
            catch(FlurlHttpException flurlExc)
            {
                return flurlExc.Call.HttpStatus.ToString();
            }
        }

    }
}
