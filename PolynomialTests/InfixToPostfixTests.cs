using System;
using Polynomial;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;

namespace PolynomialSimplifierTests
{
    [TestFixture]
    public class PolynomialParserTests
    {
        private ExpressionConverter expressionConverter;
        private Tokenizer tokenizer;
        private PolynomialParser parser;

        [SetUp]
        public void Setup()
        {
            expressionConverter = new ExpressionConverter();
            tokenizer = new Tokenizer();
            parser = new PolynomialParser(tokenizer);
        }
        [TearDown]
        public void Teardown()
        {
            expressionConverter = null;
            tokenizer = null;
            parser = null;
        }

        [TestCase("",       ExpectedResult = "")]
        [TestCase("x",      ExpectedResult = "x")]
        [TestCase("10",     ExpectedResult = "10")]
        [TestCase("-10",    ExpectedResult = "0 10 -")]
        [TestCase("0",      ExpectedResult = "0")]
        [TestCase("10x",    ExpectedResult = "10x")]
        [TestCase("x+y",    ExpectedResult = "x y +")]
        public string InfixToPostfix_WithInfixEdgeCases_ShouldReturnCorrectPostfix(string input)
        {
            // Act
            return expressionConverter.InfixToPostfix(input);
        }

        [TestCase("x + y - x^2 + y^2 - xy",             ExpectedResult = "x y + x^2 - y^2 + xy -")]
        [TestCase("x^2 + 3xy + y^2 = x^2 + xy",         ExpectedResult = "x^2 3xy + y^2 + x^2 xy + -")]
        [TestCase("3x^2 + 3.5xy - y^2 = 0",             ExpectedResult = "3x^2 3.5xy + y^2 - 0 -")]
        [TestCase("-15y^2 + 13yx - x^2 = 2x^2 + xy",    ExpectedResult = "0 15y^2 - 13yx + x^2 - 2x^2 xy + -")]
        [TestCase("0 = -12.7x^2 + xy",                  ExpectedResult = "0 0 12.7x^2 - xy + -")]
        public string InfixToPostfix_WithComplexInfixExpressions_ShouldReturnCorrectPostfix(string input)
        {
            // Act
            return expressionConverter.InfixToPostfix(input);
        }

        [TestCase("(10)",                                   ExpectedResult = "10")]
        [TestCase("x + y - (x^2 + y^2) - xy",               ExpectedResult = "x y + x^2 y^2 + - xy -")]
        [TestCase("x^3 + 3xy + y^2 = (x^2 + xy)",           ExpectedResult = "x^3 3xy + y^2 + x^2 xy + -")]
        [TestCase("-15y^2 + (13yx - x^2) = (2x^2) + xy",    ExpectedResult = "0 15y^2 - 13yx x^2 - + 2x^2 xy + -")]
        [TestCase("(x^2 + 3xy + y^2) = (x^2 + xy)",         ExpectedResult = "x^2 3xy + y^2 + x^2 xy + -")]
        [TestCase("x^2 - (3xy + y^2) = -(x + y - ((x^2 - 3x^2 + y^2) - xy))", ExpectedResult = "x^2 3xy y^2 + - 0 x y + x^2 3x^2 - y^2 + xy - - - -")]
        public string InfixToPostfix_WithParenthesizedInfixExpressions_ShouldReturnCorrectPostfix(string input)
        {
            // Act
            return expressionConverter.InfixToPostfix(input);
        }
    }
}
