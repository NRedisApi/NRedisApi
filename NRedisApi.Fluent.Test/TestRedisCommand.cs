using System;
using System.Reflection;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Fluent.Test
{
    [TestFixture]
    public class TestRedisCommand
    {
        private const string TestUrn = "urn:Test";
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

            AssertRedisDataStructureFieldIsUnknown(iRedisCommand);
        }

        [Test]
        public void TestIRedisCommandUrn()
        {
            IRedisCommand iRedisCommand = new RedisCommand(_redis);
            iRedisCommand = iRedisCommand.Urn(TestUrn);
            AssertUrnFieldEqualsTestUrn(iRedisCommand);

        }

        [Test]
        public void TestIRedisCommandAsType()
        {
            IRedisCommand iRedisCommand = new RedisCommand(_redis);
            IStringRedisCommand<string> iRedisStringCommand = iRedisCommand.Urn(TestUrn).RedisString().As<string>();

            AssertTypedRedisDataStructureFieldIsNotUnknown(iRedisStringCommand);
            AssertTypedUrnFieldEqualsTestUrn(iRedisStringCommand);

            Assert.IsInstanceOf<IRedisCommand<string>>(iRedisStringCommand);
        }

        [Test]
        public void TestIRedisCommandGetAndSet()
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

        private void AssertRedisDataStructureFieldIsUnknown(IRedisCommand redisCommand)
        {
            var redisDataStructureFieldInfo = redisCommand.GetType().GetField("_redisDataStructure", BindFlags);
            if (redisDataStructureFieldInfo != null)
                Assert.IsTrue((RedisDataStructure)redisDataStructureFieldInfo.GetValue(redisCommand) ==
                              RedisDataStructure.Unknown);
            else
                Assert.Fail("_redisDataStructureFieldInfo field not found!");
        }

        private void AssertRedisDataStructureFieldIsNotUnknown(IRedisCommand redisCommand)
        {
            var redisDataStructureFieldInfo = redisCommand.GetType().GetField("_redisDataStructure", BindFlags);
            if (redisDataStructureFieldInfo != null)
                Assert.IsTrue((RedisDataStructure)redisDataStructureFieldInfo.GetValue(redisCommand) !=
                              RedisDataStructure.Unknown);
            else
                Assert.Fail("_redisDataStructureFieldInfo field not found!");
        }

        private void AssertTypedRedisDataStructureFieldIsNotUnknown<T>(IRedisCommand<T> redisCommand)
        {
            var redisDataStructureFieldInfo = redisCommand.GetType().GetField("_redisDataStructure", BindFlags);
            if (redisDataStructureFieldInfo != null)
                Assert.IsTrue((RedisDataStructure)redisDataStructureFieldInfo.GetValue(redisCommand) !=
                              RedisDataStructure.Unknown);
            else
                Assert.Fail("_redisDataStructureFieldInfo field not found!");
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

        private void AssertTypedUrnFieldEqualsTestUrn<T>(IRedisCommand<T> redisCommand)
        {
            var urnFieldInfo = redisCommand.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((T)urnFieldInfo.GetValue(redisCommand)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

    }
}
