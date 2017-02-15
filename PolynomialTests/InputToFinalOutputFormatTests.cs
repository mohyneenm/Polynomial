using NUnit.Framework;
using Polynomial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolynomialTests
{
    class InputToFinalOutputFormatTests
    {
        private ExpressionConverter expressionConverter;
        private Tokenizer tokenizer;
        private PolynomialParser parser;
        private Sorter<PolynomialNode<double>> sorter;

        [SetUp]
        public void Setup()
        {
            expressionConverter = new ExpressionConverter();
            tokenizer = new Tokenizer();
            parser = new PolynomialParser(tokenizer);
            sorter = new Sorter<PolynomialNode<double>>();
        }
        [TearDown]
        public void Teardown()
        {
            expressionConverter = null;
            tokenizer = null;
            parser = null;
        }

        [TestCase("",           ExpectedResult = "0 = 0")]
        [TestCase("x = y",      ExpectedResult = "x - y = 0")]
        [TestCase("x = 10",     ExpectedResult = "x - 10 = 0")]
        [TestCase("-10 = x",    ExpectedResult = "- x - 10 = 0")]
        [TestCase("10x = 10.5", ExpectedResult = "10x - 10.5 = 0")]
        [TestCase("x+y=0",      ExpectedResult = "x + y = 0")]
        public string InputToFinalOutput_WithInfixEdgeCases_ShouldReturnCorrectOutput(string input)
        {
            // Arrange
            var postfix = expressionConverter.InfixToPostfix(input);
            var outputExpression = parser.SimplifyExpression(postfix);
            sorter.Sort(outputExpression);

            // Act
            var output = Helpers.GenerateOutputFormat(outputExpression);

            // Assert
            return output;
        }

        [TestCase("x + y - x^2 = y^2 - xy",             ExpectedResult = "- x^2 - y^2 + x + y + xy = 0")]
        [TestCase("x^2 + 3xy + y^2 = x^2 + xy",         ExpectedResult = "y^2 + 2xy = 0")]
        [TestCase("3x^2 + 3.5xy - y^2 - 1.3zyx = 2.2xyz",ExpectedResult = "3x^2 - y^2 + 3.5xy - 3.5zyx = 0")]
        [TestCase("-15y^2 + 13yx - x^2 = 2x^2 + xy",    ExpectedResult = "- 3x^2 - 15y^2 + 12yx = 0")]
        [TestCase("0 = -12.7x^2 + xy",                  ExpectedResult = "12.7x^2 - xy = 0")]
        [TestCase("-1000.55x^2 + 100.66xy = y^2",       ExpectedResult = "- 1000.55x^2 - y^2 + 100.66xy = 0")]
        public string InputToFinalOutput_WithComplexInfixExpressions_ShouldReturnCorrectOutput(string input)
        {
            // Arrange
            var postfix = expressionConverter.InfixToPostfix(input);
            var outputExpression = parser.SimplifyExpression(postfix);
            sorter.Sort(outputExpression);

            // Act
            var output = Helpers.GenerateOutputFormat(outputExpression);

            // Assert
            return output;
        }

        [TestCase("(10) = x",                               ExpectedResult = "- x + 10 = 0")]
        [TestCase("x + y - (x^2 + y^2) - xy = 0",           ExpectedResult = "- x^2 - y^2 + x + y - xy = 0")]
        [TestCase("x^3 + 3xy + y^2 = (x^2 + xy)",           ExpectedResult = "x^3 - x^2 + y^2 + 2xy = 0")]
        [TestCase("-15y^2 + (13yx - x^2) = (2x^2) + xy",    ExpectedResult = "- 3x^2 - 15y^2 + 12yx = 0")]
        [TestCase("(x^2 + 3xy + y^2) = (x^2 + xy)",         ExpectedResult = "y^2 + 2xy = 0")]
        [TestCase("x^2 - (3xy + y^2) = -(x + y - ((x^2 - 3x^2 + y^2) - xy))", ExpectedResult = "3x^2 - 2y^2 - 2xy + x + y = 0")]
        public string InputToFinalOutput_WithParenthesizedInfixExpressions_ShouldReturnCorrectOutput(string input)
        {
            // Arrange
            var postfix = expressionConverter.InfixToPostfix(input);
            var outputExpression = parser.SimplifyExpression(postfix);
            sorter.Sort(outputExpression);

            // Act
            var output = Helpers.GenerateOutputFormat(outputExpression);

            // Assert
            return output;
        }
    }
}
