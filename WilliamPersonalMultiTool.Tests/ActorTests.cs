using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class ActorTestsTests
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

        [TestMethod]
        public void TypeActor_WithTwoContinuations()
        {
            var expected = @"abc123
Continued
And again.";
            // Act
            TypeActor typeActor = (TypeActor) ActorHelper.Factory(@"When CapsLock 123 type abc123");
            BaseActor continuation1 = (ContinuationActor) ActorHelper.Factory("Continued", typeActor);
            BaseActor continuation2 = (ContinuationActor) ActorHelper.Factory("And again.", typeActor);

            Assert.AreEqual(expected, typeActor.TextToType);
        }
    }
}