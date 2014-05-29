using System;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Test
{
    [TestFixture]
    public class TestRedisConnection
    {
        private IRedisConnectionFactory _factory;

        [SetUp]
        public void SetupRedisConnectionMultiplexer()
        {
            _factory = new RedisConnectionFactory(ConnectionMultiplexer.Connect("192.168.1.152"));
        }

        [TearDown]
        public void TeardownRedisConnectionMultiplexer()
        {

        }

        [Test]
        public void TestSet()
        {
            var smsUrn = @"urn:smState";
            var redis = _factory.GetConnection();
            var smsToSave = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);
            redis.Save(smsUrn, smsToSave);
            var deserialisedSms = redis.GetValue<SystemMonitorState>(smsUrn);
            Assert.IsInstanceOf<SystemMonitorState>(deserialisedSms);
            Assert.IsTrue(smsToSave.Location.Equals(deserialisedSms.Location));
        }

        [Test]
        public void TestHashSet()
        {
            var hashUrn = @"urn:TestHash";
            var redis = _factory.GetConnection();
            var smsToSave = new SystemMonitorState(10, "N007E", DateTime.Now, SystemMonitorStatus.Critical);
            redis.SaveToHash(hashUrn, smsToSave, new[] { "Location", "Alerts" });
            var deserialisedSms = redis.FindFromHash<SystemMonitorState>(hashUrn, new[] {"Location", "Alerts"},
                new[] {new Tuple<string, string>("Location", "N007E"), new Tuple<string, string>("Alerts", "10")});
            Assert.IsTrue(smsToSave.Location.Equals(deserialisedSms.Location));

        }

        [Test]
        public void SmsConstructorBehaviour()
        {
            var sms = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);
            Assert.AreEqual(sms.Alerts, 10);
        }

    }
}
