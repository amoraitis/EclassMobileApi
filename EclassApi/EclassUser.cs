using EclassApi.ViewModel;
using Flurl.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EclassApi
{
    public class EclassUser
    {
        private const string _Login = "/modules/mobile/mlogin.php",
            _Logout = "/modules/mobile/mlogin.php?logout",
            _Portfolio = "/modules/mobile/mportfolio.php",
            _UserCourses = "/modules/mobile/mcourses.php",
            _PortfolioPage = "/main/portfolio.php";
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SessionToken { get; set; }
        public string Uid { get; set; }
        public List<Course> UserCourses { get; set; }
        public EclassUser(string uni) {
            
            BaseUrl = string.Concat("https://eclass.", uni, ".gr");
        }
        public async Task<bool> StartAsync(string username, string password)
        {
            Username = username;
            Password = password;
            SessionToken = await (BaseUrl + _Login).PostUrlEncodedAsync(new { uname = Username, pass = Password }).ReceiveString();
            if (SessionToken == ("FAILED")) return false;
            Uid = await GetUID(SessionToken, BaseUrl+_PortfolioPage);
            return true;
        }
        public void FillDetails()
        {
            Courses courses = new Courses(SessionToken, BaseUrl, Uid);
            UserCourses = courses.GetUserCourses();
        }
        public async Task DestroySessionAsync()
        {
            await (BaseUrl + _Logout).PostUrlEncodedAsync(new { token = SessionToken }).ReceiveString();
            SessionToken = null;
        }
        private async Task<string> GetUID(string SessionToken, string portfolioURL)
        {
            string page = page = await (portfolioURL).PostUrlEncodedAsync(new { token = SessionToken }).ReceiveString();
            List<string> hrefs = new List<string>();
            string uid = "";
            HtmlDocument portfolioDocumentPage = new HtmlDocument(); portfolioDocumentPage.LoadHtml(page);
            portfolioDocumentPage.DocumentNode.SelectNodes("//a[@href]").ToList().ForEach(node => hrefs.Add(node.Attributes["href"].Value));
            uid = hrefs.Where(href => href.Contains("uid")).ToList().First().Split("&amp;".ToCharArray()).Last().Split('=').Last();
            return uid;
        }
    }
}
