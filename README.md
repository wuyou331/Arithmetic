 一个C#写的四则运算表达式的计算器；<br/>
使用字符解析，没有使用正则或其他文法解析库，并且用了.Net Core 2.1新增的Span库来提升效率，所以只能运行在.Net Core2.1以上的版本。<br/>
```C#
var cacl = new Calculator();
var result = cacl.Sum("  (  ((1 + 1  )+  (321+3)) +(   3 + 2))*  4/((6    -4)+2)");
Assert.AreEqual(result , (((1 + 1) + (321 + 3)) + (3 + 2)) * 4 / ((6 - 4) + 2));
```

Git分支里有两种解析版本的代码Span和[Sparche](https://github.com/sprache/Sprache)，经过测试对比，使用Span类库的代码效率提升显著：<br/>
测试代码：<br/>
```C#
 var cacl = new Calculator();
 var expr = " ( 123.22  +1+   2*   4   )   +((2-1)*1-(3+(341-5-12-2)  ))-6/2 ";
 var result = cacl.Sum(expr);

 var start = DateTime.Now;
 for (int i = 0; i < 200000; i++)
 {
     cacl.Sum(expr);
 }
 var end = DateTime.Now;
Console.WriteLine(end-start);
```

以下结果为运行在Debug模式下的测试结果：<br/>

<table style="width:100px;"  >
	<tbody>
		<tr>
			<td>
				<br />
			</td>
			<td>
				Sprache
			</td>
			<td>
				Span
			</td>
		</tr>
		<tr>
			<td>
				1万次<br />
			</td>
			<td>
				00:00:02.6483960<br />
			</td>
			<td>
				00:00:00.2008868&nbsp;<br />
			</td>
		</tr>
		<tr>
			<td>
				5万次<br />
			</td>
			<td>
				00:00:13.8313581<br />
			</td>
			<td>
				00:00:00.9584799<br />
			</td>
		</tr>
		<tr>
			<td>
				10万次<br />
			</td>
			<td>
				00:00:25.0149718<br />
			</td>
			<td>
				00:00:01.9015189<br />
			</td>
		</tr>
		<tr>
			<td>
				20万次<br />
			</td>
			<td>
				00:00:50.8499154<br />
			</td>
			<td>
				00:00:03.8516696<br />
			</td>
		</tr>
	</tbody>
</table>
<br />
