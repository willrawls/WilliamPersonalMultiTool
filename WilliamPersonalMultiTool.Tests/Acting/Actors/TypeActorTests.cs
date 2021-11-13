using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting;
using WilliamPersonalMultiTool.Acting.Actors;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class TypeActorTests
    {
        [TestMethod]
        public void TypeActor_Simple()
        {
            var expected = @"abc123";
            // Act
            TypeActor typeActor = (TypeActor) ActorHelper.Factory(@"When CapsLock 123 type abc123");
            Assert.IsNotNull(typeActor);
            CollectionAssert.AreEqual(new PKey[] {PKey.CapsLock, PKey.D1, PKey.D2, PKey.D3}, typeActor.KeySequence.Sequence);
            Assert.AreEqual(expected, typeActor.TextToType);
            
        }

        [TestMethod]
        public void TypeActor_WithTwoContinuations()
        {
            var expected = @"a.b.c";
            // Act
            TypeActor typeActor = (TypeActor) ActorHelper.Factory(@"When CapsLock 123 type a");
            typeActor.OnContinue("b");
            typeActor.OnContinue("c");

            Assert.IsNotNull(typeActor);
            Assert.AreEqual(
                "\n" + expected.Replace(" ", ".").Replace("\n", "."), 
                "\n" + typeActor.TextToType.Replace(" ", ".").Replace("\n", "."));
        }
    }
}