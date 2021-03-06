﻿using System.Web.Mvc;
using Newtonsoft.Json;
using NRedisApi.Samples.Mvc.Controllers;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Samples.Mvc.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        private ConnectionMultiplexer _multiplexer;
        private RedisConnectionFactory _connectionFactory;

        [Test]
        public void Index()
        {
            // Arrange
            var config = new ConfigurationOptions
            {
                EndPoints = { { "localhost", 6379 } }
            };

            _multiplexer = ConnectionMultiplexer.Connect(config);

            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

            _connectionFactory = new RedisConnectionFactory(_multiplexer, config, jsonSettings);
            var controller = new HomeController(_connectionFactory);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void About()
        {
            // Arrange
            var controller = new HomeController(_connectionFactory);

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [Test]
        public void Contact()
        {
            // Arrange
            var controller = new HomeController(_connectionFactory);

            // Act
            var result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
