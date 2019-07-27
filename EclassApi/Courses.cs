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
    internal class Courses
    {
        private string baseurl, _Uid,
            _UserCourses= "/modules/mobile/mcourses.php";
        internal List<Course> UserCourses { get; private set; }
        internal Courses(string SessionToken, string baseurl, string uid)
        {
            this.baseurl = baseurl; this._Uid = uid;
            UserCourses = GetUserCourses(SessionToken);
        }

        internal List<Course> GetUserCourses(string SessionToken)
        {
            string coursesXml = (baseurl+ _UserCourses)
               .PostUrlEncodedAsync(new { token = SessionToken })
               .ReceiveString().GetAwaiter().GetResult();
            XDocument coursesXDocument = XDocument.Load(StringExtensions.GenerateStreamFromString(coursesXml));
            return coursesXDocument.Root
                 .Elements("coursegroup").Elements("course")
                 .Select(x => new Course
                 {
                     ID = ((string)x.Attribute("code").Value.Replace(@"\", string.Empty)),
                     Name = (string)x.Attribute("title"),
                     Tools = new ToolViewModel(SessionToken, 
                     ((string)x.Attribute("code").Value.Replace(@"\", string.Empty)),
                     _Uid,baseurl).Tools
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