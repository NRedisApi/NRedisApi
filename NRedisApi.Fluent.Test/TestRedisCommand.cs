using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
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

        private ConnectionMultiplexer _multiplexer;
        private RedisConnectionFactory _connectionFactory;


        [SetUp]
        public void SetupRedisConnectionMultiplexer()
        {
            _smsToSave = new SystemMonitorState(10, "N107W", DateTime.Now, SystemMonitorStatus.Normal);

            var config = new ConfigurationOptions
            {
                EndPoints = { { "localhost", 6379 } }
            };

            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

            _multiplexer = ConnectionMultiplexer.Connect(config);

            _connectionFactory = new RedisConnectionFactory(_multiplexer, config, jsonSettings);
        }

        [TearDown]
        public void TeardownRedisConnectionMultiplexer()
        {

        }
        [Test]
        public void TestIRedisCommandConstructor()
        {
            IRedisCommand iRedisCommand = _connectionFactory.GetConnection();
            
            AssertUrnFieldIsEmpty(iRedisCommand);
        }

        [Test]
        public void TestIRedisCommandUrn()
        {
            IRedisCommand iRedisCommand = _connectionFactory.GetConnection();
            iRedisCommand = iRedisCommand.SetUrn(TestUrn);
            AssertUrnFieldEqualsTestUrn(iRedisCommand);

        }

        [Test]
        public void TestTypedRedisCommand()
        {
            IRedisCommand iRedisCommand = _connectionFactory.GetConnection();
            IRedisStringCommand<string> iRedisStringCommand = iRedisCommand.SetUrn(TestUrn).RedisString().AsType<string>();

            AssertTypedTimeSpanUntilExpirationFieldIsNull(iRedisStringCommand);
            AssertTypedStringUrnFieldEqualsTestUrn(iRedisStringCommand);

            Assert.IsInstanceOf<IRedisStringCommand<string>>(iRedisStringCommand);
        }

        [Test]
        public void TestIRedisStringCommandGetAndSet()
        {
            IRedisCommand redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(TestUrn)
                .RedisString()
                .AsType<SystemMonitorState>()
                .Set(_smsToSave);

            IRedisCommand redisGetOperation = _connectionFactory.GetConnection();
            var returnedSms = redisGetOperation
                .SetUrn(TestUrn)
                .RedisString()
                .AsType<SystemMonitorState>()
                .Get();
            
            

            Assert.IsInstanceOf<SystemMonitorState>(returnedSms);
        }

        [Test]
        public void TestIRedisHashCommandGetAndSet()
        {
            IRedisCommand redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(HashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .Set(_smsToSave);

            IRedisCommand redisGetOperation = _connectionFactory.GetConnection();
            var returnedSms = redisGetOperation
                .SetUrn(HashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
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

            IRedisCommand redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(ManyHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(_smsToSave);

            redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(ManyHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms);

            redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(ManyHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms2);


            IRedisCommand redisGetOperation = _connectionFactory.GetConnection();
            var returnedSms = redisGetOperation
                .SetUrn(ManyHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
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

            IRedisCommand redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(_smsToSave);

            _connectionFactory.GetConnection()
                .SetUrn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .AsType<NotSystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(nsms);

            redisSetOperation = _connectionFactory.GetConnection();
            redisSetOperation
                .SetUrn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location")
                .UniqueIdFieldName("Alerts")
                .Set(sms2);


            IRedisCommand redisGetOperation = _connectionFactory.GetConnection();
            var returnedSms = redisGetOperation
                .SetUrn(ManyNotAllTHashTestUrn)
                .RedisHash()
                .AsType<SystemMonitorState>()
                .GetAll();



            Assert.IsInstanceOf<IEnumerable<SystemMonitorState>>(returnedSms);
            Assert.IsNotEmpty(returnedSms);
            Assert.IsTrue(returnedSms.Count() == 3);

        }

        [Test]
        public void TestIRedisHashCommandRaisesConfigExceptionOnIncorrectUidProperty()
        {
            IRedisCommand redisCommand = _connectionFactory.GetConnection();
            redisCommand
                .SetUrn(HashTestUrn);
                

            var typedHashCmd = redisCommand
                .RedisHash()
                .AsType<SystemMonitorState>()
                .UniqueIdFieldName("Location");


            var ex = Assert.Throws<RedisCommandConfigurationException>(() => typedHashCmd.UniqueIdFieldName("NotAProperty"));
            Assert.IsInstanceOf<RedisCommandConfigurationException>(ex);
        }

        [Test]
        public void TestIRedisHashCommandCorrectlyIdentifiesDynamicObjectAsBeingSameAsT()
        {
            IRedisCommand redisCommand = _connectionFactory.GetConnection();
            redisCommand
                .SetUrn(HashTestUrn);


            var typedHashCmd = redisCommand
                .RedisHash()
                .AsType<SystemMonitorState>()
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
            var urnFieldInfo = redisCommand.GetType().GetField("Urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsNullOrEmpty((string)urnFieldInfo.GetValue(redisCommand));
            else
                Assert.Fail("_urn field not found!");
        }

        private void AssertUrnFieldEqualsTestUrn(IRedisCommand redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("Urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((string)urnFieldInfo.GetValue(redisCommand)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

        private void AssertTypedStringUrnFieldEqualsTestUrn<T>(IRedisStringCommand<T> redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("Urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((T)urnFieldInfo.GetValue(redisCommand)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

    }
}
