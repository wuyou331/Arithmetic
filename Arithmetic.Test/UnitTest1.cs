using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arithmetic.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
          var cacl =  new Calculator();
          var result=  cacl.Sum(" 123.22 +(341-2)-6/2 ");

        }
    }
}
