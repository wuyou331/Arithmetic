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
            (from s in Parse.Chars('+', '-', '*', '/').Once() select s.First()).Token();

        private static readonly Parser<char> BracketsParser = Parse.Char('(').Token().Or(Parse.Char(')').Token());

        public double Sum(string expr)
        {
            var numberStack = new Stack<double>();
            var signStack = new Stack<char>();
            var index = 0;

            numberStack.Push(GetNextValue(expr, ref index));
            while (index < expr.Length)
            {
                var sign = SignParse.TryParse(expr.Substring(index));
                index += sign.Remainder.Position;

                var number = GetNextValue(expr,ref index);

                if (sign.Value == '+' || sign.Value == '-')
                {
                    //当符号为+ -时，优先计算出栈中的内容，并将结果入栈
                    if (numberStack.Count > 1)
                    {
                        SumStack(numberStack, signStack);
                    }

                    signStack.Push(sign.Value);
                    numberStack.Push(number);
                }
                else if (sign.Value == '*' || sign.Value == '/')
                {
                    var result = 0d;
                    if (sign.Value == '*')
                    {
                        result = numberStack.Pop() * number;
                    }
                    else if (sign.Value == '/')
                    {
                        result = numberStack.Pop() / number;
                    }
                    numberStack.Push(result);
                }
            }
            SumStack(numberStack, signStack);

            return numberStack.Pop();
        }

        /// <summary>
        /// 获取下一个值，如果是表达式，计算出结果返回
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double GetNextValue(string expr, ref int index)
        {
            var first = NumberParse.TryParse(expr);
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
            while (numberStack.Count > 1)
            {
                var right = numberStack.Pop();
                var sign = signStack.Pop();
                var left = numberStack.Pop();
                numberStack.Push(sign == '+' ? right + left : left - right);
            }

        }
    }
}