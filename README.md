 一个C#写的四则运算表达式的计算器；<br/>
使用字符解析，没有使用正则或其他文法解析库，并且用了.Net Core 2.1新增的Span库来提升效率，所以只能运行在.Net Core2.1以上的版本。<br/>
```C#
var cacl = new Calculator();
var result = cacl.Sum("  (  ((1 + 1  )+  (321+3)) +(   3 + 2))*  4/((6    -4)+2)");
Assert.AreEqual(result , (((1 + 1) + (321 + 3)) + (3 + 2)) * 4 / ((6 - 4) + 2));
```
