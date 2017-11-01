using System;
using Flurl.Http;
using System.Threading.Tasks;

namespace EclassMobileApi
{
    class Program
    {
        static string _auebEclass = "https://eclass.aueb.gr",
             _Login = "/modules/mobile/mlogin.php",
            _portfolio = "/modules/mobile/mportfolio.php",
            _UserCourses = "/modules/mobile/mcourses.php",
            _OpenCourses = "/modules/mobile/mcourses.php";
        static string loginResult { get; set; }
        public static void Main(string[] args)
        {
            string login ="", portfolio="", opencourses = "", userCourses = "", loginurl = _auebEclass +_Login;
            string username = "p3140138"; //Console.ReadLine();
            string password = "noveplef97"; //Console.ReadLine();
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
            Console.WriteLine("Login: "+loginurl + Environment.NewLine + loginResult);
            Console.WriteLine("Portfolio: " + Environment.NewLine + portfolio);
            Console.WriteLine("User Courses:" + Environment.NewLine + userCourses);
            Console.WriteLine("Open courses:" + Environment.NewLine + opencourses);
            System.Console.ReadLine();
        }
        

    }
}
