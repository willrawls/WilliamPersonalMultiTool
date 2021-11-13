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
        public void TypeActor_WithTwoContinuations()
        {
            var expected = @"abc123
Continued
And again.";
            // Act
            TypeActor typeActor = (TypeActor) ActorHelper.Factory(@"When CapsLock 123 type abc123
Continued
And again.");
            Assert.IsNotNull(typeActor);
            Assert.AreEqual(expected, typeActor.TextToType);
        }
    }
}