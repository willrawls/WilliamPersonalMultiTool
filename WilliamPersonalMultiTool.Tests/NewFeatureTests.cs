
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;
using WilliamPersonalMultiTool.Custom;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class NewFeatureTests
    {
        [TestMethod]
        public void TextBetweenSlashStarAreComments()
        {
            var actual = Build("A B /* C */D type 123");
            My.AssertAllAreEqual(TestPKeys.ABD, actual.Sequence);
        }

        [TestMethod]
        public void KeysNotSeparatedBySpacesGetInterpretedAsIfThereWereSpaces()
        {
            var actual = Build("CapsLock ABD type 123");
            My.AssertAllAreEqual(TestPKeys.ABD, actual.Sequence);
        }

        [TestMethod]
        public void ActionsCanBeTwoWords_MoveTo()
        {
            var actual = Build("CapsLock A move to 1 2 3 4");
            Assert.AreEqual("1 2 3 4", actual.Arguments);

            Assert.IsNotNull(actual.Actor);
            Assert.AreEqual(ActionableType.Move, actual.Actor.ActionableType);
            var moveActor = (MoveActor) actual.Actor;

            Assert.AreEqual("to", moveActor.Verb);
            Assert.AreEqual(1, moveActor.Left);
            Assert.AreEqual(2, moveActor.Top);
            Assert.AreEqual(3, moveActor.Width);
            Assert.AreEqual(4, moveActor.Height);
        }

        [TestMethod]
        public void ActionsCanBeTwoWords_MovePercent()
        {
            var actual = Build("CapsLock A move % 10 10 75 50");
            Assert.IsInstanceOfType(actual.Actor, typeof(MoveActor));
            My.AssertAllAreEqual(TestPKeys.ABD, actual.Sequence);
            Assert.AreEqual("1 2 3 4", actual.Arguments);

            Assert.IsNotNull(actual.Actor);
            Assert.AreEqual(ActionableType.Move, actual.Actor.ActionableType);
            var actor = (MoveActor) actual.Actor;

            Assert.AreEqual("percent", actor.Verb);
            Assert.AreEqual(10, actor.Left);
            Assert.AreEqual(10, actor.Top);
            Assert.AreEqual(75, actor.Width);
            Assert.AreEqual(50, actor.Height);
        }

        [TestMethod]
        public void VerbHasADefault()
        {
            var actual = Build("CapsLock A size 500 400");
            Assert.AreEqual(ActionableType.Size, actual.Actor.ActionableType);
            var actor = (SizeActor) actual.Actor;
            Assert.AreEqual("to", actor.Verb);
        }

        [TestMethod]
        public void SizeActor_SizePercent()
        {
            var actual = Build("CapsLock A size % -10 +10");
            Assert.AreEqual(ActionableType.Size, actual.Actor.ActionableType);
            var actor = (SizeActor) actual.Actor;
            Assert.AreEqual("percent", actor.Verb);
        }

        [TestMethod]
        public void TypeActor_RandomString_Letters()
        {
            var actual = Build("CapsLock A type .{10 random letters}.");
            Assert.AreEqual(ActionableType.Type, actual.Actor.ActionableType);
            var actor = (TypeActor) actual.Actor;
            Assert.AreEqual("expand", actor.Verb);
        }

        [TestMethod]
        public void TypeActor_RandomString_Numbers()
        {
            var actual = Build("CapsLock A type .{Random number 1 to 20}.");
            Assert.AreEqual(ActionableType.Type, actual.Actor.ActionableType);
            var actor = (TypeActor) actual.Actor;
            Assert.AreEqual("expand", actor.Verb);
        }

        [TestMethod]
        public void TypeActor_Hidden()
        {
            var actual = Build("CapsLock A type hidden fred");
            Assert.AreEqual(ActionableType.Type, actual.Actor.ActionableType);
            var actor = (TypeActor) actual.Actor;
            Assert.AreEqual("hidden", actor.Verb);
        }

        [TestMethod]
        public void RepeatActor_RepeatLast()
        {
            var actual = Build("CapsLock A repeat");
            Assert.AreEqual(ActionableType.Repeat, actual.Actor.ActionableType);
            var actor = (RepeatActor) actual.Actor;
            Assert.AreEqual("last", actor.Verb);
            Assert.AreEqual(1, actor.RepeatLastCount);
        }

        [TestMethod]
        public void RepeatActor_RepeatLast4()
        {
            var actual = Build("CapsLock A repeat last 4");
            Assert.AreEqual(ActionableType.Repeat, actual.Actor.ActionableType);
            var actor = (RepeatActor) actual.Actor;
            Assert.AreEqual("last", actor.Verb);
            Assert.AreEqual(4, actor.RepeatLastCount);
        }

        [TestMethod]
        public void TypeActor_Paste()
        {
            var actual = Build("CapsLock A type .{Paste from keyboard}.");
            Assert.AreEqual(ActionableType.Type, actual.Actor.ActionableType);
            var actor = (TypeActor) actual.Actor;
            Clipboard.SetText("123");
            Assert.AreEqual("123", actor.ClipboardText());
        }

        private static CustomKeySequence Build(string input)
        {
            var manager = new CustomPhraseManager(null, input);
            var actual = (CustomKeySequence) manager.Keyboard.KeySequences[0];
            return actual;
        }
    }
}

