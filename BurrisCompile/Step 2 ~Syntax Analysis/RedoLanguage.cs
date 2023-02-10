using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurrisCompile.Step_2__Syntax_Analysis
{
    internal class RedoLanguage
    {
        private string TextFileRead;
        private string TextFileWritten;
        public RedoLanguage()
        {
            TextFileRead = "Language.txt";
            TextFileWritten = "Language2.txt";
        }
        public void redo()
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFileRead);
            using StreamWriter sw = new StreamWriter(TextFileWritten);

            using (StreamReader sr = File.OpenText(fileName))
            {

                string text;
                string[] lines1;
                string[] lines2;

                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines1 = text.Trim().Split("::=", StringSplitOptions.RemoveEmptyEntries);
                    lines2 = lines1[1].Trim().Split("|", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in lines2)
                    {
                        sw.WriteLine(lines1[0] + " - " + item);

                    }
                }
            }
        }

    }
}
