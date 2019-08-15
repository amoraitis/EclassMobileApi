using EclassApi.ViewModel;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
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
            Courses courses = new Courses(user.SessionToken, user.BaseUrl, user.Uid);
            user.UserCourses = courses.GetUserCourses();
            Assert.IsNotNull(user.UserCourses);
            Assert.IsNotEmpty(user.UserCourses.AsEnumerable());
        }

        [OneTimeTearDown]
        public async Task DestroySession()
        {
            await user.DestroySessionAsync();
        }
    }
}