using System;
using System.Collections.Generic;
using System.Text;

namespace Arithmetic
{
    internal ref struct Result
    {
        public bool IsSuccessfu { get; set; } 
        public int Position { get; set; }
        public ReadOnlySpan<char> Value { get; set; }

     
    }
}