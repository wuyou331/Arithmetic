using System;
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
            var expr = " ( 123.22  +1+   2*   4   )   +((2-1)*1-(3+(341-5-12-2)  ))-6/2 ";
            var result = cacl.Sum(expr);
            Assert.IsTrue(result == (123.22 + 1 + 2 * 4) + ((2 - 1) * 1 - (3 + (341 - 5 - 12 - 2))) - 6 / 2);

        }
    }
}