using System;
using System.Reflection;
using NUnit.Framework;
using StackExchange.Redis;

namespace NRedisApi.Fluent.Test
{
    [TestFixture]
    public class TestFluentRedisOperation
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
        public void TestRedisOperationConstructor()
        {
            var redisOperation = new RedisOperation(_redis);
            
            AssertUrnFieldIsEmpty(redisOperation);

            AssertRedisDataStructureFieldIsUnknown(redisOperation);
        }

        [Test]
        public void TestRedisOperationUrn()
        {
            var redisOperation = new RedisOperation(_redis);
            redisOperation = redisOperation.Urn(TestUrn);
            AssertUrnFieldEqualsTestUrn(redisOperation);

        }

        [Test]
        public void TestRedisOperationAsType()
        {
            var redisOperation = new RedisOperation(_redis);
            redisOperation = redisOperation.Urn(TestUrn).String().AsType<string>();

            AssertRedisDataStructureFieldIsNotUnknown(redisOperation);
            AssertUrnFieldEqualsTestUrn(redisOperation);

            Assert.IsInstanceOf<RedisOperation<string>>(redisOperation);
        }

        [Test]
        public void TestRedisOperationGet()
        {
            var redisOperation = new RedisOperation(_redis);
            var typedRedisOperation = 
                redisOperation
                .Urn(TestUrn)
                .String()
                .AsType<string>();
            
            var returned = typedRedisOperation.Get();

            Assert.IsInstanceOf<RedisOperation<string>>(typedRedisOperation);
        }

        private void AssertRedisDataStructureFieldIsUnknown(RedisOperation redisOperation)
        {
            var redisDataStructureFieldInfo = redisOperation.GetType().GetField("_redisDataStructure", BindFlags);
            if (redisDataStructureFieldInfo != null)
                Assert.IsTrue((RedisDataStructure)redisDataStructureFieldInfo.GetValue(redisOperation) ==
                              RedisDataStructure.Unknown);
            else
                Assert.Fail("_redisDataStructureFieldInfo field not found!");
        }

        private void AssertRedisDataStructureFieldIsNotUnknown(RedisOperation redisOperation)
        {
            var redisDataStructureFieldInfo = redisOperation.GetType().GetField("_redisDataStructure", BindFlags);
            if (redisDataStructureFieldInfo != null)
                Assert.IsTrue((RedisDataStructure)redisDataStructureFieldInfo.GetValue(redisOperation) !=
                              RedisDataStructure.Unknown);
            else
                Assert.Fail("_redisDataStructureFieldInfo field not found!");
        }

        private void AssertUrnFieldIsEmpty(RedisOperation redisOperation)
        {
            var urnFieldInfo = redisOperation.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsNullOrEmpty((string)urnFieldInfo.GetValue(redisOperation));
            else
                Assert.Fail("_urn field not found!");
        }

        private void AssertUrnFieldEqualsTestUrn(RedisOperation redisOperation)
        {
            var urnFieldInfo = redisOperation.GetType().GetField("_urn", BindFlags);
            if (urnFieldInfo != null)
                Assert.IsTrue(((string)urnFieldInfo.GetValue(redisOperation)).Equals(TestUrn));
            else
                Assert.Fail("_urn field not found!");
        }

    }
}
