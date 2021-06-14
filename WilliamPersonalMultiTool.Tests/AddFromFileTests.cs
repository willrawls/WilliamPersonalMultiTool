using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class AddFromFileTests
    {
        readonly List<PKey> pKeys_Caps123 = new() {PKey.CapsLock, PKey.D1, PKey.D2, PKey.D3};
        readonly List<PKey> pKeys_Caps1A3 = new() {PKey.CapsLock, PKey.D1, PKey.A, PKey.D3};

        [TestMethod]
        public void ToPKeyList_Caps123()
        {
            var actual = CustomPhraseManager.ToPKeyList(null, "CapsLock 1 2 3");
            AssertAllAreEqual(pKeys_Caps123, actual);
        }

        [TestMethod]
        public void ToPKeyList_1A3_WithPrependCaps()
        {
            var actual = CustomPhraseManager.ToPKeyList(new List<PKey>{PKey.CapsLock}, "1 a 3");
            AssertAllAreEqual(pKeys_Caps1A3, actual);
        }

        [TestMethod]
        public void SingleLine_Caps123()
        {
            var data = new CustomPhraseManager();
            CustomKeySequence actual = data.AddOrReplace("CapsLock 1 2 3");
            AssertAllAreEqual(pKeys_Caps123, actual.Sequence);
        }

        private void AssertAllAreEqual(List<PKey> expected, List<PKey> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < pKeys_Caps123.Count; i++)
            {
                Assert.AreEqual(actual[i], expected[i], $"Index {i}");
            }
        }
    }
}
