using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Sprache;

namespace Arithmetic
{
    public class Calculator
    {
        private static readonly Parser<string> NumberParse =
            (from num in Parse.Decimal.Once() select num.First()).Token();

        private static readonly Parser<char> SignParse =
            (from sign in Parse.Chars('+', '-', '*', '/').Once() select sign.First()).Token();

        private static readonly Parser<char> BracketsParser = Parse.Char('(').Token().Or(Parse.Char(')').Token());

        public double Sum(string expr)
        {
            Sum(expr.AsSpan());

            var numberStack = new Stack<double>();
            var signStack = new Stack<char>();
            var index = 0;

            numberStack.Push(GetNextValue(expr, ref index));
            while (index < expr.Length)
            {
                var sign = GetNextSign(expr, ref index);
                var secondNumber = GetNextValue(expr, ref index);

                if (sign == '+' || sign == '-')
                {
                    //当符号为+ -时，优先计算出栈中的内容，并将结果入栈
                    SumStack(numberStack, signStack);
                    //剩余的运算符和数字入栈
                    signStack.Push(sign);
                    numberStack.Push(secondNumber);
                }
                else if (sign == '*' || sign == '/')
                {
                    var result = sign == '*' ? numberStack.Pop() * secondNumber : numberStack.Pop() / secondNumber;
                    numberStack.Push(result);
                }
            }
            SumStack(numberStack, signStack);

            return numberStack.Pop();
        }

        private void Sum(ReadOnlySpan<char> span)
        {
            var index =0;
          var a=  MatchDouble(span, ref index);
            var b= MatchSign(span, ref index);
            var c = MatchDouble(span, ref index);
        }

        /// <summary>
        /// 返回WhiteSpace长度
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        private int WhiteSpace(ReadOnlySpan<char> span)
        {
            var length = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (Char.IsWhiteSpace(span[i]))
                    length++;
                else
                    break;
            }
            return length;
        }

        private char[] Signs = new[] {'+','-','*','/'};
        private Result<char> MatchSign(ReadOnlySpan<char> span, ref int index)
        {
            var result = new Result<char>{ Position = index };
            var start = WhiteSpace(span.Slice(index));
            var i =index +start;

            if (Signs.Contains(span[i]))
            {
                result.IsSuccessfu = true;
                i++;
                result.Position = i + WhiteSpace(span.Slice(i));
                result.Value = span[i];
                index = result.Position;
            }
            return result;
        }

        /// <summary>
        /// 匹配数字
        /// </summary>
        /// <param name="span"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Result<double> MatchDouble(ReadOnlySpan<char> span, ref int index)
        {
            var result = new Result<double> { Position = index };
            var start = WhiteSpace(span.Slice(index));
            var end =index+ start;
            var existDot = false;
            for (; end < span.Length; end++)
            {
                if (char.IsNumber(span[end]))
                    continue;
                else if (span[end] == '.' && !existDot)
                    existDot = true;
                else
                    break;
            }
            if (end-index > start)
            {
                result.IsSuccessfu = true;
                result.Position = end  + WhiteSpace(span.Slice(end));
                result.Value = Double.Parse(span.Slice(index+start, end -index- start));
                index = result.Position;
            }
            return result;
        }

        /// <summary>
        /// 获取下一个运算符
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private char GetNextSign(string expr, ref int index)
        {
            var sign = SignParse.TryParse(expr.Substring(index));
            index += sign.Remainder.Position;
            return sign.Value;
        }

        /// <summary>
        /// 获取下一个值，如果是表达式，计算出结果返回
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double GetNextValue(string expr, ref int index)
        {
            var first = NumberParse.TryParse(expr.Substring(index));
            if (first.WasSuccessful)
            {
                index += first.Remainder.Position;
                return Convert.ToDouble(first.Value);
            }
            else
            {
                var subExpr = SubExprString(expr, ref index);
                return Sum(subExpr);
            }
        }

        /// <summary>
        /// 截取子表达式
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string SubExprString(string expr, ref int index)
        {
            //递归计算出括号中的运算符
            var bracketStack = new Stack<int>();
            //计算括号内容的开始和结尾，用于截取完整的子表达式
            var start = 0;
            var end = 0;
            do
            {
                var bracket = BracketsParser.TryParse(expr.Substring(index));
                if (bracket.WasSuccessful)
                {
                    index += bracket.Remainder.Position;
                    if (bracket.Value == '(')
                    {
                        bracketStack.Push(index);
                    }
                    else if (bracket.Value == ')')
                    {
                        start = bracketStack.Pop();
                        end = index - start - bracket.Remainder.Position;
                    }
                }
                else
                {
                    index += 1;
                }
            } while (bracketStack.Count > 0);
            return expr.Substring(start, end);
        }

        /// <summary>
        /// 计算栈中的内容
        /// </summary>
        /// <param name="numberStack"></param>
        /// <param name="signStack"></param>
        /// <returns></returns>
        private void SumStack(Stack<double> numberStack, Stack<char> signStack)
        {
            if(numberStack.Count==1)
                return;
            
            if (numberStack.Count ==2)
            {
                var right = numberStack.Pop();
                var sign = signStack.Pop();
                var left = numberStack.Pop();
                numberStack.Push(sign == '+' ? left + right : left - right);
            }
        }
    }
}