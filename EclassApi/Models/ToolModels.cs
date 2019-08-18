using System;
using System.Collections.Generic;

namespace EclassApi.Models
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


        internal static bool IsNeeded(string type) => Enum.IsDefined(typeof(ToolType), type);


    }

    public class DescriptionTool : ToolBase
    {
        public string Content { get; set; }
        public DescriptionTool(ToolBase tool) : base(tool.Name, tool.Link, tool.Type)
        {
            this.Content = null;
        }

    }

    public class CourseDescriptionTool : ToolBase
    {
        public string Content { get; set; }
        public CourseDescriptionTool(ToolBase tool) : base(tool.Name, tool.Link, tool.Type)
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