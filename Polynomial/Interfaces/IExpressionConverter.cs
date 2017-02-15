using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public interface IExpressionConverter
    {
        string InfixToPostfix(string input);
        string InfixToPrefix(string input);
    }
}
