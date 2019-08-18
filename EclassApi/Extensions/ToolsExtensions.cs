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
using EclassApi.Extensions;
using System.Xml.Linq;
using EclassApi.Models;

namespace EclassApi.Extensions
{
    public static class ToolsExtensions
    {

        public static async Task AddAnnouncementsAsync(this EclassUser eclassUser)
        {
            await eclassUser.UserCourses.ForEachAsync(async course =>
                    await course.AddAnouncementsAsync(eclassUser.Uid, eclassUser.SessionToken, eclassUser.BaseUrl));
        }

        public static async Task AddAnouncementsAsync(this Course course, string uid, string sessionToken, string baseurl)
        {
            var announcementsURL = (baseurl + "/modules/announcements/?course=" + course.ID);
            var rssAnnouncementsURL = (baseurl + "/modules/announcements/rss.php?c=" + course.ID);
            var announcements = new List<Announcement>();
            var rss = await GetRSSAsync(rssAnnouncementsURL, announcementsURL, uid, sessionToken);
            //Correct RSS, in case there is no date
            if (rss.Contains("<lastBuildDate></lastBuildDate>"))
                rss = rss.Replace("<lastBuildDate></lastBuildDate>", string.Empty);

            RssDocument rssDocument = RssDocument.Load(rss);

            foreach (var item in rssDocument.Channel.Items)
            {
                Announcement announcement = new Announcement
                {
                    Title = item.Title.HTMLtoSTRING(),
                    Description = item.Description.HTMLtoSTRING(),
                    DatePublished = item.PubDate.Value.ToString("MM/dd/yyyy HH:mm"),
                    Link = item.Link.Url
                };
                announcements.Add(announcement);
            }
            course.ToolViewModel.Tools.OfType<AnnouncementsTool>().Single().Content = announcements;
        }

        public static async Task<IList<Course>> AddToolsAsync(this IList<Course> courses)
        {
            await courses.ForEachAsync(async course => await course.ToolViewModel.AddToolsAsync());
            return courses;
        }

        public static async Task AddToolsAsync(this ToolViewModel toolViewModel)
        {
            string xml = (toolViewModel.baseUrl + Constants.ToolsEndpoint + toolViewModel.courseId).PostUrlEncodedAsync(new { token = toolViewModel.sessionToken }).ReceiveString().GetAwaiter().GetResult();
            XDocument toolsDocument = XDocument.Load(Extensions.StringExtensions.GenerateStreamFromString(xml));
            toolViewModel.Tools = toolsDocument.Root.Elements("toolgroup").Elements("tool").Where(t => ToolBase.IsNeeded(t.Attribute("type").Value))
                .Select(t =>
                {
                    ToolType type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value);
                    switch (type)
                    {
                        case ToolType.coursedescription:
                            return new CourseDescriptionTool(new ToolBase
                            {
                                Name = t.Attribute("name").Value,
                                Link = t.Attribute("redirect").Value,
                                Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                            });
                        case ToolType.announcements:
                            return new AnnouncementsTool(new ToolBase
                            {
                                Name = t.Attribute("name").Value,
                                Link = t.Attribute("redirect").Value,
                                Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                            });
                        case ToolType.description:
                            return new DescriptionTool(new ToolBase
                            {
                                Name = t.Attribute("name").Value,
                                Link = t.Attribute("redirect").Value,
                                Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                            });
                        case ToolType.docs:
                            return new DocsTool(new ToolBase
                            {
                                Name = t.Attribute("name").Value,
                                Link = t.Attribute("redirect").Value,
                                Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                            });
                        default:
                            return new ToolBase
                            {
                                Name = t.Attribute("name").Value,
                                Link = t.Attribute("redirect").Value,
                                Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                            };
                    }
                }

            ).ToList();
            await toolViewModel.Tools.AddContentAsync(toolViewModel.sessionToken);
        }

        #region Helpers
        private static async Task AddContentAsync(this IList<ToolBase> tools, string sessionToken)
        {
            await tools.ForEachAsync(null,
               async tool =>
               {
                   switch (tool.Type)
                   {
                       case ToolType.coursedescription:
                           ((CourseDescriptionTool)tool).Content = await GetCourseDescription(((CourseDescriptionTool)tool).Link, sessionToken);
                           break;
                       case ToolType.description:
                           ((DescriptionTool)tool).Content = await GetDescription(((DescriptionTool)tool).Link, sessionToken);
                           break;
                       case ToolType.docs:
                           var docsTuple = await GetDocs(tool.Link, sessionToken);
                           ((DocsTool)tool).RootDirectory = docsTuple.Item1;
                           ((DocsTool)tool).RootDirectoryDownloadLink = docsTuple.Item2;
                           break;
                       default:
                           break;
                   }
               });
        }

        private static async Task<Tuple<string, string>> GetDocs(string link, string sessionToken)
        {
            string docs = await link.PostUrlEncodedAsync(new { token = sessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(docs);
            var rootDirectoryLink = htmlDocument.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/a[1]").Single().Attributes["href"].Value;

            var rootDirectoryDownloadLink = htmlDocument.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/a[2]").Single().Attributes["href"].Value;

            return new Tuple<string, string>(rootDirectoryLink, rootDirectoryDownloadLink);
        }

        private static async Task<string> GetDescription(string link, string sessionToken)
        {
            string description = await link.PostUrlEncodedAsync(new { token = sessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(description);
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["id"] != null)
                .Where(div => div.Attributes["id"].Value == "main-content").FirstOrDefault();
            var resToStr = res.InnerText;
            return StringExtensions.IfNullConvertToEmpty(resToStr);
        }
        private static async Task<string> GetCourseDescription(string link, string sessionToken)
        {
            string courseDescription = await link.PostUrlEncodedAsync(new { token = sessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(courseDescription);
            var remove = htmlDocument.DocumentNode.SelectNodes("//ul").Where(ul => ul.Attributes["class"] != null).Where(ul => ul.Attributes["class"].Value == "course-title-actions clearfix pull-right list-inline").FirstOrDefault();
            remove.Remove();
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["class"] != null)
                .Where(div => div.Attributes["class"].Value == "panel panel-default").FirstOrDefault();


            var resToStr = res.ParentNode.InnerText;
            var resToStrUnescaped = StringExtensions.RemoveEscapeChars(resToStr);
            return StringExtensions.IfNullConvertToEmpty(resToStrUnescaped);
        }

        private static async Task<String> GetRSSAsync(string rssAnnouncementsURL, string announcementsURL, string uid, string sessionToken)
        {
            try
            {
                return await rssAnnouncementsURL.GetAsync().ReceiveString();
            }
            catch (FlurlHttpException)
            {
                string announcementToken = await GetTokenAsync(announcementsURL, sessionToken);
                return await (rssAnnouncementsURL + "&uid=" + uid + "&token=" + announcementToken).GetAsync().ReceiveString();
            }
        }
        private static async Task<string> GetTokenAsync(string announcementsURL, string sessionToken)
        {
            var CourseAnnouncementsHtml = await (announcementsURL).PostUrlEncodedAsync(new { token = sessionToken }).ReceiveString();
            var doc = new HtmlDocument(); doc.LoadHtml(CourseAnnouncementsHtml);
            var value = doc.DocumentNode.Descendants("a").Where(x => x.Attributes.Contains("href"));
            var myval = value.Where(y => y.Attributes["href"].Value.Contains("/modules/announcements/rss.php"));
            string announcementToken = (myval.First().Attributes["href"].Value.Split('&').GetValue(2).ToString()).Split('=').Last().ToString();
            return announcementToken;
        }
        #endregion
    }
}