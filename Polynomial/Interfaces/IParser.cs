using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public interface IParser
    {
        IList<PolynomialNode<double>> SimplifyExpression(string postfixExpression);
        Expression Parse(string postfixExpression);
    }
}
