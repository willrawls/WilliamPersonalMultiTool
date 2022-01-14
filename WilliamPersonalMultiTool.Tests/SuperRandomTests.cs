using System.Reflection.Metadata.Ecma335;
using MetX.Standard.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class SuperRandomTests
    {
        [TestMethod]
        [DataRow(127, 7)]
        [DataRow(128, 8)]
        [DataRow(-1, 32)]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 2)]
        [DataRow(4, 3)]
        [DataRow(int.MinValue, 32)]
        [DataRow(int.MaxValue, 31)]
        public void BitCount_Simple(int integer, int expected)
        {
            // NOTE TO SELF, Move to MetX.Tests
            Assert.AreEqual(expected, SuperRandom.BitsUsed(integer));

        }

        [TestMethod]
        public void NextUnsignedInteger_InRange()
        {
            // NOTE TO SELF, Move to MetX.Tests
            for (uint min = 0; min < 256; min++)
            {
                for (uint max = min + 101; max < 101; max++)
                {
                    for (uint iteration = 0; iteration < 102; iteration++)
                    {
                        var number = SuperRandom.NextUnsignedInteger(min + iteration, max + iteration);
                        Assert.IsTrue(number >= min + iteration);
                        Assert.IsTrue(number <= max + iteration);
                    }
                }
            }
        }

        [TestMethod]
        public void NextInteger_InRange()
        {
            // NOTE TO SELF, Move to MetX.Tests
            for (int min = 0; min < 256; min++)
            {
                for (int max = min + 101; max < 101; max++)
                {
                    for (int iteration = 0; iteration < 102; iteration++)
                    {
                        var number = SuperRandom.NextInteger(min + iteration, max + iteration);
                        Assert.IsTrue(number >= min + iteration);
                        Assert.IsTrue(number <= max + iteration);
                    }
                }
            }
        }
    }
}
