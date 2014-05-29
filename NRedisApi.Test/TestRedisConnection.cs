using System;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Test
{
    /// <summary>
    /// These unit tests require a Database Sandbox (http://xunitpatterns.com/Database%20Sandbox.html) to run as otherwise there is no point in testing that the Set/Save methods using a Mock
    /// as all that will show is that those methods can accept their parameters.
    /// In this case, a Database Sandbox requires that you have a locally available instance of Redis running, preferably purely for the purposes of running these tests.
    /// </summary>
    [TestFixture]
    public class TestRedisConnection
    {
        private const string SmsUrn = @"urn:smState";
        private const string HashUrn = @"urn:TestHash";
        private SystemMonitorState _smsToSave;
        private IRedisConnectionFactory _factory;

        [SetUp]
        public void SetupRedisConnectionMultiplexer()
        {
            _smsToSave = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);

            _factory = new RedisConnectionFactory(ConnectionMultiplexer.Connect("localhost"));
        }

        [TearDown]
        public void TeardownRedisConnectionMultiplexer()
        {

        }

        [Test]
        public void TestSet()
        {
            var redis = _factory.GetConnection();
            
            redis.Save(SmsUrn, _smsToSave);
            var deserialisedSms = redis.GetValue<SystemMonitorState>(SmsUrn);
            Assert.IsInstanceOf<SystemMonitorState>(deserialisedSms);
            Assert.IsTrue(_smsToSave.Location.Equals(deserialisedSms.Location));
        }

        [Test]
        public void TestHashSet()
        {
            var redis = _factory.GetConnection();
          
            redis.SaveToHash(HashUrn, _smsToSave, new[] { "Location", "Alerts" });
            var deserialisedSms = redis.FindFromHash<SystemMonitorState>(HashUrn, new[] {"Location", "Alerts"},
                new[] { new Tuple<string, string>("Location", "N107W"), new Tuple<string, string>("Alerts", "10") });
            Assert.IsTrue(_smsToSave.Location.Equals(deserialisedSms.Location));
        }

        [Test]
        public void SmsConstructorBehaviour()
        {
            var sms = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);
            Assert.AreEqual(sms.Alerts, 10);
        }

    }
}
