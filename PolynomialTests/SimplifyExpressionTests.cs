using FluentAssertions;
using NUnit.Framework;
using Polynomial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolynomialTests
{
    class SimplifyExpressionTests
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

        [TestCase("", ExpectedResult = "")]
        public string SimplifyExpression_WithEmptyString_ShouldReturnEmptyList(string input)
        {
            // Act
            var val = parser.SimplifyExpression(input);

            // Assert
            val.Should().NotBeNull();
            val.Should().HaveCount(0);
            return "";
        }

        [TestCase("x",      ExpectedResult = "1x^1")]
        [TestCase("10",     ExpectedResult = "10")]
        [TestCase("0",      ExpectedResult = "0")]
        [TestCase("10x",    ExpectedResult = "10x^1")]
        [TestCase("0 10 -", ExpectedResult = "-10")]
        public string SimplifyExpression_PostfixWithOneNode_ShouldReturnCorrectInfix(string input)
        {
            // Act
            var val = parser.SimplifyExpression(input);

            // Assert
            val.Should().NotBeNull();
            val.Should().HaveCount(1);
            return val[0].ToString();
        }

        
        [TestCase("x y +",                  ExpectedResult = "1x^1 1y^1")]
        [TestCase("0 0 12.7x^2 - xy + -",   ExpectedResult = "12.7x^2 -1x^1y^1")]
        public string SimplifyExpression_PostfixWithTwoNodes_ShouldReturnCorrectInfix(string input)
        {
            // Act
            var val = parser.SimplifyExpression(input);
            
            // Assert
            val.Should().NotBeNull();
            val.Should().HaveCount(2);
            return $"{val[0].ToString()} {val[1].ToString()}";
        }

        [TestCase("x y + x^2 - y^2 + xy -", ExpectedResult = "1x^1 1y^1 -1x^2 1y^2 -1x^1y^1")]
        public string SimplifyExpression_PostfixWithFiveNodes_ShouldReturnCorrectInfix(string input)
        {
            // Act
            var val = parser.SimplifyExpression(input);
            
            // Assert
            val.Should().NotBeNull();
            val.Should().HaveCount(5);
            return $"{val[0].ToString()} {val[1].ToString()} {val[2].ToString()} {val[3].ToString()} {val[4].ToString()}";
        }
    }
}
