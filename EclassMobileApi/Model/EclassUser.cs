using Flurl.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EclassMobileApi.Model
{
    public class EclassUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("uid")]
        public string Uid { get; set; }
        [JsonProperty("remember")]
        public bool IsRememberEnabled { get; set; }
        public EclassUser() {

        }
        public string GetUID(string UserToken, string link)
        {
            string page = "";
            Task.Run(async () => { page = await (link).PostUrlEncodedAsync(new { token = UserToken }).ReceiveString(); }).GetAwaiter().GetResult();
            List<string> hrefs = new List<string>();
            string uid = "";
            HtmlDocument portfolioDocumentPage = new HtmlDocument(); portfolioDocumentPage.LoadHtml(page);
            portfolioDocumentPage.DocumentNode.SelectNodes("//a[@href]").ToList().ForEach(node => hrefs.Add(node.Attributes["href"].Value));
            uid = hrefs.Where(href => href.Contains("uid")).ToList().First().Split("&amp;".ToCharArray()).Last().Split('=').Last();
            return uid;
        }
    }
}
