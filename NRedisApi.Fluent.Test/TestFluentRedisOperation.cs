using System.Reflection;
using NUnit.Framework;

namespace NRedisApi.Fluent.Test
{
    [TestFixture]
    public class TestFluentRedisOperation
    {
        private const string TestUrn = "urn:Test";
        const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [Test]
        public void TestRedisOperationConstructor()
        {
            var redisOperation = new RedisOperation();
            
            AssertUrnFieldIsEmpty(redisOperation);

            AssertRedisDataStructureFieldIsUnknown(redisOperation);
        }

        [Test]
        public void TestRedisOperationUrn()
        {
            var redisOperation = new RedisOperation();
            redisOperation = redisOperation.Urn(TestUrn);
            AssertUrnFieldEqualsTestUrn(redisOperation);

        }

        [Test]
        public void TestRedisOperationAsType()
        {
            var redisOperation = new RedisOperation();
            redisOperation = redisOperation.Urn(TestUrn).String().AsType<string>();

            AssertRedisDataStructureFieldIsNotUnknown(redisOperation);
            AssertUrnFieldEqualsTestUrn(redisOperation);

            var type = redisOperation.GetType();
            Assert.IsInstanceOf<RedisOperation<string>>(redisOperation);
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
