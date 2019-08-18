using System.Collections.Generic;
using EclassApi.Models;

namespace EclassApi
{
    public class ToolViewModel
    {
        public readonly string sessionToken, courseId, uid, baseUrl;
        public List<ToolBase> Tools { get; set; }
        public ToolViewModel(string sessionToken, string courseID,string uid,string baseUrl)
        {
            this.sessionToken = sessionToken; courseId = courseID;
            this.uid = uid;
            this.baseUrl = baseUrl;
            Tools = new List<ToolBase>();
        }
    }
}