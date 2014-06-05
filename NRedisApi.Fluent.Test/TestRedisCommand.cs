using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Fluent.Test
{
    [TestFixture]
    public class TestRedisCommand
    {
        private const string TestUrn = "urn:Test";
        private const string HashTestUrn = "urn:TestHash";
        private const string ManyHashTestUrn = "urn:TestManyHash";
        private const string ManyNotAllTHashTestUrn = "urn:TestManyHash";

        const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private SystemMonitorState _smsToSave;

        private IDatabase _redis;
        private ConnectionMultiplexer _multiplexer;

        [SetUp]
        public void SetupRedisConnectionMultiplexer()
        {
            _smsToSave = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);

            _multiplexer = ConnectionMultiplexer.Connect("localhost");

            _redis = _multiplexer.GetDatabase();
        }

        [TearDown]
        public void TeardownRedisConnectionMultiplexer()
        {

        }
        [Test]
        public void TestIRedisCommandConstructor()
        {
            IRedisCommand iRedisCommand = new RedisCommand(_redis);
            
            AssertUrnFieldIsEmpty(iRedisCommand);
        }

        [Test]
        public void TestIRedisCommandUrn()
        {
            IRedisCommand iRedisCommand = new RedisCommand(_redis);
            iRedisCommand = iRedisCommand.Urn(TestUrn);
            AssertUrnFieldEqualsTestUrn(iRedisCommand);

        }

        [Test]
        public void TestTypedRedisCommand()
        {
            IRedisCommand iRedisCommand = new RedisCommand(_redis);
            IRedisStringCommand<string> iRedisStringCommand = iRedisCommand.Urn(TestUrn).RedisString().As<string>();

            AssertTypedTimeSpanUntilExpirationFieldIsNull(iRedisStringCommand);
            AssertTypedStringUrnFieldEqualsTestUrn(iRedisStringCommand);

            Assert.IsInstanceOf<IRedisStringCommand<string>>(iRedisStringCommand);
        }

        [Test]
        public void TestIRedisStringCommandGetAndSet()
        {
            IRedisCommand redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(TestUrn)
                .RedisString()
                .As<SystemMonitorState>()
                .Set(_smsToSave);

            IRedisCommand redisGetOperation = new RedisCommand(_redis);
            var returnedSms = redisGetOperation
                .Urn(TestUrn)
                .RedisString()
                .As<SystemMonitorState>()
                .Get();
            
            

            Assert.IsInstanceOf<SystemMonitorState>(returnedSms);
        }

        [Test]
        public void TestIRedisHashCommandGetAndSet()
        {
            IRedisCommand redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(HashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .Set(_smsToSave);

            IRedisCommand redisGetOperation = new RedisCommand(_redis);
            var returnedSms = redisGetOperation
                .Urn(HashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldValues(new Dictionary<string, string>{ {"Location", _smsToSave.Location} })
                .Get();



            Assert.IsInstanceOf<SystemMonitorState>(returnedSms);
        }

        [Test]
        public void TestIRedisHashGetAll()
        {
            var sms = new SystemMonitorState(15, "N174E", DateTime.Now, SystemMonitorStatus.Critical);
            var sms2 = new SystemMonitorState(15, "N202W", DateTime.Now, SystemMonitorStatus.Normal);

            IRedisCommand redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(ManyHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(_smsToSave);

            redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(ManyHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms);

            redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(ManyHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms2);


            IRedisCommand redisGetOperation = new RedisCommand(_redis);
            var returnedSms = redisGetOperation
                .Urn(ManyHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .GetAll();



            Assert.IsInstanceOf<IEnumerable<SystemMonitorState>>(returnedSms);
            Assert.IsNotEmpty(returnedSms);
            Assert.IsTrue(returnedSms.Count() == 3);

        }

        [Test]
        public void TestIRedisHashGetAllExcludesNonTItems()
        {
            var nsms = new NotSystemMonitorState(15, "S123E", DateTime.Now, SystemMonitorStatus.Critical, "blah-de-blah");
            var sms2 = new SystemMonitorState(15, "N202W", DateTime.Now, SystemMonitorStatus.Normal);

            IRedisCommand redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(_smsToSave);

            new RedisCommand(_redis)
                .Urn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .As<NotSystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(nsms);

            redisSetOperation = new RedisCommand(_redis);
            redisSetOperation
                .Urn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms2);


            IRedisCommand redisGetOperation = new RedisCommand(_redis);
            var returnedSms = redisGetOperation
                .Urn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .As<SystemMonitorState>()
                .GetAll();



            Assert.IsInstanceOf<IEnumerable<SystemMonitorState>>(returnedSms);
            Assert.IsNotEmpty(returnedSms);
            Assert.IsTrue(returnedSms.Count() == 2);

        }

        [Test]
        public void TestIRedisHashCommandRaisesConfigExceptionOnIncorrectUidProperty()
        {
            IRedisCommand redisCommand = new RedisCommand(_redis);
            redisCommand
                .Urn(HashTestUrn);
                

            var typedHashCmd = redisCommand
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location");


            var ex = Assert.Throws<RedisCommandConfigurationException>(() => typedHashCmd.UniqueIdFieldName("NotAProperty"));
            Assert.IsInstanceOf<RedisCommandConfigurationException>(ex);
        }

        [Test]
        public void TestIRedisHashCommandCorrectlyIdentifiesDynamicObjectAsBeingSameAsT()
        {
            IRedisCommand redisCommand = new RedisCommand(_redis);
            redisCommand
                .Urn(HashTestUrn);


            var typedHashCmd = redisCommand
                .RedisHash()
                .As<SystemMonitorState>()
                .UniqueIdFieldName("Location");


            var ex = Assert.Throws<RedisCommandConfigurationException>(() => typedHashCmd.UniqueIdFieldName("NotAProperty"));
            Assert.IsInstanceOf<RedisCommandConfigurationException>(ex);
        }

        private void AssertUntypedTimeSpanUntilExpirationIsNull(IRedisCommand redisCommand)
        {
            var redisExpiresFieldInfo = redisCommand.GetType().GetField("_timeSpanUntilExpiration", BindFlags);
            if (redisExpiresFieldInfo != null)
                Assert.IsNull((long?)redisExpiresFieldInfo.GetValue(redisCommand));
            else
                Assert.Fail("_redisSecondsUntilExpiration field not found!");
        }

        private void AssertTypedTimeSpanUntilExpirationFieldIsNull<T>(IRedisStringCommand<T> redisCommand)
        {
            var redisExpiresFieldInfo = redisCommand.GetType().GetField("_timeSpanUntilExpiration", BindFlags);
            if (redisExpiresFieldInfo != null)
                Assert.IsNull((long?)redisExpiresFieldInfo.GetValue(redisCommand));
            else
                Assert.Fail("_redisSecondsUntilExpiration field not found!");
        }

        private void AssertUrnFieldIsEmpty(IRedisCommand redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsNullOrEmpty((string)urnFieldInfo.GetValue(redisCommand));
            else
                Assert.Fail("_urn field not found!");
        }

        private void AssertUrnFieldEqualsTestUrn(IRedisCommand redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((string)urnFieldInfo.GetValue(redisCommand)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

        private void AssertTypedStringUrnFieldEqualsTestUrn<T>(IRedisStringCommand<T> redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((T)urnFieldInfo.GetValue(redisCommand)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

    }
}
