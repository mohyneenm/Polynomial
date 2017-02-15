using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public interface ITokenizer
    {
        IList<string> Tokenize(string input);
    }
}
