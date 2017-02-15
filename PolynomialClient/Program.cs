using System;
using System.IO;

namespace Polynomial
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputProcessor = new InputProcessor();

            while (true)
            {
                Console.Write("Enter input mode: 'i' for interactive, 'f' for file: ");
                var mode = Console.ReadLine();

                try
                {
                    if (mode == "i")
                        inputProcessor.ProcessConsoleInput();
                    else if (mode == "f")
                        inputProcessor.ProcessFileInput();
                }
                catch(Exception e)
                {
                    if (Console.OpenStandardInput(1) != Stream.Null)
                        Console.WriteLine($"{e.Message}\n");
                    else
                        break;
                }
            }
        }
    }
}
