using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class ExpressionConverter : IExpressionConverter
    {
        private Queue<string> outputQueue = new Queue<string>();
        private Stack<string> operatorStack = new Stack<string>();

        /// <summary>
        /// Shunting-yard algorithm (source: wikipedia)
        /// </summary>
        /// <param name="infixExpression"></param>
        /// <returns></returns>
        public string InfixToPostfix(string infixExpression)
        {
            if (string.IsNullOrWhiteSpace(infixExpression))
                return string.Empty;

            outputQueue.Clear();
            operatorStack.Clear();

            infixExpression = SanitizeInput(infixExpression);   // handle unary minus, equality, etc

            using (var reader = new StringReader(infixExpression))
            {
                int peek;
                while ((peek = reader.Peek()) > -1)
                {
                    var token = (char)peek;
                    var operatorType = Helpers.GetOperatorType(token.ToString());

                    if (char.IsLetterOrDigit(token))
                    {
                        // read the full operand which is of the form ax^k
                        ReadOperand(reader);
                    }
                    else if (Helpers.IsOperator(token.ToString()))
                    {
                        ReadOperator(reader, token);
                    }
                    else if (token == '(')
                    {
                        operatorStack.Push("(");
                        reader.Read();
                    }
                    else if (token == ')')
                    {
                        MoveOperatorsFromStackUntilOpenParenthesis();
                        reader.Read();
                    }
                    else if (token == ' ')
                    {
                        reader.Read();
                    }
                    else
                    {
                        throw new Exception($"Encountered invalid character '{token}'");
                    }
                }

                MoveOperatorsFromStackToQueue();
            }

            return outputQueue.Aggregate((a, b) => a + " " + b);
        }

        public string InfixToPrefix(string input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Trims, handles unary minus, handles equation format
        /// </summary>
        /// <param name="infixExpression"></param>
        /// <returns></returns>
        private string SanitizeInput(string infixExpression)
        {
            infixExpression = infixExpression.Trim();
            if (infixExpression.First() == '-')
                infixExpression = "0" + infixExpression;    // format to handle unary minus operator

            if (infixExpression.Contains("="))
            {
                var expr = infixExpression.Split('=');
                if (expr.Length == 2)
                {
                    expr[1] = expr[1].Trim();
                    if (expr[1].First() == '-')
                        expr[1] = "0" + expr[1];            // format to handle unary minus operator
                }

                infixExpression = $"{expr[0]} - ({expr[1]})";
            }

            return infixExpression;
        }

        /// <summary>
        /// In a polynomial an operand is of the form ax^k, where 'a' can be a double, 'x' is a variable and 'k' is an integer.
        /// </summary>
        /// <param name="reader"></param>
        private void ReadOperand(TextReader reader)
        {
            var operand = string.Empty;
            var peek = -1;

            while ((peek = reader.Peek()) > -1)
            {
                var next = (char)peek;
                if (char.IsLetterOrDigit(next) || next == '^' || next == '.')
                {
                    reader.Read();
                    operand += next;
                }
                else
                {
                    break;
                }
            }
            outputQueue.Enqueue(operand);
        }

        /// <summary>
        /// In a polynmial an operator will be of the form '+' or '-'.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        private void ReadOperator(StringReader reader, char token)
        {
            if (operatorStack.Count == 0)
            {
                operatorStack.Push(token.ToString());
                reader.Read();
            }
            else
            {
                // first, we need to move higher precedence operators from stack to queue 
                MoveOperatorsFromStackToQueue(token);
                operatorStack.Push(token.ToString());
                reader.Read();
            }
        }

        /// <summary>
        /// Move operators which have higher precedence than this token from the stack to the queue
        /// </summary>
        /// <param name="token"></param>
        private void MoveOperatorsFromStackToQueue(char token)
        {
            while (operatorStack.Count > 0 && Helpers.IsOperator(operatorStack.Peek()))
            {

                var associativity = Helpers.GetOperatorAssociativity(token);
                var tokenPrecedence = Helpers.GetOperatorPrecedence(token);
                var stackTopPrecedence = Helpers.GetOperatorPrecedence(operatorStack.Peek()[0]);
                if ((associativity == OperatorAssociativity.LEFT && tokenPrecedence <= stackTopPrecedence) ||
                   (associativity == OperatorAssociativity.RIGHT && tokenPrecedence < stackTopPrecedence))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                else
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Move all operators from the operator stack to output queue.
        /// </summary>
        private void MoveOperatorsFromStackToQueue()
        {
            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")")
                    throw new Exception("Mismatched parenthesis...");

                outputQueue.Enqueue(operatorStack.Pop());
            }
        }

        /// <summary>
        /// Moves operators from stack to queue until an open parenthesis is encountered.
        /// </summary>
        private void MoveOperatorsFromStackUntilOpenParenthesis()
        {
            while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }
            operatorStack.Pop();
        }
    }
}
