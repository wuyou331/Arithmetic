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
            var result = cacl.Sum("  (  ((1 + 1  )+  (321+3)) +(   3 + 2))*  4/((6    -4)+2)");
            Assert.AreEqual(result , (((1 + 1) + (321 + 3)) + (3 + 2)) * 4 / ((6 - 4) + 2));
        }
    }
}