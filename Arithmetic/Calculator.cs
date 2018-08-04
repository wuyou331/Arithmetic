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


            var first = NumberParse.TryParse(expr);
            index += first.Remainder.Position;
            numberStack.Push(Convert.ToDouble(first.Value));
            while (index < expr.Length)
            {
                var sign = SignParse.TryParse(expr.Substring(index));
                index += sign.Remainder.Position;
                var number = 0d;
                var numberResult = NumberParse.TryParse(expr.Substring(index));
                if (numberResult.WasSuccessful)
                {
                    index += numberResult.Remainder.Position;
                    number = Convert.ToDouble(numberResult.Value);
                }
                else
                {
                    var bracketStack = new Stack<int>();
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

                    var sub = expr.Substring(start, end);
                    number = Sum(sub);
                }

                if (sign.Value == '+' || sign.Value == '-')
                {
                    if (numberStack.Count > 1)
                    {
                        var firstSum = SumStack(numberStack, signStack);
                        numberStack.Push(firstSum);
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


            return SumStack(numberStack, signStack);
        }

        private double SumStack(Stack<double> numberStack, Stack<char> signStack)
        {

            while (numberStack.Count > 1)
            {
                var right = numberStack.Pop();
                var sign = signStack.Pop();
                var left = numberStack.Pop();
                numberStack.Push(sign == '+' ? right + left : left - right);
            }

            return numberStack.Pop();
        }
    }
}