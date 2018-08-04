using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arithmetic.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cacl = new Calculator();
            var result = cacl.Sum(" (123.22) +((2-1)*1-(3+(341-5-12-2)  ))-6/2 ");
            Assert.IsTrue(result == 123.22 + ((2 - 1) * 1-(3 + (341 - 5 - 12 - 2))) - 6 / 2);
        }
    }
}