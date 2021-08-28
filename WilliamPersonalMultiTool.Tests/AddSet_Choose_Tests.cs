using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class AddSet_Choose_Tests
    {
        private const string SetText = @"
When CapsLock 1 2 3 type someone.at@hotmail.com
When CapsLock 1 2 4 choose
    abc
    def
    ghi
When CapsLock 1 2 P choose
jkl
  mno
When CapsLock 1 2 5 type someone.at@hotmail.com
";

        readonly List<PKey> pKeys_Caps123 = new() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D3 };
        readonly List<PKey> pKeys_Caps124 = new() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D4 };
        readonly List<PKey> pKeys_Caps12P = new() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.P };
        readonly List<PKey> pKeys_Caps125 = new() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D5 };

        readonly List<PKey> ShiftX2 = new() { PKey.Shift, PKey.X };

        [TestMethod]
        public void AddSet_SetText_Basic()
        {
            var data = new CustomPhraseManager();
            var actual = data.AddSet(SetText);
            Assert.AreEqual(4, actual.Count);
        }

        [TestMethod]
        public void AddSet_Choose_124_3Choices()
        {
            var data = new CustomPhraseManager();
            var set = data.AddSet(SetText);
            var actual = set[1];
            Assert.IsNotNull(actual.Choices);
            Assert.AreEqual(3, actual.Choices.Count);
            AssertAllAreEqual(pKeys_Caps124, actual.Sequence);
            Assert.AreEqual("abc", actual.Choices[0].Text);
            Assert.AreEqual("def", actual.Choices[1].Text);
            Assert.AreEqual("ghi", actual.Choices[2].Text);
        }

        [TestMethod]
        public void AddSet_Choose_12P_2Choices()
        {
            var data = new CustomPhraseManager();
            var set = data.AddSet(SetText);
            var actual = set[2];
            Assert.IsNotNull(actual.Choices);
            Assert.AreEqual(2, actual.Choices.Count);
            AssertAllAreEqual(pKeys_Caps12P, actual.Sequence);
            Assert.AreEqual("jkl", actual.Choices[0].Text);
            Assert.AreEqual("mno", actual.Choices[1].Text);
        }

        public static void AssertAllAreEqual(List<PKey> expected, List<PKey> actual)
        {
            Assert.IsNotNull(actual);
            
            var debug = "\nExpected: ";
            foreach (var key in expected)
                debug += key.ToString() + " ";

            debug += "\nActual:   ";
            foreach (var key in actual)
                debug += key.ToString() + " ";

            Assert.AreEqual(expected.Count, actual.Count, debug);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(actual[i], expected[i], $"{debug}: Index {i}");
            }
        }
    }
}