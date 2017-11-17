using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using Flurl.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                    Type = t.Attribute("type").Value
                }).Where(t => t.IsNeeded()).ToList().ForEach(t => Tools.Add(t));
            AddContent();
        }

        private void AddContent()
        {
            Tools.ForEach(tool => {
                switch (tool.Type)
                {
                    case "coursedescription":
                        tool.Content = GetCourseDescription(tool.Link).Result.ToString();
                        break;
                    case "description":
                        break;
                    case "docs":
                        break;
                    default:
                        tool.Content = null;
                        break;
                }
            });
        }

        private async Task<string> GetCourseDescription(string link)
        {
            string courseDescription = await link.PostUrlEncodedAsync(new { token = _LoginToken }).ReceiveString();
            HtmlDocument htmlDocument = new HtmlDocument(); htmlDocument.LoadHtml(courseDescription);
            htmlDocument.DocumentNode.SelectNodes("//div").Where(c=> c.Attributes.Contains("class")).Where(y=>y.Attributes["class"].Value.Equals("panel panel-action-btn-default")).ToList().ForEach(sr=>courseDescription+=sr);

            return courseDescription;
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
            Tools.ForEach(t => { toolsName += t.Name + "\t" + t.Type + "\t" + t.Link + "\n"+t.Content; });
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
        public bool IsNeeded()
        {
            return Type.Equals("coursedescription") || Type.Equals("description") || Type.Equals("docs");
        }
    }
}
