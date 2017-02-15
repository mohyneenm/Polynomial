using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    /// <summary>
    /// Supports polynomial terms of the form ax^k where 'a' is a double, 'x' is a variable and 'k' is an integer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PolynomialNode<T> : IComparable<T>
    {
        public T Coefficient { get; set; }
        public Dictionary<char, int> Variables { get; set; } = new Dictionary<char, int>();   // variables and exponents

        public int CompareTo(T other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var variables = Variables.Count > 0 ? Variables.Select(kvp => kvp.Key + "^" + kvp.Value).Aggregate((s1, s2) => s1 + s2) : "";
            return Coefficient.ToString() + variables;
        }
    }
}
