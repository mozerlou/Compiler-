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
            String FSATokensFile = "fsatext1.txt";
            String FSASymbolsFile = "fsatext2.txt";
            FSAScanner scanner = new FSAScanner(FSATokensFile, FSASymbolsFile);
            /*FSAScanner scanner = new FSAScanner(FSATokensFile, FSASymbolsFile);
            String sc = scanner.ToString();
            Console.WriteLine(sc);*/
            //   Console.WriteLine(scanner.ToString());
            scanner.mwamwa();
           //scanner.ReadChar();
            //Console.WriteLine(str);
        }

    }


}
