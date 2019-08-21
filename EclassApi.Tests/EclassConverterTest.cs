using System.IO;
using System.Linq;
using System.Text;
using EclassApi.Converters;
using EclassApi.Models;
using Microsoft.DotNet.PlatformAbstractions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EclassApi.Tests
{
    /// <summary>
    /// The text fixture for <see cref="EclassConverterTest" />.
    /// </summary>
    [TestFixture]
    public class EclassConverterTest
    {
        private readonly string _json;
        public EclassConverterTest()
        {
            _json = File.ReadAllText(
                Path.Combine(ApplicationEnvironment.ApplicationBasePath, ".content", "eclassuser.json"), 
                Encoding.UTF8);
        }

        /// <summary>
        /// Test for <see cref="EclassConverterTest" />.
        /// </summary>
        [Test]
        public void Test()
        {
            EclassUser eclassUser = null;
            Assert.DoesNotThrow(()=>eclassUser = JsonConvert.DeserializeObject<EclassUser>(_json, new ToolItemConverter()));
            Assert.That((eclassUser.UserCourses.First().ToolViewModel.Tools.First(tool=>tool.Type == ToolType.announcements) as AnnouncementsTool)?.Content, Is.Not.Null);
            var list = eclassUser.UserCourses.SelectMany(course => course.ToolViewModel.Tools.OfType<AnnouncementsTool>()).Select(tool=>tool.Content).ToList();
            Assert.That(list, Has.Some.Not.Empty);
            list.ForEach(content => Assert.That(content, Is.Not.Null));
        }
    }
}

