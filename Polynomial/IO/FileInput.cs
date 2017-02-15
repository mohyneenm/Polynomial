using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class FileInput : IInputMedium
    {
        public string FilePath { get; set; }
        public FileInput(string filePath)
        {
            FilePath = filePath;
        }

        public string[] Read()
        {
            var lines = File.ReadAllLines(FilePath);
            return lines;
        }
    }
}
