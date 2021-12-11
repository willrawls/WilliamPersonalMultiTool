using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;
using WilliamPersonalMultiTool.Custom;

namespace WilliamPersonalMultiTool.Tests.Acting.Actors
{
    [TestClass]
    public class MoveActorTestsTests
    {
        [TestMethod]
        public void Simple_MoveTo_Absolute_1_2_3_4()
        {
            List<PKey> expected = TestPKeys.Caps123;

            // Act
            var customPhraseManager = new CustomPhraseManager(null);
            BaseActor actual = ActorHelper.Factory("CapsLock 123 move to 1 2 3 4", customPhraseManager);

            Assert.IsNotNull(actual);
            MoveActor moveActor = (MoveActor) actual;
            Assert.AreEqual(ActionableType.Move, actual.ActionableType);
            My.AssertAllAreEqual(expected, actual.KeySequence.Sequence);

            Assert.IsNotNull(actual.LegalVerbs);
            Assert.AreEqual(3, actual.LegalVerbs.Count);

            Assert.IsNotNull(actual.ExtractedVerbs);
            Assert.AreEqual(1, actual.ExtractedVerbs.Count);
            Assert.AreEqual("to", actual.ExtractedVerbs[0].Name);
            Assert.IsTrue(actual.ExtractedVerbs[0].Mentioned);
            Assert.IsTrue(moveActor.To.Mentioned);

            Assert.IsFalse(actual.Has(moveActor.Percent));
            Assert.IsFalse(actual.Has(moveActor.Relative));

            Assert.AreEqual(1, moveActor.Left);
            Assert.AreEqual(2, moveActor.Top);
            Assert.AreEqual(3, moveActor.Right);
            Assert.AreEqual(4, moveActor.Bottom);
            Assert.AreEqual(2, moveActor.Width);
            Assert.AreEqual(2, moveActor.Height);
        }

        [TestMethod]
        public void Simple_MoveTo_Relative_1_2_3_4()
        {
            List<PKey> expected = TestPKeys.Caps123;

            // Act
            var customPhraseManager = new CustomPhraseManager(null);
            BaseActor actual = ActorHelper.Factory("CapsLock 123 move to relative 1 2 3 4", customPhraseManager);

            Assert.IsNotNull(actual);
            MoveActor moveActor = (MoveActor) actual;
            Assert.AreEqual(ActionableType.Move, actual.ActionableType);
            My.AssertAllAreEqual(expected, actual.KeySequence.Sequence);

            Assert.IsNotNull(actual.LegalVerbs);
            Assert.AreEqual(3, actual.LegalVerbs.Count);

            Assert.IsNotNull(actual.ExtractedVerbs);
            Assert.AreEqual(2, actual.ExtractedVerbs.Count);
            Assert.AreEqual("to", actual.ExtractedVerbs[0].Name);
            Assert.IsTrue(actual.ExtractedVerbs[0].Mentioned);
            Assert.IsTrue(moveActor.To.Mentioned);

            Assert.IsFalse(actual.Has(moveActor.Percent));
            Assert.IsTrue(actual.Has(moveActor.Relative));

            Assert.AreEqual(1, moveActor.Left);
            Assert.AreEqual(2, moveActor.Top);
            Assert.AreEqual(4, moveActor.Right);
            Assert.AreEqual(6, moveActor.Bottom);
            Assert.AreEqual(3, moveActor.Width);
            Assert.AreEqual(4, moveActor.Height);
        }
    }
}