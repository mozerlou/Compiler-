namespace BurrisCompile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string FSATokensFile = "fsatext1.txt";
            string FSASymbolsFile = "fsatext2.txt";

            FSAScanner scanner = new FSAScanner(FSATokensFile, FSASymbolsFile);           
            scanner.mwamwa();

            OPG food = new OPG();
            food.Whatwegonnado();

/*
            Step_2__Syntax_Analysis.Syntax lalala = new Step_2__Syntax_Analysis.Syntax();
            lalala.aiai();
*/


            using (StreamWriter log_out = new StreamWriter("quads.txt"))
            {
                try
                {
                    // Redirect standard out to logfile.txt.
                    Console.SetOut(log_out);
                    Step_2__Syntax_Analysis.Syntax lalala = new Step_2__Syntax_Analysis.Syntax();
                    lalala.aiai();
                }
                catch (IOException exc)
                {
                    Console.WriteLine("I/O Error\n" + exc.Message);
                }

            }

            using (StreamWriter log_out = new StreamWriter("Assembly.txt"))
            {
                try
                {
                    // Redirect standard out to logfile.txt.
                    Console.SetOut(log_out);
                    Step_4__Code_Generation.Generate jokesonyou = new Step_4__Code_Generation.Generate();
                    jokesonyou.Run();
                }
                catch (IOException exc)
                {
                    Console.WriteLine("I/O Error\n" + exc.Message);
                }

            }


        }


        
    }


}
