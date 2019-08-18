using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EclassApi.Extensions;
using EclassApi.Models;
using Flurl.Http;

namespace EclassApi
{
    public class Courses
    {
        private readonly string baseurl, uid, sessionToken;

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
            string coursesXml = (baseurl + Constants.UserCourses)
               .PostUrlEncodedAsync(new { token = sessionToken })
               .ReceiveString().GetAwaiter().GetResult();
            var coursesXDocument = XDocument.Load(StringExtensions.GenerateStreamFromString(coursesXml));
            return coursesXDocument.Root?.Elements("coursegroup").Elements("course")
                 .Select(x => new Course
                 {
                     ID = ((string)x.Attribute("code")?.Value.Replace(@"\", string.Empty)),
                     Name = (string)x.Attribute("title"),
                     ToolViewModel = new ToolViewModel(sessionToken,
                     ((string)x.Attribute("code")?.Value.Replace(@"\", string.Empty)),
                     uid, baseurl)
                 }).ToList();
        }
    }
}