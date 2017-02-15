using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Polynomial
{
    public class PolynomialParser : IParser
    {
        ITokenizer Tokenizer { get; set; }

        public PolynomialParser(ITokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }

        /// <summary>
        /// Simplifies a given postfix polynomial expression into canonical form.
        /// </summary>
        /// <param name="postfixExpression"></param>
        /// <returns></returns>
        public virtual IList<PolynomialNode<double>> SimplifyExpression(string postfixExpression)
        {
            // The polynomial could have been implemented as a LinkedList or an Expression tree, but a List is way simpler.
            // Here we are representing a polynomial expression as a List of PolynomialNodes.
            var stack = new Stack<List<PolynomialNode<double>>>();
            IList<string> tokens = Tokenizer.Tokenize(postfixExpression);

            foreach (var token in tokens)
            {
                if (Helpers.IsOperand(token))   // operand
                {
                    AddToExpressionStack(token, stack);
                }
                else if(Helpers.IsOperator(token))  // operator
                {
                    var operatorType = Helpers.GetOperatorType(token);
                    var numOfArgs = NumberOfArgs(operatorType);
                    if (stack.Count > 1 && stack.Count < numOfArgs)
                    {
                        throw new Exception("stack is missing required number of arguments");
                    }
                    else
                    {
                        var args = new List<List<PolynomialNode<double>>>();
                        while (numOfArgs > 0 && stack.Count > 0)
                        {
                            args.Add(stack.Pop());
                            numOfArgs--;
                        }
                        args.Reverse();
                        var result = CombineExpressions(operatorType, args);
                        stack.Push(result);
                    }
                }
            }

            if (stack.Count == 1)
                return stack.Pop();
            else if(stack.Count == 0)
                return new List<PolynomialNode<double>>();
            else
                throw new Exception("too many values in stack");
        }

        /// <summary>
        /// Creates list of PolynomialNode (expression) and adds it to stack.
        /// </summary>
        /// <param name="polynomialTerm"></param>
        /// <param name="expressionStack"></param>
        private void AddToExpressionStack(string polynomialTerm, Stack<List<PolynomialNode<double>>> expressionStack)
        {
            var polyNode = new PolynomialNode<double>() { };
            polyNode.Coefficient = GetCoefficient(polynomialTerm);

            // case: AX^mY^n
            var i = 0;
            while (i < polynomialTerm.Length)
            {
                var c = polynomialTerm[i];
                if (char.IsLetter(c))
                {
                    // case: x^k
                    if ((i + 1) < polynomialTerm.Length && polynomialTerm[i + 1] == '^' && (i + 2) < polynomialTerm.Length && char.IsDigit(polynomialTerm[i + 2]))
                    {
                        var exponent = int.Parse(polynomialTerm[i + 2].ToString());
                        polyNode.Variables.Add(c, exponent);
                        i += 3;
                        continue;
                    }
                    else
                    {
                        // case: x
                        polyNode.Variables.Add(c, 1);
                        i++;
                        continue;
                    }
                }
                i++;
            }

            var list = new List<PolynomialNode<double>>();
            list.Add(polyNode);
            expressionStack.Push(list);
        }

        /// <summary>
        /// Returns the coefficient of a polynomial term.
        /// </summary>
        /// <param name="polynomialTerm"></param>
        /// <returns></returns>
        private double GetCoefficient(string polynomialTerm)
        {
            if (string.IsNullOrWhiteSpace(polynomialTerm))
                throw new Exception("polynomial term was empty");

            // Extract number from strings like: 3, 2.5, 3x, 2.5x, 7.4x^2, 4.2x^2y^3, x, xy
            var coefficient = Regex.Match(polynomialTerm, @"^[0-9]+(\.[0-9]+)?");

            if (!string.IsNullOrWhiteSpace(coefficient.Value))
            {
                double val;
                if (double.TryParse(coefficient.ToString(), out val))
                    return val;
                else
                    throw new Exception($"Error converting coefficient to number");
            }
            else
            {
                if (char.IsLetter(polynomialTerm.First()))
                    return 1.0;
                else
                    throw new Exception($"Error converting coefficient to number");
            }
        }

        /// <summary>
        /// Applies the operator to a list of expressions 
        /// </summary>
        /// <param name="operatorType"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        private List<PolynomialNode<double>> CombineExpressions(Operator operatorType, List<List<PolynomialNode<double>>> expressions)
        {
            var num = NumberOfArgs(operatorType);
            if (num != expressions.Count)
                throw new Exception($"Wrong number of arguments for operator '{operatorType}'");

            if (operatorType == Operator.Plus || operatorType == Operator.Minus)
            {
                var firstExpression = expressions[0];
                var secondExpression = expressions[1];

                // Given two expressions (2x @ y) + (3x @ y), where @ is any operator, we want to find if any term in the 2nd expression can be
                // added to terms in the first expression. If not then we just concatenate the 2nd expression terms to the first expression.
                foreach (var node2 in secondExpression)
                {
                    bool termsHaveSameVariables = false;

                    // first look for matching variables in the terms
                    foreach (var node1 in firstExpression)
                    {
                        termsHaveSameVariables = CompareVariables(node2, node1);
                        if (termsHaveSameVariables)
                        {
                            node1.Coefficient = (operatorType == Operator.Plus) ? (node1.Coefficient + node2.Coefficient) : (node1.Coefficient - node2.Coefficient);
                            break;
                        }
                    }

                    // if no matching variables are found for this term then add a new node to the end of firstExpression
                    if (!termsHaveSameVariables)
                    {
                        int sign = operatorType == Operator.Plus ? 1 : -1;
                        node2.Coefficient *= sign;
                        firstExpression.Add(node2);
                    }
                }

                // remove '0' valued coefficients
                for (var i = firstExpression.Count - 1; i >= 0; i--)
                {
                    if (firstExpression[i].Coefficient == 0)
                        firstExpression.RemoveAt(i);
                }

                // firstExpression now contains the terms from secondExpression, either inserted into firstExpresion terms or as new terms added to the end
                return firstExpression;
            }
            return null;
        }

        /// <summary>
        /// Check if the two polynomial terms have the same type of variables (eg: 2xy vs 5yx)
        /// </summary>
        private static bool CompareVariables(PolynomialNode<double> term1, PolynomialNode<double> term2)
        {
            if (term2.Variables.Count == term1.Variables.Count)
            {
                bool varsMatch = true;
                foreach (var kvp in term2.Variables)
                {
                    varsMatch = varsMatch && term1.Variables.ContainsKey(kvp.Key) && term1.Variables[kvp.Key] == kvp.Value;
                }

                return varsMatch;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the number of arguments an operator takes.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private static int NumberOfArgs(Operator op)
        {
            if (op == Operator.Plus || op == Operator.Minus || op == Operator.Multiplication || op == Operator.Division || op == Operator.Exponent)
                return 2;
            else
                return -1;
        }

        public Expression Parse(string postfixExpression)
        {
            throw new NotImplementedException();
        }
    }
}
