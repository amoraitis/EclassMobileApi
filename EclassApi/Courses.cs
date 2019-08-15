using EclassApi.Extensions;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EclassApi.ViewModel
{
    public class Courses
    {
        private readonly string baseurl, uid, sessionToken,
            _UserCourses = "/modules/mobile/mcourses.php";

        public Courses(string sessionToken, string baseurl, string uid)
        {
            if (string.IsNullOrEmpty(sessionToken) || string.IsNullOrEmpty(baseurl) || string.IsNullOrEmpty(uid))
                throw new ArgumentException();
            this.sessionToken = sessionToken;
            this.baseurl = baseurl;
            this.uid = uid;
        }

        public List<Course> GetUserCourses()
        {
            string coursesXml = (baseurl+ _UserCourses)
               .PostUrlEncodedAsync(new { token = sessionToken })
               .ReceiveString().GetAwaiter().GetResult();
            XDocument coursesXDocument = XDocument.Load(StringExtensions.GenerateStreamFromString(coursesXml));
            return coursesXDocument.Root
                 .Elements("coursegroup").Elements("course")
                 .Select(x => new Course
                 {
                     ID = ((string)x.Attribute("code").Value.Replace(@"\", string.Empty)),
                     Name = (string)x.Attribute("title"),
                     Tools = new ToolViewModel(sessionToken, 
                     ((string)x.Attribute("code").Value.Replace(@"\", string.Empty)),
                     uid,baseurl).Tools
                 }).ToList<Course>();
        }
    }

}
namespace EclassApi
{
    public class Course
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public List<ToolBase> Tools { get; set; }
    }
}