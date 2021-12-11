using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;
using WilliamPersonalMultiTool.Custom;
using Win32Interop.Structs;

namespace WilliamPersonalMultiTool.Tests.Acting.Actors
{
    [TestClass]
    public class MoveActorTests
    {
        [TestMethod]
        public void Move_To_1_2_3_4()
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
        public void Move_To_Relative_1_2_3_4()
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

        [TestMethod]
        public void Move_Relative_Percent_1_2_3_4()
        {
            List<PKey> expected = TestPKeys.Caps123;

            // Act
            var customPhraseManager = new CustomPhraseManager(null);
            BaseActor actual = ActorHelper.Factory("CapsLock 123 move percent relative 1 2 3 4", customPhraseManager);

            MoveActor moveActor = (MoveActor) actual;

            Assert.IsTrue(actual.Has(moveActor.Percent));
            Assert.IsTrue(actual.Has(moveActor.Relative));

            Assert.AreEqual(1, moveActor.Left);
            Assert.AreEqual(2, moveActor.Top);
            Assert.AreEqual(4, moveActor.Right);
            Assert.AreEqual(6, moveActor.Bottom);
            Assert.AreEqual(3, moveActor.Width);
            Assert.AreEqual(4, moveActor.Height);
        }

        [TestMethod]
        public void CalculateNewPosition_To()
        {
            var moveActor = new MoveActor();
            moveActor.Initialize("When A move to 1 2 3 4");
            var originalPosition = new RECT{ left = 10, right = 20, top = 30, bottom = 40};

            var actual = moveActor.CalculateNewPosition(originalPosition);
            Assert.AreEqual(1, actual.left);
            Assert.AreEqual(2, actual.top);
            Assert.AreEqual(3, actual.right);
            Assert.AreEqual(4, actual.bottom);
        }

        [TestMethod]
        public void CalculateNewPosition_To_Percent()
        {
            var moveActor = new MoveActor();
            moveActor.Initialize("When A move to 10 10 10 10");
            var originalPosition = new RECT{ left = 0, right = 0, top = 0, bottom = 0};

            var screen = System.Windows.Forms.Screen.AllScreens[0];
            var oneX = (screen.WorkingArea.Width - screen.WorkingArea.Left) / 100;
            var oneY = (screen.WorkingArea.Height - screen.WorkingArea.Top) / 100;
            var actual = moveActor.CalculateNewPosition(originalPosition);
            Assert.AreEqual(1, actual.left);
            Assert.AreEqual(2, actual.top);
            Assert.AreEqual(3, actual.right);
            Assert.AreEqual(4, actual.bottom);
        }
    }
}