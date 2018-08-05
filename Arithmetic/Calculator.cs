using System;
using System.Collections.Generic;
using System.Linq;

namespace Arithmetic
{
    public class Calculator
    {
        public double Sum(string expr)
        {
            return Sum(expr.AsSpan());
        }

        private double Sum(ReadOnlySpan<char> span)
        {
            var numberStack = new Stack<double>();
            var signStack = new Stack<char>();
            var index = 0;

            numberStack.Push(GetNextDouble(span, ref index));
            while (index < span.Length)
            {
                var sign = GetNextSign(span, ref index);
                var secondNumber = GetNextDouble(span, ref index);

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

        #region 文本分析

        /// <summary>
        /// 返回空白长度
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        private int SkipWhiteSpace(ReadOnlySpan<char> span)
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

        /// <summary>
        /// 匹配一个字符
        /// </summary>
        /// <param name="span"></param>
        /// <param name="chars"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private Result<char> MatchChars(ReadOnlySpan<char> span, char[] chars)
        {
            var result = new Result<char>();
            var start = SkipWhiteSpace(span);
            var i = start;

            if (chars.Contains(span[i]))
            {
                result.IsSuccessfu = true;
                result.Position = i + 1 + SkipWhiteSpace(span.Slice(i));
                result.Value = span[i];
            }
            return result;
        }

        /// <summary>
        /// 匹配数字
        /// </summary>
        /// <param name="span"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Result<double> MatchDouble(ReadOnlySpan<char> span)
        {
            var result = new Result<double> { };
            var start = SkipWhiteSpace(span);
            var end = start;
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
            if (end > start)
            {
                result.IsSuccessfu = true;
                result.Position = end + SkipWhiteSpace(span.Slice(end));
                result.Value = Double.Parse(span.Slice(start, end - start));
            }
            return result;
        }

        #endregion

        #region  解析步骤

        private readonly char[] Signs = new[] {'+', '-', '*', '/'};

        /// <summary>
        /// 获取下一个运算符
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private char GetNextSign(ReadOnlySpan<char> span, ref int index)
        {
            var sign = MatchChars(span.Slice(index), Signs);
            index += sign.Position;
            return sign.Value;
        }

        /// <summary>
        /// 获取下一个值，如果是表达式，计算出结果返回
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double GetNextDouble(ReadOnlySpan<char> span, ref int index)
        {
            var first = MatchDouble(span.Slice(index));
            if (first.IsSuccessfu)
            {
                index += first.Position;
                return Convert.ToDouble(first.Value);
            }
            else
            {
                var subExpr = SubExprString(span, ref index);
                return Sum(subExpr);
            }
        }

        private readonly char[] Brackets = new[] {'(', ')'};

        /// <summary>
        /// 截取子表达式
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ReadOnlySpan<char> SubExprString(ReadOnlySpan<char> span, ref int index)
        {
            //递归计算出括号中的运算符
            var bracketStack = new Stack<int>();
            //计算括号内容的开始和结尾，用于截取完整的子表达式
            var start = 0;
            var end = 0;
            do
            {
                var bracket = MatchChars(span.Slice(index), Brackets);
                if (bracket.IsSuccessfu)
                {
                    index += bracket.Position;
                    if (bracket.Value == '(')
                    {
                        bracketStack.Push(index);
                    }
                    else if (bracket.Value == ')')
                    {
                        start = bracketStack.Pop();
                        end = index - start - bracket.Position;
                    }
                }
                else
                {
                    index += 1;
                }
            } while (bracketStack.Count > 0);
            return span.Slice(start, end);
        }

        #endregion


        /// <summary>
        /// 计算栈中的内容
        /// </summary>
        /// <param name="numberStack"></param>
        /// <param name="signStack"></param>
        /// <returns></returns>
        private void SumStack(Stack<double> numberStack, Stack<char> signStack)
        {
            if (numberStack.Count == 1)
                return;

            if (numberStack.Count == 2)
            {
                var right = numberStack.Pop();
                var sign = signStack.Pop();
                var left = numberStack.Pop();
                numberStack.Push(sign == '+' ? left + right : left - right);
            }
        }
    }
}