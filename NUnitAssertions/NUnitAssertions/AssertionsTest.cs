using NUnit.Framework;

namespace NUnitAssertions
{
    public class AssertionsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test()
        {
            // It causes that further assestions will not be executed!
            // Assert.Pass();

            Customer c = null;

            //Assert.AreEqual("Jacek", c?.Name);
            Assert.AreEqual("Jacek", null);
        }
    }
}