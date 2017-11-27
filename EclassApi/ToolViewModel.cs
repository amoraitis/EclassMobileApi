using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Flurl.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EclassApi.ViewModel
{
    internal class ToolViewModel
    {
        private string _SessionToken, _CourseID, _Uid, _BaseUrl;
        public List<Tool> Tools { get; set; }
        internal ToolViewModel(string sessionToken, string courseID,string uid,string baseurl)
        {
            _SessionToken = sessionToken; _CourseID = courseID;
            _Uid = uid; _BaseUrl = baseurl;
            Tools = new List<Tool>();
        }
        private void SetTools(string xml)
        {
            XDocument toolsDocument = XDocument.Load(GenerateStreamFromString(xml));
            toolsDocument.Root.Elements("toolgroup").Elements("tool")
                .Select(t => new Tool
                {
                    Name = t.Attribute("name").Value,
                    Link = t.Attribute("redirect").Value,
                    Type = t.Attribute("type").Value,
                    Content = ""
                }).Where(t => t.IsNeeded()).ToList().ForEach(t => Tools.Add(t));
            AddContent();
        }

        private void AddContent()
        {
            for (int i = 0; i < 3; i++) {
                Tool tool = Tools.ElementAt(i);
                switch (tool.Type)
                {
                    case "coursedescription":
                        tool.Content = GetCourseDescription(tool.Link).GetAwaiter().GetResult();
                        break;
                    case "description":
                        tool.Content = GetDescription(tool.Link).GetAwaiter().GetResult();
                        break;
                    case "docs":
                        tool.Content = GetDocs(tool.Link).GetAwaiter().GetResult();
                        break;
                    case "announcements":
                        tool.Content = GetAnnouncements(tool.Link);
                        break;
                    default:
                        tool.Content = null;
                        break;
                }
            }
        }

        private List<Announcement> GetAnnouncements(string link)
        {
            AnnouncementViewModel announcements = new AnnouncementViewModel(_Uid, _SessionToken, _CourseID, _BaseUrl);
            return announcements.Announcements;
        }

        private async Task<object> GetDocs(string link)
        {
            string docs = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(docs);

            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["class"] != null).Where(div => div.Attributes["class"].Value == "row");//.Where(div => div.InnerHtml.Contains("openDir"));
            res = res.ToList().GetRange(2, 2);
            return res;
        }

        private async Task<HtmlNode> GetDescription(string link)
        {
            string description = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(description);
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["id"] != null)
                .Where(div => div.Attributes["id"].Value == "main-content").FirstOrDefault();
            return res;
        }
        private async Task<HtmlNode> GetCourseDescription(string link)
        {
            string courseDescription = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(courseDescription);
            var remove = htmlDocument.DocumentNode.SelectNodes("//ul").Where(ul => ul.Attributes["class"] != null).Where(ul => ul.Attributes["class"].Value == "course-title-actions clearfix pull-right list-inline").FirstOrDefault();
            remove.Remove();
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["class"] != null)
                .Where(div => div.Attributes["class"].Value == "panel panel-default").FirstOrDefault();
            return res;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
namespace EclassApi
{
    public class Tool
    {
        public string Name { get; set; }
        //For this property "redirect" property from xml is used
        public string Link { get; set; }
        public string Type { get; set; }
        public Object Content { get; set; }
        //Returns true if we can implement this tool
        public bool IsNeeded()
        {
            return Type.Equals("coursedescription") || Type.Equals("announcements") || Type.Equals("description") || Type.Equals("docs");
        }
    }
}
