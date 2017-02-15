using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class ConsoleOutput : IOutputMedium
    {
        public bool Write(string text)
        {
            Console.WriteLine(text);
            return true;
        }
    }
}
