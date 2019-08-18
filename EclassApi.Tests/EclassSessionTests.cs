using EclassApi.Extensions;
using EclassApi.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EclassApi.Tests
{
    [TestFixture]
    public class EclassSessionTests
    {
        private IConfiguration Configuration { get; set; }
        private EclassUser user;

        [OneTimeSetUp]
        public void SetUp()
        {
            var builder = new ConfigurationBuilder()
            .AddUserSecrets<EclassSessionTests>();

            Configuration = builder.Build();
            user = new EclassUser("aueb");
        }

        [Test, Order(1)]
        public async Task ShouldCreateSessionForUser()
        {
            var username = Configuration["EclassUser"];
            var password = Configuration["EclassUserPassword"];
            var sessionCreated = await user.StartAsync(username, password);
            Assert.True(sessionCreated);
            Assert.NotNull(user.BaseUrl);
            Assert.NotNull(user.Uid);
        }

        [Test, Order(2)]
        public void ShouldHaveCoursesWhenCoursesAdded()
        {
            var courses = new Courses(user.SessionToken, user.BaseUrl, user.Uid);
            user.UserCourses = courses.GetUserCourses();
            Assert.IsNotNull(user.UserCourses);
            Assert.IsNotEmpty(user.UserCourses.AsEnumerable());
        }

        [Test, Order(3)]
        public void ExpectNoToolsWhenToolsNotAdded()
        {
            var everyCourseTools = user.UserCourses.Select(course => course.ToolViewModel.Tools);
            Assert.That(everyCourseTools, Is.All.Empty);
        }

        [Test, Order(4)]
        public async Task ShouldHaveToolsApartFromAnnouncements()
        {
            await user.UserCourses.AddToolsAsync();
            var descriptionOfAllCourses = user.UserCourses.Select(c => c.ToolViewModel.Tools.OfType<DescriptionTool>().Single().Content);
            var courseDescriptionOfAllCourses = user.UserCourses.Select(c => c.ToolViewModel.Tools.OfType<CourseDescriptionTool>().Single().Content);
            var docsOfAllCourses = user.UserCourses.SelectMany(c => c.ToolViewModel.Tools.OfType<DocsTool>().Single().RootDirectoryDownloadLink);
            docsOfAllCourses = docsOfAllCourses.Intersect(user.UserCourses.SelectMany(c => c.ToolViewModel.Tools.OfType<DocsTool>().Single().RootDirectory));
            Assert.Multiple(() =>
            {
                CollectionAssert.AllItemsAreNotNull(descriptionOfAllCourses);
                CollectionAssert.AllItemsAreNotNull(courseDescriptionOfAllCourses);
                CollectionAssert.AllItemsAreNotNull(docsOfAllCourses);
            });

        }

        [Test, Order(5)]
        public async Task ShouldHaveAnnouncementsWhenAnnouncementsAddedAsync()
        {
            await AddAnnouncementsToCoursesAsync();

            var announcementsOfAllCourses = user.UserCourses.Select(c => c.ToolViewModel.Tools.OfType<AnnouncementsTool>().Single().Content);

            Assert.That 
                (announcementsOfAllCourses, Is.All.Not.Null);
        }

        private async Task AddAnnouncementsToCoursesAsync()
        {
            await user.AddAnnouncementsAsync();
        }

        [OneTimeTearDown]
        public async Task DestroySession()
        {
            await user.DestroySessionAsync();
        }
    }
}