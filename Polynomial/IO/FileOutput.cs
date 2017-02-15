using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polynomial
{
    public class FileOutput : IOutputMedium
    {
        public string FilePath { get; set; }
        public FileOutput(string filePath)
        {
            FilePath = filePath;
        }

        public bool Write(string text)
        {
            try
            {
                File.AppendAllText(FilePath, text);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
