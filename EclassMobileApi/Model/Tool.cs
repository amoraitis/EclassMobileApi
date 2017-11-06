using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace EclassMobileApi.Model
{
    public class ToolViewModel
    {
        public List<Tool> Tools { get; set; }
        public ToolViewModel()
        {
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
                }).Where(t=>t.IsNeeded()).ToList().ForEach(t=>Tools.Add(t));
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
            Tools.ForEach(t => { toolsName+=  t.Name + "\n"; });
            return toolsName;
        }
    }
    public class Tool
    {
        public string Name { get; set; }
        //For this property "redirect" property from xml is used
        public string Link { get; set; }
        public string Type { get; set; }
        public bool IsNeeded()
        {
            return Type.Equals("coursedescription") || Type.Equals("description") || Type.Equals("docs");
        }
    }
}
