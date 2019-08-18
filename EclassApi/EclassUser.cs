using EclassApi.Models;
using Flurl.Http;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EclassApi
{
    public class EclassUser
    {
        
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { private get; set; }
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
            SessionToken = await (BaseUrl + Constants.Login).PostUrlEncodedAsync(new { uname = Username, pass = Password }).ReceiveString();
            if (SessionToken == ("FAILED")) return false;
            Uid = await GetUid(SessionToken, BaseUrl+Constants.PortfolioPage);
            return true;
        }
        public void AddCourses()
        {
            var courses = new Courses(SessionToken, BaseUrl, Uid);
            UserCourses = courses.GetUserCourses();
        }
        public async Task DestroySessionAsync()
        {
            await (BaseUrl + Constants.Logout).PostUrlEncodedAsync(new { token = SessionToken }).ReceiveString();
            SessionToken = null;
        }
        private async Task<string> GetUid(string sessionToken, string portfolioUrl)
        {
            string page = page = await (portfolioUrl).PostUrlEncodedAsync(new { token = sessionToken }).ReceiveString();
            var hrefs = new List<string>();
            var uid = "";
            var portfolioDocumentPage = new HtmlDocument(); portfolioDocumentPage.LoadHtml(page);
            portfolioDocumentPage.DocumentNode.SelectNodes("//a[@href]").ToList().ForEach(node => hrefs.Add(node.Attributes["href"].Value));
            uid = hrefs.Where(href => href.Contains("uid")).ToList().First().Split("&amp;".ToCharArray()).Last().Split('=').Last();
            return uid;
        }
    }
}
