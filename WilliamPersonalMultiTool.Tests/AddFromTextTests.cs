using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class AddFromTextTests
    {
        readonly List<PKey> pKeys_Caps123 = new() {PKey.CapsLock, PKey.D1, PKey.D2, PKey.D3};
        readonly List<PKey> pKeys_Caps124 = new() {PKey.CapsLock, PKey.D1, PKey.D2, PKey.D4};
        readonly List<PKey> pKeys_Caps1A3 = new() {PKey.CapsLock, PKey.D1, PKey.A, PKey.D3};

        readonly List<PKey> ShiftX2 = new() {PKey.Shift, PKey.X};

        [TestMethod]
        public void ToPKeyList_Caps123()
        {
            var actual = CustomPhraseManager.ToPKeyList("CapsLock 1 2 3", null, out var wildcardMatchType, out var wildcardCount);
            AssertAllAreEqual(pKeys_Caps123, actual);
            Assert.AreEqual(0, wildcardCount);
        }

        [TestMethod]
        public void AddSet_Caps123_OnOneLine()
        {
            var data = new CustomPhraseManager();
            var actual = data.AddSet("When CapsLock 1 2 3 type someone.at@gmail.com");
            Assert.AreEqual("someone.at@gmail.com", actual[0].Name);
            AssertAllAreEqual(pKeys_Caps123, actual[0].Sequence);
        }

        [TestMethod]
        public void AddSet_ShiftX2WildDigits_OnOneLine()
        {
            var data = new CustomPhraseManager();
            var actual = data.AddSet("When Shift X ## type x~~~~y");
            Assert.AreEqual("x~~~~y", actual[0].Name);
            AssertAllAreEqual(ShiftX2, actual[0].Sequence);
            Assert.AreEqual(2, actual[0].WildcardCount);
            Assert.AreEqual(WildcardMatchType.Digits, actual[0].WildcardMatchType);
        }

        [TestMethod]
        public void AddSet_ShiftX2WildAlphaNumeric_OnOneLine()
        {
            var data = new CustomPhraseManager();
            var actual = data.AddSet("When Shift X ** type x~~~~y");
            Assert.AreEqual("x~~~~y", actual[0].Name);
            AssertAllAreEqual(ShiftX2, actual[0].Sequence);
            
            Assert.AreEqual(2, actual[0].WildcardCount);
            Assert.AreEqual(WildcardMatchType.AlphaNumeric, actual[0].WildcardMatchType);
        }

        [TestMethod]
        public void AddSet_Caps123and4_OnTwoLines()
        {
            var data = new CustomPhraseManager();
            var actual = data.AddSet(@"
When CapsLock 1 2 3 type someone.at@gmail.com
Or 4 type someone.at@hotmail.com
");
            Assert.AreEqual("someone.at@gmail.com", actual[0].Name);
            AssertAllAreEqual(pKeys_Caps123, actual[0].Sequence);
            Assert.AreEqual("someone.at@hotmail.com", actual[1].Name);
            AssertAllAreEqual(pKeys_Caps124, actual[1].Sequence);
        }

        [TestMethod]
        public void ToPKeyList_1A3_WithPrependCaps()
        {
            var actual = CustomPhraseManager.ToPKeyList("1 a 3", new List<PKey>{PKey.CapsLock}, out var wildcardMatchType, out var wildcardCount);
            AssertAllAreEqual(pKeys_Caps1A3, actual);
        }

        [TestMethod]
        public void AddSet_SingleLine_Caps123()
        {
            var data = new CustomPhraseManager();
            CustomKeySequence actual = data.AddOrReplace("CapsLock 1 2 3");
            AssertAllAreEqual(pKeys_Caps123, actual.Sequence);
        }

        private void AssertAllAreEqual(List<PKey> expected, List<PKey> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(actual[i], expected[i], $"Index {i}");
            }
        }
    }
}
