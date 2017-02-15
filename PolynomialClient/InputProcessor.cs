using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    /// <summary>
    /// This is the client where dependencies can be switched with alternative implementations.
    /// </summary>
    public class InputProcessor
    {
        private IExpressionConverter expressionConverter;
        private ITokenizer tokenizer;
        private IParser parser;
        private ISorter<PolynomialNode<double>> sorter;

        public InputProcessor()
        {
            expressionConverter = new SimpleExpressionConverter();
            tokenizer = new SimpleTokenizer();
            parser = new PolynomialParser(tokenizer);
            sorter = new Sorter<PolynomialNode<double>>();
        }

        public void ProcessConsoleInput()
        {
            Console.WriteLine("(Press CTRL+C to quit)\n");
            Console.WriteLine("Enter polynomial expression to convert into canonical form:\n");

            var inputMedium = new ConsoleInput();
            while (true)
            {
                // input
                var inputExpression = inputMedium.Read();
                if (inputExpression?.Length == 0 || string.IsNullOrWhiteSpace(inputExpression?[0]))
                    continue;

                // parse
                var postfix = expressionConverter.InfixToPostfix(inputExpression[0]);
                var outputExpression = parser.SimplifyExpression(postfix).ToList();

                // sort
                sorter.Sort(outputExpression);

                // output   
                var outputMedium = new ConsoleOutput();
                var expression = Helpers.GenerateOutputFormat(outputExpression);
                outputMedium.Write(expression);
            }
        }

        public void ProcessFileInput()
        {
            Console.Write("Enter file path: ");
            var inputPath = Console.ReadLine();
            var outputPath = Path.ChangeExtension(inputPath, "out");
            var inputMedium = new FileInput(inputPath);
            var linesFromFile = inputMedium.Read();
            var outputMedium = new FileOutput(outputPath);
            var expression = string.Empty;

            foreach (var line in linesFromFile)
            {
                // input
                var inputExpression = line;

                // parse
                var postfixExpression = expressionConverter.InfixToPostfix(inputExpression);
                var simplifiedExpression = parser.SimplifyExpression(postfixExpression).ToList();

                // sort
                sorter.Sort(simplifiedExpression);

                expression += Helpers.GenerateOutputFormat(simplifiedExpression) + Environment.NewLine;
            }

            // output
            bool success = outputMedium.Write(expression);
            if (success)
                Console.WriteLine($"\nProcessing Complete.\nResults have been written to \"{outputPath}\"\n");
            else
                Console.WriteLine($"Exception occurred: results could not be written to \"{outputPath}\"\n");
        }
    }
}
