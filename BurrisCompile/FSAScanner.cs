using System.Text;
using System.Text.RegularExpressions;

namespace BurrisCompile
{
    public class FSAScanner
    {
        private int[,] FSA;
        private int[,] FSA2;

        private int[,] CharacterList;
        private String[] TokenList;
        private int row { get; set; }
        private int col { get; set; }
        private int tokens { get; set; }

        private int[,] Warshall;


        public FSAScanner(String textfile, String textfile2)
        {
            DetermineTableSize(textfile);
            FSA = new int[row, col];//31,18
            ReadTable1(textfile);

            DetermineTableSize(textfile2);
            FSA2 = new int[row, col];
            ReadTable2(textfile2);
        }


        private void Warshalls()
        {
            Console.WriteLine(row + " " + col);
            DetermineTableSize("warsh.txt");
            Console.WriteLine(row + " " + col);

            Warshall = new int[row, col];
            ReadWarshall("warsh.txt");
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    if (Warshall[j, i] == 1)
                    {
                        for (int k = 0; k < row; k++)
                        {
                            if (Warshall[j,k]!=1 && Warshall[i, k]==1)
                            {
                                Warshall[j,k] = 1; 
                            }
                        }
                    }
                }
            }


            string str = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    str += Warshall[i, j] + " ";
                }
                str += "\n";
            }
            Console.WriteLine(str);
        }
        private void ReadWarshall(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text = sr.ReadLine();
                string[] lines;


                for (int i = 0; i < row; i++)
                {
                    text = sr.ReadLine();
                    lines = text.Split('\t');

                    if (int.TryParse(lines[2], out int f))
                    {
                        for (int j = 0; j < lines.Length - 1; j++)
                        {
                            if (int.TryParse(lines[j + 1], out int mf)) Warshall[i, j] = int.Parse(lines[j + 1]); //in case user adds extra whitespace                            
                        }
                    }
                }
            }
        }

        //determines the FSA Table size based on file input
        public void DetermineTableSize(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                //ADD SAFETY
                string text = sr.ReadLine();

                //determines col length
                string[] lines = text.Split('\t');
                this.col = lines.Length - 1;

                //determines row length
                int count = 0;
                while ((text = sr.ReadLine()) != null)
                {
                    count++;
                }
                this.row = count;
            }
        }

        //reads 1st FSA table(aka the token/class) and transfers it to array 
        private void ReadTable1(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text = sr.ReadLine();
                string[] lines;

                for (int i = 0; i < row; i++)
                {
                    text = sr.ReadLine();
                    lines = text.Split('\t');

                    if (int.TryParse(lines[2], out int f))
                    {
                        for (int j = 0; j < lines.Length - 1; j++)
                        {
                            if (int.TryParse(lines[j + 1], out int mf)) FSA[i, j] = int.Parse(lines[j + 1]); //in case user adds extra whitespace                            
                        }
                    }
                }
            }
        }

        //reads 2nd FSA table(aka the symbol table) and transfers it to array 
        private void ReadTable2(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text = sr.ReadLine();
                string[] lines;


                for (int i = 0; i < row; i++)
                {
                    text = sr.ReadLine();
                    lines = text.Split('\t');

                    if (int.TryParse(lines[2], out int f))
                    {
                        for (int j = 0; j < lines.Length - 1; j++)
                        {
                            if (int.TryParse(lines[j + 1], out int mf)) FSA2[i, j] = int.Parse(lines[j + 1]); //in case user adds extra whitespace                            
                        }
                    }
                }
            }
        }
        //fluff
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    str += FSA2[i, j] + " ";
                }
                str += "\n";
            }
            return str;
        }



        //Populates Characterlist array with all the corresponding collumns of each char from sampleprogram  
        //Needs to be switched to a method call that returns a single character for larger programs, will do that later
        //Column based array (because its faster than rows)- have two rows- row 1 contains the character
        //                                                                  row 2 contains the matching collumn
        private void ReadChar()
        {
            //opens files directory- allows for easier transfer
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\SampleProgram.txt");

            //>need to find better way to determine length, try string seperation
            StreamReader sf = File.OpenText(fileName);
            int filelength = sf.ReadToEnd().Length;
            CharacterList = new int[2, filelength + 2];
            sf.Close();

            using (StreamReader sr = File.OpenText(fileName))
            {
                //goes through each file char one by one, returns an ascii value, feeds it to determinechar methods
                for (int i = 0; i < filelength; i++)
                {
                    int m = sr.Read();
                    CharacterList[0, i] = m;
                    CharacterList[1, i] = DetermineChar(m);
                }
                //adds spaces at the end of file, cheat-cheat but I will have to change this method anyways
                CharacterList[0, filelength] = 32;
                CharacterList[1, filelength] = 17;
                CharacterList[0, filelength + 1] = 32;
                CharacterList[1, filelength + 1] = 17;
            }
        }

        //Returns corresponding column based on a char
        private int DetermineChar(int ascii)
        {
            //0-L,1-D,2-*,3=/,4==,5=<,6=>,7=!,8=.,9=(,10=),11={,12=},13=,,14=;,15=-,16=+,17=b,18=other

            int i = 0;
            switch (ascii)
            {
                case int n when (ascii > 64 && ascii < 91 || ascii > 96 && ascii < 123): i = 0; break;
                case int n when (ascii > 47 && ascii < 58 || ascii > 96 && ascii < 123): i = 1; break;
                case 42: i = 2; break;
                case 47: i = 3; break;
                case 61: i = 4; break;
                case 60: i = 5; break;
                case 62: i = 6; break;
                case 33: i = 7; break;
                case 46: i = 8; break;
                case 40: i = 9; break;
                case 41: i = 10; break;
                case 123: i = 11; break;
                case 125: i = 12; break;
                case 44: i = 13; break;
                case 59: i = 14; break;
                case 45: i = 15; break;
                case 43: i = 16; break;
                case 32: case 13: case 10: i = 17; break;
                default: i = 18; break;
            }
            return i;
        }

        private void ReadToken()
        {
            //opens files directory- allows for easier transfer
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\TokenClass.txt");

            //whats going to seperate strings in our text file, aka all the whitespace
            string[] stringSeparators = new string[] { "\n","\t", " " };

            using (StreamReader sr = File.OpenText(fileName))
            {
                sr.ReadLine();
                TokenList = sr.ReadToEnd().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);                 
            }
            //this will switch all tokens in the classification collumn to its corresponding table value
            for (int i = 1; i < tokens*2;i=i+2)
            { 
                switch (TokenList[i].Trim())
                {
                   
                    case "$CLASS": TokenList[i] = "0"; break;
                    case "var": case "$ODD": case "$DO": case "$IF": case "$CALL": case "$WHILE": TokenList[i] = "1"; break;
                    case "LB": TokenList[i] = "2"; break;
                    case "$CONST": TokenList[i] = "3"; break;
                    case "assign": TokenList[i] = "4"; break;
                    case "NumLit": TokenList[i] = "5"; break;
                    case "semi": TokenList[i] = "6"; break;
                    case "$VAR": TokenList[i] = "7"; break;
                    case "Comma": TokenList[i] = "8"; break;
                    case "$PROCEDURE": TokenList[i] = "10"; break;
                    default: TokenList[i] = "9"; break;
                }   
            }

        }
        public void mwamwa()
        {

            ReadChar();            
            ProcessTokens();
            ReadToken();
            ProcessSymbolTable();



            Warshalls();
        }

        //will create a partial token/class - still needs to go over var variables
        private void ProcessTokens()
        {

            int nextState = 0;
            int i = 0;
            String str = "";
            var builder = new StringBuilder();
            int len = CharacterList.Length / 2;
            this.tokens = 0;
            String file = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\TokenClass.txt");
            using StreamWriter sw = new StreamWriter(file);
            sw.WriteLine("Token\t\tClassification"); 

            while (i < len)
            {
                switch (nextState)
                {
                    case 0: nextState = FSA[0, CharacterList[1, i]]; i++; break;
                    case 1: nextState = 0; sw.WriteLine("error occured here"); break;
                    case 2: nextState = 0; sw.WriteLine("*\t\tmop"); tokens++; break;
                    case 3: nextState = FSA[3, CharacterList[1, i]]; i--; builder.Append((char)CharacterList[0, i]); i++; i++; break;
                    case 4: nextState = 0; sw.WriteLine(builder.ToString() + "\t\tNumLit"); builder.Clear(); i--; tokens++; break;
                    case 5: nextState = FSA[5, CharacterList[1, i]]; i++; break;
                    case 6: nextState = FSA[6, CharacterList[1, i]]; i--; builder.Append((char)CharacterList[0, i]); i++; i++; break;
                    case 7:
                        nextState = 0; str = builder.ToString(); builder.Clear();
                        switch (str)
                        {
                            case "CONST": sw.WriteLine(str + "\t\t$CONST"); break;
                            case "IF": sw.WriteLine(str + "\t\t$IF"); break;
                            case "THEN": sw.WriteLine(str + "\t\t$CONST"); break;
                            case "VAR": sw.WriteLine(str + "\t\t$VAR"); break;
                            case "WHILE": sw.WriteLine(str + "\t\t$WHILE"); break;
                            case "CALL": sw.WriteLine(str + "\t\t$CALL"); break;
                            case "DO": sw.WriteLine(str + "\t\t$DO"); break;
                            case "ODD": sw.WriteLine(str + "\t\t$ODD"); break;
                            case "CLASS": sw.WriteLine(str + "\t\t$CLASS"); break;
                            case "PROCEDURE": sw.WriteLine(str + "\t\t$PROCEDURE"); break;
                            default:
                                sw.WriteLine(str + "\t\tvar");
                                break;
                        }
                        i--; tokens++; break;
                    case 8: nextState = FSA[8, CharacterList[1, i]]; i++; break;
                    case 9: nextState = FSA[9, CharacterList[1, i]]; i++; break;
                    case 10: nextState = FSA[10, CharacterList[1, i]]; break;
                    case 11: nextState = 0; sw.WriteLine("/\t\tdop"); tokens++; break;
                    case 12: nextState = FSA[12, CharacterList[1, i]]; i++; break;
                    case 13: nextState = 0; i--; sw.WriteLine("<=\t\trelop"); tokens++; break;
                    case 14: nextState = 0; i--; sw.WriteLine("<\t\trelop"); tokens++; break;
                    case 15: nextState = FSA[15, CharacterList[1, i]]; i++; break;
                    case 16: nextState = 0; i--; sw.WriteLine(">=\t\trelop"); tokens++; break;
                    case 17: nextState = 0; i--; sw.WriteLine(">\t\trelop"); tokens++; break;
                    case 18: nextState = FSA[18, CharacterList[1, i]]; i++; break;
                    case 19: nextState = 0; i--; sw.WriteLine("=\t\tassign"); tokens++; break;
                    case 20: nextState = 0; i--; sw.WriteLine("==\t\trelop"); tokens++; break;
                    case 21: nextState = FSA[21, CharacterList[1, i]]; i++; break;
                    case 22: nextState = 0; sw.WriteLine("!=\t\trelop"); tokens++; break;
                    case 23: nextState = 0; sw.WriteLine("}\t\tRB"); tokens++; break;
                    case 24: nextState = 0; sw.WriteLine("{\t\tLB"); tokens++; break;
                    case 25: nextState = 0; sw.WriteLine(",\t\tComma"); tokens++; break;
                    case 26: nextState = 0; sw.WriteLine(";\t\tsemi"); tokens++; break;
                    case 27: nextState = 0; sw.WriteLine("-\t\tsop"); tokens++; break;
                    case 28: nextState = 0; sw.WriteLine("+\t\taddop"); tokens++; break;
                    case 29: nextState = 0; sw.WriteLine("(\t\tLP"); tokens++; break;
                    case 30: nextState = 0; sw.WriteLine(")\t\tRP"); tokens++; break;
                    default: nextState = 1; break;
                }
            }
            sw.Close();
        }

        private string[] GetTokenList()
        {
            return TokenList;
        }

        private void ProcessSymbolTable()
        {
            int nextState = 0;
            int i = 1;

            String file = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\SymbolTable.txt");
            using StreamWriter sw = new StreamWriter(file);

            sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", "Token",	"Class", "Value","Address","Segment");


            int DSadd = 0;
            int CSadd = 0;

            //We assume two bytes per instruction and data item.
            //DS-Area to hold the information which the program modifies. CS-Area to hold the user defined instructions to modify the information in the data area.
         
            while (i < tokens*2)
            {          
                switch (nextState)
                {
                    case 0: nextState = FSA2[0, Int32.Parse(TokenList[i])]; i=i+2;  break;
                    case 1: nextState = FSA2[1, Int32.Parse(TokenList[i])]; sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", $"{TokenList[i - 1]}", "PGM NAME", "", $"{DSadd}", "DS"); DSadd=DSadd+2; i=i+2; break;
                    case 2: nextState = FSA2[2, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 3: nextState = FSA2[3, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 4: nextState = FSA2[4, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 5: nextState = FSA2[5, Int32.Parse(TokenList[i])]; i =i+2; break;
                    case 6: nextState = FSA2[6, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 7: nextState = FSA2[7, Int32.Parse(TokenList[i])]; sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", $"{TokenList[i - 7]}", "CONST", $"{TokenList[i - 3]}", $"{CSadd}", "CS"); CSadd=CSadd+2; i =i+2; break;
                    case 8: nextState = FSA2[8, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 9: nextState = FSA2[9, Int32.Parse(TokenList[i])]; sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", $"{TokenList[i - 3]}", "VAR", "", $"{CSadd}", "CS"); CSadd=CSadd+2; i =i+2; break;
                    case 10: nextState = FSA2[10, Int32.Parse(TokenList[i])]; i=i+2; break;
                    case 11: nextState = FSA2[11, Int32.Parse(TokenList[i])]; sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", $"{TokenList[i - 3]}", "NUMLIT", $"{TokenList[i - 3]}", $"{CSadd}", "CS");CSadd=CSadd+2; i =i+2; break;
                    case 12: throw (new Exception("Invalid Format, another token was expected. Currently processing token: "+ (TokenList[i - 2])));  break;
                    case 13: nextState = FSA2[13, Int32.Parse(TokenList[i])]; sw.WriteLine("{0,-30}{1,-10}{2,-7}{3,-10}{4,-10}", $"{TokenList[i - 1]}", "PROCEDURE", "", $"{DSadd}", "DS"); DSadd=DSadd+2; Console.WriteLine(nextState); i =i+2;break;
                    case 14: nextState = FSA2[14, Int32.Parse(TokenList[i])]; i=i+2;  break;
                }
            }
            sw.Close();
        }

    

    
    }
        

    
}

