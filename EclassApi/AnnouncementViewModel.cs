using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using X.Web.RSS;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace EclassApi.ViewModel
{
    internal class AnnouncementViewModel
    {
        private string _Uid, _SessionToken, _CourseID, AnnouncementsURL, RSSAnnouncementsURL;
        internal List<Announcement> Announcements { get; set; }

        public AnnouncementViewModel(string uid, string sessionToken,string courseID, string baseurl)
        {
            Announcements = new List<Announcement>();
            _Uid = uid;
            _SessionToken = sessionToken;
            _CourseID = courseID;
            AnnouncementsURL = (baseurl +"/modules/announcements/?course="+_CourseID);
            RSSAnnouncementsURL = (baseurl + "/modules/announcements/rss.php?c=" + _CourseID);
            SetAnouncementsAsync();

        }
        

        internal async void SetAnouncementsAsync()
        {
            var rss = await GetRSS(RSSAnnouncementsURL);
            //Correct RSS
            if(rss.Contains("<lastBuildDate></lastBuildDate>"))
                rss = rss.Replace("<lastBuildDate></lastBuildDate>", "");

            RssDocument rssDocument = RssDocument.Load(rss);
            
            foreach (X.Web.RSS.Structure.RssItem item in rssDocument.Channel.Items)
            {
                Announcement an = new Announcement
                {
                    Title = Extensions.StringExtensions.HTMLtoSTRING(item.Title),
                    Description = Extensions.StringExtensions.HTMLtoSTRING(item.Description),
                    DatePublished = item.PubDate.Value.ToString("MM/dd/yyyy HH:mm"),
                    Link = item.Link.Url
                };
                this.AddAnnouncement(an);
            }
        }
        private void AddAnnouncement(Announcement announcement)
        {
            this.Announcements.Add(announcement);
        }
        private async Task<String> GetRSS(string struri)
        {
            try
            {
                return await struri.GetAsync().ReceiveString();
            }
            catch(FlurlHttpException)
            {
                string announcementToken = GetToken();
                return await (RSSAnnouncementsURL + "&uid=" + _Uid + "&token=" + announcementToken).GetAsync().ReceiveString();
            }
        }
        private string GetToken()
        {
            var CourseAnnouncementsHtml = "";
            Task.Run(async () => { CourseAnnouncementsHtml = await (AnnouncementsURL).PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString(); }).GetAwaiter().GetResult();
            var doc = new HtmlDocument(); doc.LoadHtml(CourseAnnouncementsHtml);
            var value = doc.DocumentNode.Descendants("a").Where(x => x.Attributes.Contains("href"));
            var myval = value.Where(y => y.Attributes["href"].Value.Contains("/modules/announcements/rss.php"));
            string announcementToken = (myval.First().Attributes["href"].Value.Split('&').GetValue(2).ToString()).Split('=').Last().ToString();
            return announcementToken;
        }
    }
}
namespace EclassApi {
    public class Announcement
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public string Description { get; set; }
        public string DatePublished { get; set; }
        public Announcement() { }
        public Announcement(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
