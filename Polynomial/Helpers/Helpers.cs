using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Polynomial
{
    public enum OperatorAssociativity
    {
        NONE = 0,
        LEFT,
        RIGHT
    }
    public enum TokenType
    {
        None = 0,
        Operand,
        Operator
    }
    public enum Operator
    {
        None = 0,
        Plus,
        Minus,
        Multiplication,
        Division,
        Exponent
    }
    public enum VariableDisplayOrder
    {
        x = 'x',
        y = 'y',
        z = 'z'
    }

    public static class Helpers
    {
        public static TokenType GetTokenType(string token)
        {
            return IsOperand(token) ? TokenType.Operand : 
                   IsOperator(token) ? TokenType.Operator : 
                   TokenType.None;
        }
        public static Operator GetOperatorType(string token)
        {
            var operatorType = Operator.None;

            switch (token)
            {
                case "+":
                    operatorType = Operator.Plus;
                    break;
                case "-":
                    operatorType = Operator.Minus;
                    break;
                case "*":
                    operatorType = Operator.Multiplication;
                    break;
                case "/":
                    operatorType = Operator.Division;
                    break;
                case "^":
                    operatorType = Operator.Exponent;
                    break;
                default:
                    operatorType = Operator.None;
                    break;
            }

            return operatorType;
        }

        /// <summary>
        /// Returns true if token is an operand (as opposed to an operator), false otherwise.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsOperand(string token)
        {
            var isOperand = false;
            var operatorType = Helpers.GetOperatorType(token);
            if (operatorType == Operator.None)
            {
                var matchNum = Regex.Match(token, @"-?[0-9]+");
                var matchVar = Regex.Match(token, @"[a-zA-Z]+");
                isOperand = !string.IsNullOrEmpty(matchNum.Value) || !string.IsNullOrEmpty(matchVar.Value);
            }
            else
            {
                isOperand = false;
            }

            return isOperand;
        }

        /// <summary>
        /// Returns true if token is an operator (+, -, *, /, etc), false otherwise.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsOperator(string token)
        {
            var operatorType = GetOperatorType(token);
            return operatorType != Operator.None && Enum.IsDefined(typeof(Operator), operatorType);
        }

        public static int GetOperatorPrecedence(char operatorToken)
        {
            var precedence = -1;

            switch (operatorToken)
            {
                case '+':
                case '-':
                    precedence = 1;
                    break;
                case '*':
                case '/':
                    precedence = 2;
                    break;
                case '^':
                    precedence = 3;
                    break;
                default:
                    precedence = -1;
                    break;
            }

            return precedence;
        }
        public static OperatorAssociativity GetOperatorAssociativity(char operatorToken)
        {
            var associativity = OperatorAssociativity.NONE;

            switch (operatorToken)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                    associativity = OperatorAssociativity.LEFT;
                    break;
                case '^':
                    associativity = OperatorAssociativity.RIGHT;
                    break;
                default:
                    associativity = OperatorAssociativity.NONE;
                    break;
            }

            return associativity;
        }

        public static string GenerateOutputFormat(IList<PolynomialNode<double>> nodes)
        {
            var expression = "";
            foreach (var node in nodes)
            {
                var term = BuildPolynomialTerm(node);
                expression += $"{term}";
            }

            expression = expression.TrimStart('+');
            expression = Regex.Replace(expression, @"\+", " + ");
            expression = Regex.Replace(expression, @"\-", " - ");
            expression = expression == "" ? "0" : expression;
            expression = expression.Trim();

            return $"{expression} = 0";
        }
        public static string BuildPolynomialTerm(PolynomialNode<double> node)
        {
            var variables = node.Variables.Select(kvp => kvp.Key + ((kvp.Value == 1) ? "" : "^" + kvp.Value)).ToArray();
            var arr = string.Join("", variables);
            var sign = node.Coefficient >= 0 ? 1 : -1;
            var coefficient = Math.Abs(node.Coefficient) == 1 ? "" : Math.Abs(node.Coefficient).ToString();

            var term = "";
            if (sign > 0)
                term = $"+{coefficient}{arr}";
            else
                term = $"-{coefficient}{arr}";

            return term;
        }

        // Extension Methods
        public static bool In<T>(this T source, params T[] list)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return list.Contains(source);
        }
    }
}
