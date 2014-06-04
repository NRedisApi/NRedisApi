using System.Web.Mvc;
using NRedisApi.Samples.Mvc.Controllers;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Samples.Mvc.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            var controller = new HomeController(new RedisConnectionFactory(ConnectionMultiplexer.Connect("localhost")));

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void About()
        {
            // Arrange
            var controller = new HomeController(new RedisConnectionFactory(ConnectionMultiplexer.Connect("localhost")));

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [Test]
        public void Contact()
        {
            // Arrange
            var controller = new HomeController(new RedisConnectionFactory(ConnectionMultiplexer.Connect("localhost")));

            // Act
            var result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
