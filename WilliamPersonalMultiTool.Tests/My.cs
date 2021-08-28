using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Tests
{
    public class My
    {
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