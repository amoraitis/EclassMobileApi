using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Flurl.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;

namespace EclassMobileApi.ViewModel
{
    public class ToolViewModel
    {
        private string _LoginToken;
        public List<Tool> Tools { get; set; }
        public ToolViewModel(string loginToken)
        {
            _LoginToken = loginToken;
            Tools = new List<Tool>();
        }
        public void SetTools(string xml)
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
                    default:
                        tool.Content = null;
                        break;
                }
            }
        }

        private async Task<object> GetDocs(string link)
        {
            string docs = await link.PostUrlEncodedAsync(new { token = _LoginToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(docs);

            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["class"] != null).Where(div => div.Attributes["class"].Value == "row");//.Where(div => div.InnerHtml.Contains("openDir"));
            res = res.ToList().GetRange(2, 2);
            return res;
        }

        private async Task<HtmlNode> GetDescription(string link)
        {
            string description = await link.PostUrlEncodedAsync(new { token = _LoginToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(description);
            var res = htmlDocument.DocumentNode.SelectNodes("//div").Where(div => div.InnerText != "" && div.Attributes["id"] != null)
                .Where(div => div.Attributes["id"].Value == "main-content").FirstOrDefault();
            return res;
        }
        private async Task<HtmlNode> GetCourseDescription(string link)
        {
            string courseDescription = await link.PostUrlEncodedAsync(new { token = _LoginToken }).ReceiveString();
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
        override public string ToString()
        {
            string toolsName = "";
            //Tools.ForEach(t => {if(t.Content.GetType()==typeof(HtmlNode))toolsName += t.Name + "\t" + t.Type + "\t" + t.Link + "\n"+((HtmlNode)t.Content).InnerText; });
            return toolsName;
        }
    }
}
namespace EclassMobileApi.ViewModel
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
            return Type.Equals("coursedescription") || Type.Equals("description") || Type.Equals("docs");
        }
    }
}
