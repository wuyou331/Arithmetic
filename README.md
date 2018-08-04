一个C#写的四则运算表达式的计算器，使用了Sprache库替代正则表达式作为文法解析库。
```C#
var cacl = new Calculator();
var result = cacl.Sum(" (123.22) +((2-1)*1-(3+(341-5-12-2)  ))-6/2 ");
Assert.IsTrue(result == 123.22 + ((2 - 1) * 1-(3 + (341 - 5 - 12 - 2))) - 6 / 2);
```
