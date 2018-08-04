using System;
using System.Collections.Generic;
using System.Text;

namespace Arithmetic
{
    internal class Result<T>
    {
        public bool IsSuccessfu { get; set; } = false;
        public int Position { get; set; }
        public T Value { get; set; }
    }
}