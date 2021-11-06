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

            BaseActor actual = ActorHelper.Factory("CapsLock CapsLock 123 run maximized notepad");

            Assert.IsNotNull(actual);
            Assert.AreEqual(ActionableType.Run, actual.ActionableType);
            Assert.AreEqual(ActionableType.Run, actual);
            
            List<PKey> expected = new List<PKey>
            {
                PKey.CapsLock,
                PKey.CapsLock,
                PKey.D1,
                PKey.D2,
                PKey.D3,
            };
            My.AssertAllAreEqual(expected, actual.KeySequence.Sequence);

            Assert.IsNotNull(actual.Verbs);
            Assert.AreEqual(1, actual.Verbs.Count);
            Assert.AreEqual("maximized", actual.Verbs[0]);
            Assert.AreEqual("notepad", actual.Arguments);

        }
    }
}