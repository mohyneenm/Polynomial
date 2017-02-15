using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class ConsoleInput : IInputMedium
    {
        public string[] Read()
        {
            var input = Console.ReadLine();
            return new string[] { input };
        }
    }
}
