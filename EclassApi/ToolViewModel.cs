using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Flurl.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Async;
using EclassApi.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EclassApi.ViewModel
{
    internal class ToolViewModel
    {
        private string _SessionToken, _CourseID, _Uid, _BaseUrl, 
            _ToolsEndpoint= "/modules/mobile/mtools.php?course=";
        internal List<ToolBase> Tools { get; set; }
        internal ToolViewModel(string sessionToken, string courseID,string uid,string baseurl)
        {
            _SessionToken = sessionToken; _CourseID = courseID;
            _Uid = uid; _BaseUrl = baseurl;
            Tools = new List<ToolBase>();
            SetTools();
        }
        private void SetTools()
        {
            string xml = (_BaseUrl + _ToolsEndpoint + _CourseID).PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString().GetAwaiter().GetResult();
            XDocument toolsDocument = XDocument.Load(Extensions.StringExtensions.GenerateStreamFromString(xml));
            toolsDocument.Root.Elements("toolgroup").Elements("tool").Where(t=> ToolBase.IsNeeded(t.Attribute("type").Value))
                .Select(t => new ToolBase
                {
                    Name = t.Attribute("name").Value,
                    Link = t.Attribute("redirect").Value,
                    Type = (ToolType)Enum.Parse(typeof(ToolType), t.Attribute("type").Value)
                }).ToList().ForEach(t => Tools.Add(t));
            AddContentAsync();
        }

        private async void AddContentAsync()
        {
            await Tools.ParallelForEachAsync(
               async tool =>
               {
                   switch (tool.Type)
                   {
                       case ToolType.coursedescription:
                           tool = new DescriptionTool(tool);
                           (tool as DescriptionTool).Content = await GetCourseDescription(tool.Link);
                           break;
                       case ToolType.description:
                           tool = new DescriptionTool(tool);
                           (tool as DescriptionTool).Content = await GetDescription(tool.Link);
                           break;
                       case ToolType.docs:
                           tool = new DocsTool(tool);
                           var docsTuple = await GetDocs(tool.Link);
                           (tool as DocsTool).RootDirectory = docsTuple.Item1;
                           (tool as DocsTool).RootDirectoryDownloadLink = docsTuple.Item2;
                           break;
                       case ToolType.announcements:
                           tool = new AnnouncementsTool(tool);
                           (tool as AnnouncementsTool).Content = GetAnnouncements(tool.Link);
                           break;
                       default:
                           break;
                   }
               },
               maxDegreeOfParalellism: Environment.ProcessorCount);
        }

        private List<Announcement> GetAnnouncements(string link)
        {
            AnnouncementViewModel announcements = new AnnouncementViewModel(_Uid, _SessionToken, _CourseID, _BaseUrl);
            return announcements.Announcements;
        }

        private async Task<Tuple<string,string>> GetDocs(string link)
        {
            string docs = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(docs);
            var rootDirectoryLink = htmlDocument.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/a[1]").Single().Attributes["href"].Value;

            var rootDirectoryDownloadLink = htmlDocument.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/a[2]").Single().Attributes["href"].Value;

            return new Tuple<string, string>(rootDirectoryLink, rootDirectoryDownloadLink);
        }

        private async Task<string> GetDescription(string link)
        {
            string description = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(description);
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["id"] != null)
                .Where(div => div.Attributes["id"].Value == "main-content").FirstOrDefault();
            var resToStr = res.InnerText;
            return resToStr;
        }
        private async Task<string> GetCourseDescription(string link)
        {
            string courseDescription = await link.PostUrlEncodedAsync(new { token = _SessionToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(courseDescription);
            var remove = htmlDocument.DocumentNode.SelectNodes("//ul").Where(ul => ul.Attributes["class"] != null).Where(ul => ul.Attributes["class"].Value == "course-title-actions clearfix pull-right list-inline").FirstOrDefault();
            remove.Remove();
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["class"] != null)
                .Where(div => div.Attributes["class"].Value == "panel panel-default").FirstOrDefault();

            
            var resToStr = res.ParentNode.InnerText;
            var resToStrUnescaped = StringExtensions.RemoveEscapeChars(resToStr);
            return resToStrUnescaped;
        }
    }
}
namespace EclassApi
{
    public class ToolBase
    {
        public ToolBase() { }
        public ToolBase(string name, string link, ToolType type)
        {
            Name = name;
            Link = link;
            Type = type;
        }

        public string Name { get; set; }
        //For this property "redirect" property from xml is used
        public string Link { get; set; }
        public ToolType Type { get; set; }
        //Returns true if we can implement this tool
        internal bool IsNeeded()
        {
            return Type.Equals("coursedescription") || Type.Equals("announcements") || Type.Equals("description") || Type.Equals("docs");
        }
        internal static bool IsNeeded(string type)
        {
            return type.Equals("coursedescription") || type.Equals("announcements") || type.Equals("description") || type.Equals("docs");
        }
    }

    public class DescriptionTool : ToolBase
    {
        public string Content { get; set; }
        public DescriptionTool(ToolBase tool) : base(tool.Name, tool.Link, tool.Type)
        {
            this.Content = null;
        }
    }

    public class AnnouncementsTool : ToolBase
    {
        public List<Announcement> Content { get; set; }
        public AnnouncementsTool(ToolBase tool) : base(tool.Name, tool.Link, tool.Type)
        {
            this.Content = null;
        }
    }

    public class DocsTool : ToolBase
    {
        public string RootDirectory { get; set; }
        public string RootDirectoryDownloadLink { get; set; }
        public DocsTool(ToolBase tool) : base(tool.Name, tool.Link, tool.Type)
        {
           
        }
    }

    public enum ToolType
    {
        coursedescription, announcements, description, docs
    }
}