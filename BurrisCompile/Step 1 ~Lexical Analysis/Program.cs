namespace BurrisCompile
{
    internal class Program
    {
        private int[,] FSA = new int[31, 18];
        static void Main(string[] args)
        {
            string FSATokensFile = "fsatext1.txt";
            string FSASymbolsFile = "fsatext2.txt";
            FSAScanner scanner = new FSAScanner(FSATokensFile, FSASymbolsFile);
            string sc = scanner.ToString();
            scanner.mwamwa();
            OPG food = new OPG();
            food.Whatwegonnado();
        }

    }


}
