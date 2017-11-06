using Flurl.Http;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EclassMobileApi.Model
{
    public class AnnouncementToken
    {
        public String ID { get; set; }
        public String Token { get; set; }
        public AnnouncementToken(string courseID, string loginToken)
        {
            ID = courseID;
            Token = GetToken(courseID, loginToken);
        }
        string GetToken(string courseUID, string loginToken)
        {
            var CourseAnnouncementsHtml = "";
            Task.Run(async () => { CourseAnnouncementsHtml = await ("https://eclass.aueb.gr/modules/announcements/?course=" + courseUID).PostUrlEncodedAsync(new { token = loginToken }).ReceiveString(); }).GetAwaiter().GetResult();
            var doc = new HtmlDocument(); doc.LoadHtml(CourseAnnouncementsHtml);


            var value = doc.DocumentNode.Descendants("a").Where(x => x.Attributes.Contains("href"));
            var myval = value.Where(y => y.Attributes["href"].Value.Contains("/modules/announcements/rss.php"));
            return (myval.First().Attributes["href"].Value.Split('&').GetValue(2).ToString()).Split('=').Last().ToString();
        }
    }
}