using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class Sorter<T> : ISorter<PolynomialNode<double>>
    {
        public void Sort(ICollection<PolynomialNode<double>> collection)
        {
            // sort by exponent and variable
            // this is an ad-hoc sort function which should be improved later
            var nodes = (List<PolynomialNode<double>>)collection;
            nodes.Sort((n1, n2) =>
            {
                var maxExponnetNode1 = n1.Variables.Count > 0 ? n1.Variables.Max(kvp => kvp.Value) : 0;
                var maxExponnetNode2 = n2.Variables.Count > 0 ? n2.Variables.Max(kvp => kvp.Value) : 0;

                if (maxExponnetNode1 > maxExponnetNode2)
                    return -1;
                else if (maxExponnetNode1 < maxExponnetNode2)
                    return 1;
                else
                {
                    if (n1.Variables.Count == 1 && n2.Variables.Count == 1)
                    {
                        if (n1.Variables.ContainsKey((char)VariableDisplayOrder.x))
                            return -1;
                        else if (n2.Variables.ContainsKey((char)VariableDisplayOrder.x))
                            return 1;
                        else
                            return 0;
                    }
                    return 0;
                }
            });
        }
    }
}
