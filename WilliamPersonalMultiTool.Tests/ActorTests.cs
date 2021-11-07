using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class ActorTestsTests
    {
        [TestMethod]
        public void ActorTestsTest_Simple()
        {
            List<PKey> expected = new List<PKey>
            {
                PKey.CapsLock,
                PKey.CapsLock,
                PKey.D1,
                PKey.D2,
                PKey.D3,
            };

            // Act
            BaseActor actual = ActorHelper.Factory("CapsLock CapsLock 123 run maximized notepad");

            Assert.IsNotNull(actual);
            Assert.AreEqual(ActionableType.Run, actual.ActionableType);
            
            My.AssertAllAreEqual(expected, actual.KeySequence.Sequence);

            Assert.IsNotNull(actual.LegalVerbs);
            Assert.AreEqual(1, actual.LegalVerbs.Count);
            Assert.AreEqual(1, actual.ExtractedVerbs);
            Assert.AreEqual("maximized", actual.ExtractedVerbs[0]);

            Assert.IsNotNull(actual.ExtractedVerbs);
            Assert.AreEqual(1, actual.ExtractedVerbs.Count);
            Assert.AreEqual("maximized", actual.ExtractedVerbs[0]);

            Assert.AreEqual("notepad", actual.Arguments);
        }
    }
}