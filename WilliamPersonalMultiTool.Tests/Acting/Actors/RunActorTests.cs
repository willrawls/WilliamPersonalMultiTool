using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class RunActorTests
    {
        [TestMethod]
        public void RunActor_Simple()
        {
            List<PKey> expected = TestPKeys.Caps123;

            // Act
            BaseActor actual = ActorHelper.Factory("CapsLock 123 run maximized notepad");

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