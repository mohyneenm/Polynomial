﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class SimpleTokenizer : ITokenizer
    {
        public IList<string> Tokenize(string input)
        {
            return input.Split(' ').ToList();
        }
    }
}
