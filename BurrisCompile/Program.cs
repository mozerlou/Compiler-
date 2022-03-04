using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurrisCompile
{
    internal class Program
    {
        private int[,] FSA = new int[31, 18];
        static void Main(string[] args)
        {
            String FSATableTextFile = "fsatext2.txt";
            FSAScanner scanner = new FSAScanner(FSATableTextFile);
            Console.WriteLine(scanner.ReadProgram);
        }

    }


}
