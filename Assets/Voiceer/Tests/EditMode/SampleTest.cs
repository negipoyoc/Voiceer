using NUnit.Framework;

namespace Voiceer
{
    public class SampleTest
    {
        [Test]
        public void TestPassSample()
        {
            Assert.That(true, Is.True);
        }

        [Test]
        public void TestFailSample()
        {
            Assert.That(false, Is.True);
        }
    }
}