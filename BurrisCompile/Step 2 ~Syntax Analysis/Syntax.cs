using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurrisCompile.Step_2__Syntax_Analysis
{
    internal class Syntax
    {
        private string [,] OPGtable;

        OPG opgo = new OPG();

        List<string> stack = new List<string>();
        List<int> precpointer = new List<int>();
        List<int> fixup = new List<int>();

        int label = 1;
        int tmpnum = 1;
        int WhileLabel = 1;

        private int[,] PDATable;
        private int row { get; set; }
        private int col { get; set; }

        public void aiai()
        {
            DetermineTableSize("PDATable.txt");
            PDATable = new int[row, col];//31,18
            ReadTable1("PDATable.txt");
            PDA("TokenClass.txt");

            // opgo.Whatwegonnado();
            OPGtable = opgo.ReadOPGTableTXT();

         

           // +opgo.FindOPGCell(stack[stack.Count - 1], stack[stack.Count - 2])
           // ReadTokenClass("TokenClassTemp.txt");
            //PrintList();
            
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
                            if (int.TryParse(lines[j + 1], out int mf)) PDATable[i, j] = int.Parse(lines[j + 1]); //in case user adds extra whitespace                            
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
                    str += PDATable[i, j] + " ";
                }

                str += "\nline\n";
            }
            return str;
        }

        //returns the PDA corresponding collumn base on the token classification fed into it
        private int ReadToken(string text)
        {
            int col;
                switch (text)
                {
                    case "@CONST": col = 0; break;
                    case "@VAR": col = 1; break;
                    //default any
                    case "@PROCEDURE": col = 3; break;
                    case "var": col = 4; break;
                    case "@LP": col = 5; break;
                    case "@RP": col = 6; break;
                    case "@LB": col = 7; break;
                    case "@IF": col = 8; break;
                    case "@WHILE": col = 9; break;
                    case "@ODD": col = 10; break;
                    case "@relop": col = 11; break;
                    case "@THEN": col = 12; break;
                    case "@DO": col = 13; break;
                    //POPPED
                    case "@CALL": col = 15; break;
                    case "@semi": col = 16; break;
                    case "NumLit": col = 17; break;
                    case "@assign": col = 18; break;
                    case "@addop": col = 19; break;
                    case "@sop": col = 20; break;
                    case "@mop": col = 21; break;
                    case "@dop": col = 22; break;
                    case "@Comma": col = 23; break;
                    case "@COUT": col = 24; break;
                    case "@CIN": col = 25; break;
                default: col = 2; break;
                } 
            return col;
        }

       

        private void PDA(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {              
                int nextState = 0;
                int previState = 0;
                string token;
                string procedure = "";
                string[] stringSeparators = new string[] { "\n", "\t", " " };
               
                string text = sr.ReadLine(); //ignoring first line
                       text = sr.ReadLine();
                string[] lines = text.Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                while (sr.Peek() >= 0)
                {
                    previState = nextState;
                    token = lines[1].Substring(1);
                     switch (nextState)
                    {
                        case 0: nextState = PDATable[0, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 1: nextState = PDATable[1, ReadToken(lines[1])]; if (nextState!=21) lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 2: nextState = PDATable[2, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 3: Console.WriteLine("PROCEDURE " + lines[0]); procedure = lines[0]; nextState = PDATable[3, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 4: nextState = PDATable[4, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 5:  nextState = PDATable[5, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 6: Stack(lines[0], lines[1]);  nextState = PDATable[6, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 7:  if (stack.Count==0) { nextState = PDATable[7, 14]; Console.WriteLine("END OF PROCEDURE " ); tmpnum = 1; } else { if (lines[1] != "@VAR" && lines[1] != "@CONST") Stack(lines[0], lines[1]);  if (stack.Count != 0) { nextState = PDATable[7, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); } }  break;
                        case 8: Stack(lines[0], lines[1]); Console.WriteLine("WHILE W" + WhileLabel + "  -  -"); WhileLabel++; nextState = PDATable[8, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 9: Stack(lines[0], lines[1]); nextState = PDATable[9, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 10: Stack(lines[0], lines[1]); Console.WriteLine("DO  L" + label + "  -  -"); fixup.Add(label); label++; nextState = PDATable[10, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 11: Stack(lines[0], lines[1]); Console.WriteLine("IF -  -  -"); nextState = PDATable[11, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 12: Stack(lines[0], lines[1]); nextState = PDATable[12, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 13: Console.WriteLine("THEN L"+ label + "  -  -"); fixup.Add(label); label++;  nextState = PDATable[13, ReadToken(lines[1])]; break;
                        case 14: if (lines[0] == procedure) { Console.WriteLine("RECURSIVE CALL " + lines[0]); } else { Console.WriteLine("CALL " + lines[0]); } stack.RemoveAt(stack.Count - 1); nextState = PDATable[14, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 15: nextState = PDATable[15, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 16: nextState = PDATable[16, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 17: Stack(lines[0], lines[1]); nextState = PDATable[17, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 18: Stack(lines[0], lines[1]); nextState = PDATable[18, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 19: Stack(lines[0], lines[1]); nextState = PDATable[19, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 20: Stack(lines[0], lines[1]); nextState = PDATable[20, 7]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 21: Stack("{","@LB"); nextState = PDATable[21, ReadToken(lines[1])]; Console.WriteLine("START OF MAIN"); break;
                        case 22: nextState = PDATable[22, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 23: if (!stack.Any()) { nextState = PDATable[23, 14]; Console.WriteLine("END OF PROGRAM"); } else { nextState = PDATable[23, ReadToken(lines[1])]; Stack(lines[0], lines[1]); lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); } break;
                        case 24: Stack(lines[0], lines[1]); nextState = PDATable[24, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 25: Stack(lines[0], lines[1]); nextState = PDATable[25, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 26: Stack(lines[0], lines[1]); nextState = PDATable[26, 7]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 27: Stack(lines[0], lines[1]); Console.WriteLine("WHILE W" + WhileLabel + "  -  -"); WhileLabel++; nextState = PDATable[27, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 28: Stack(lines[0], lines[1]); nextState = PDATable[28, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 29: Stack(lines[0], lines[1]); Console.WriteLine("DO  L" + label + "  -  -"); fixup.Add(label); label++; nextState = PDATable[29, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 30: Stack(lines[0], lines[1]); Console.WriteLine("IF -  -  -"); nextState = PDATable[30, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 31: Stack(lines[0], lines[1]); nextState = PDATable[31, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 32: Console.WriteLine("THEN L" + label + "  -  -"); fixup.Add(label); label++; nextState = PDATable[32, ReadToken(lines[1])]; break;
                        case 33: Console.WriteLine("CALL " + lines[0]); stack.RemoveAt(stack.Count - 1); nextState = PDATable[33, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 34: nextState = PDATable[34, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 35: nextState = PDATable[35, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 36: nextState = PDATable[36, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 37: Console.WriteLine("Error");break;
                        case 38: Stack(lines[0], lines[1]); nextState = PDATable[38, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 39: Stack(lines[0], lines[1]); nextState = PDATable[39, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 40: Stack(lines[0], lines[1]); nextState = PDATable[40, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 41: Stack(lines[0], lines[1]); nextState = PDATable[41, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;

                        case 42: Stack(lines[0], lines[1]); nextState = PDATable[42, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 43: Stack(lines[0], lines[1]); nextState = PDATable[43, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 44: Stack(lines[0], lines[1]); nextState = PDATable[44, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 45: Stack(lines[0], lines[1]); nextState = PDATable[45, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 46: Stack(lines[0], lines[1]); nextState = PDATable[46, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 47: Stack(lines[0], lines[1]); nextState = PDATable[47, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 48: Stack(lines[0], lines[1]); nextState = PDATable[48, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                        case 49: Stack(lines[0], lines[1]); nextState = PDATable[49, ReadToken(lines[1])]; lines = sr.ReadLine().Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries); break;
                    }
                    if (nextState==37)
                    {

                        switch (previState)
                        {
                            case 0: Console.WriteLine("Error at token " + token); break;
                            case 1: Console.WriteLine("Error at token " + token); break;
                            case 2: Console.WriteLine("Error at token " + token); break;
                            case 3: Console.WriteLine("Error at token " + token); break;
                            case 4: Console.WriteLine("Error at token " + token); break;
                            case 5: Console.WriteLine("Error at token " + token); break;
                            case 6: Console.WriteLine("Error at token " + token); break;
                            case 7: Console.WriteLine("Error at token " + token); break;
                            case 8: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 9: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 10: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 11: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 12: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 13: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 14: Console.WriteLine("Error at token " + token); Console.WriteLine("We were expecting a var for the call function"); break;
                            case 15: Console.WriteLine("Error at token " + token); Console.WriteLine("We were expecting a bracket"); break;
                            case 16: Console.WriteLine("Error at token " + token); Console.WriteLine("No closing bracket"); break;
                            case 17: Console.WriteLine("Error at token " + token); Console.WriteLine("No semi for call"); break;
                            case 18: Console.WriteLine("Error at token " + token); break;
                            case 19: Console.WriteLine("Error at token " + token); Console.WriteLine("An assignment was expected"); break;
                            case 20: Console.WriteLine("Error at token " + token); break;
                            case 21: Console.WriteLine("Error at token " + token); break;
                            case 22: Console.WriteLine("Error at token " + token); break;
                            case 23: Console.WriteLine("Error at token " + token); break;
                            case 24: Console.WriteLine("Error at token " + token); break;
                            case 25: Console.WriteLine("Error at token " + token); break;
                            case 26: Console.WriteLine("Error at token " + token); break;
                            case 27: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 28: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 29: Console.WriteLine("Error at token " + token); break;
                            case 30: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 31: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 32: Console.WriteLine("Error at token " + token); break;
                            case 33: Console.WriteLine("Error at token " + token); break;
                            case 34: Console.WriteLine("Error at token " + token); break;
                            case 35: Console.WriteLine("Error at token " + token); break;
                            case 36: Console.WriteLine("Error at token " + token); break;
                            case 37: Console.WriteLine("Error at token " + token); break;
                            case 38: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 39: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID B.E. OPERATOR"); break;
                            case 40: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID BE OPERATOR"); break;
                            case 41: Console.WriteLine("Error at token " + token); Console.WriteLine("NOT A VALID BE OPERATOR"); break;
                            case 42: Console.WriteLine("Error at token " + token); break;
                            case 43: Console.WriteLine("Error at token " + token); break;
                            case 44: Console.WriteLine("Error at token " + token); break;
                            case 45: Console.WriteLine("Error at token " + token); break;
                            case 46: Console.WriteLine("Error at token " + token); break;
                            case 47: Console.WriteLine("Error at token " + token); break;
                            case 48: Console.WriteLine("Error at token " + token); break;
                            case 49: Console.WriteLine("Error at token " + token); break;
                            case 50: Console.WriteLine("Error at token " + token); break;
                        }

                        break;
                    }
                }
                Console.WriteLine("FINI");
            }
        }

        private void Stack (string token, string classi)
        {
            bool popped = false;
            string text, temp = "ou";
            string text1 = "";
            bool needstemp = false;
            if (classi.Trim().StartsWith("@"))
            {               
                //if its an operator, add to stack and compare with previous op

                //finds previous operator relation to new operator
                text = stack.FindLast(x => x.StartsWith("@"));
                if (stack.Count != 0) temp = opgo.FindOPGCell(text.Substring(1), token);
                else precpointer.Add(stack.Count);

                //Console.WriteLine("FIRST TEXT " + text + ", TOKEN CURRENT " + token + ", TEMP "+ temp);

                if (temp.Equals("<") || token == "{") { precpointer.Add(stack.Count);  }
                if (temp == ">")
                {
                  /*  Console.WriteLine("BEFORE THE POP " + text);
                    PrintList();*/
                    //POP:
                    switch (text.Substring(1))
                    {
                        case "(": break;
                        case "COUT": Console.WriteLine("COUT " + stack[stack.Count - 1].ToString()); break;
                        case "CIN": Console.WriteLine("CIN " + stack[stack.Count - 1].ToString()); break;
                        case "{":    break;
                        case "DO":   Console.WriteLine("WHILEFIN L" + fixup[fixup.Count - 1]); fixup.RemoveAt(fixup.Count - 1);  break;
                        case "THEN": Console.WriteLine("FIN L" + fixup[fixup.Count-1]);fixup.RemoveAt(fixup.Count-1); break;
                        case "ODD":  Console.WriteLine(text.Substring(1) + " " + stack[stack.Count - 1] + "  -  -"); break;
                        case "*": case "/": case "+": case "-": 
                             
                            
                            Console.WriteLine(text.Substring(1) + " " + stack[precpointer[precpointer.Count - 1] - 1] + " " + stack[stack.Count - 1] + " T" + tmpnum); needstemp = true; break;
                        //case "IF": Console.WriteLine(lines[0] + ", L" + label + ", ?, ?"); label++; break;
                        default: Console.WriteLine(text.Substring(1) + " " + stack[precpointer[precpointer.Count - 1] - 1] + " " + stack[stack.Count - 1] + " ?"); break;
                    }

                    switch (text.Substring(1))
                    {
                        case "(": stack.RemoveAt(stack.Count - 2);
                            break;
                        case "{":
                            popped = true;
                            // Console.WriteLine("REMOVING LEFT BRACKET");
                            stack.RemoveAt(stack.Count - 1);
                            /*for (int i = stack.Count - 1; i >= precpointer[precpointer.Count - 1]; i--)
                            {
                                stack.RemoveAt(i);
                            }*/
                            precpointer.RemoveAt(precpointer.Count - 1);
                            try
                            {
                                text1 = stack.FindLast(x => x.StartsWith("@"));
                            }
                            catch (Exception ex) { text1 = ""; }

                                if (text1 == "@DO")
                            {
                                stack.RemoveAt(stack.Count-1);
                                stack.RemoveAt(stack.Count-1);
                                Console.WriteLine("WHILEFIN L" + fixup[fixup.Count - 1]); fixup.RemoveAt(fixup.Count - 1); 
                               
                                precpointer.RemoveAt(precpointer.Count - 1);

                               
                            }
                            if (text1 == "@THEN")
                            {
                                stack.RemoveAt(stack.Count - 1);
                                stack.RemoveAt(stack.Count - 1);
                                Console.WriteLine("FIN L" + fixup[fixup.Count - 1]); fixup.RemoveAt(fixup.Count - 1); 
                                precpointer.RemoveAt(precpointer.Count - 1);
                            }

                            break;


                        case "ODD": case "THEN": case "DO": 
                            for (int i = stack.Count - 1; i >= precpointer[precpointer.Count - 1]; i--)
                            {
                                stack.RemoveAt(i);
                            }
                            precpointer.RemoveAt(precpointer.Count - 1);
                            break;

                        case "COUT":
                           
                            stack.RemoveAt(stack.Count - 1);
                            stack.RemoveAt(stack.Count - 1);
                            precpointer.RemoveAt(precpointer.Count - 1);
                            break;
                        case "CIN":

                            stack.RemoveAt(stack.Count - 1);
                            stack.RemoveAt(stack.Count - 1);
                            precpointer.RemoveAt(precpointer.Count - 1);
                            break;

                        default:
                            try { stack.RemoveRange(precpointer[precpointer.Count - 1] - 1, stack.Count - 1); }
                            catch (System.ArgumentException)
                            {
                                for (int i = stack.Count - 1; i >= precpointer[precpointer.Count - 1] - 1; i--)
                                {
                                    stack.RemoveAt(i);
                                }
                            }
                            if (needstemp == true)
                            {
                                stack.Add("T" + tmpnum); tmpnum++;
                                needstemp = false;
                            }
                            precpointer.RemoveAt(precpointer.Count - 1);
                            //PrintList();
                            break;
                    }

                    //adds new item and looks if its one of the reserved words
                    text = stack.FindLast(x => x.StartsWith("@"));
                    /*
                                        if (popped = true)
                                        {
                                            popped = false;
                                        }
                                        else if (text != "@{")
                                        {
                                            Stack(token, classi);
                                        }
                                        else if (token == "}")
                                        {
                                            Stack(token, classi);
                                        }*/


                    if (popped != true)
                    {
                        if (token != ";" && token!=")") {  Stack(token, classi); }
                        else if (text.Substring(1) != "{" && token != ")")
                        {
                           
                            Stack(token, classi);
                        }
                        else
                        {
                            popped = false;
                        }
                    }
                }
                else
                {
                    //adds new op (@+token) to stack
                    if (token != ";" & token != "}") { stack.Add("@" + token);  }
                }
              /*  Console.WriteLine("TOKEN " + token + " TEXT " + text);
                PrintList();
                Console.WriteLine("\n");*/
            }
            else
            {
                //if its not an operator, add to stack 
                if (classi == "NumLit")
                {
                    stack.Add("#"+token);
                }
                else
                {
                    stack.Add(token);
                }
               
            }
          

        }
        private void ReadTokenClass(String file)
        { 
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string temp = "0";
                int num;
                string[] lines;
                //ignoring first line
                text = sr.ReadLine();

                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();

                    string[] stringSeparators = new string[] { "\n", "\t", " " };
                    lines = text.Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                
                    //checking to see whether next token is an operator (OP Classification in TokenClass.txt starts with an @)
                    if (lines[1].StartsWith("@"))
                    {
                        //if its an operator, add to stack and compare with previous op

                        //finds previous operator relation to new operator
                        text = stack.FindLast(x => x.StartsWith("@"));
                        if (stack.Count != 0) temp = opgo.FindOPGCell(text.Substring(1), lines[0]);
                        else precpointer.Add(stack.Count);

                        if (temp.Equals("<")){ precpointer.Add(stack.Count);  }
                        if (temp == ">")
                        {
                            //POP:
                            switch (text.Substring(1))
                            {
                                //case "IF": Console.WriteLine(lines[0] + ", L" + label + ", ?, ?"); label++; break;
                                default: Console.WriteLine(text.Substring(1) + ", " + stack[precpointer[precpointer.Count - 1] - 1] + " " + stack[stack.Count - 1] + ", ?"); break;
                            }
                            //removes popped items from the stack. RemoveRange can only remove if the range that is being removed is smaller than whats left in the stack
                            //hence the catch statement
                            try { stack.RemoveRange(precpointer[precpointer.Count - 1] - 1, stack.Count - 1); }
                            catch (System.ArgumentException)
                            {
                                for (int i = stack.Count - 1; i >= precpointer[precpointer.Count - 1] - 1; i--)
                                {
                                    stack.RemoveAt(i);
                                }
                            }
                            precpointer.RemoveAt(precpointer.Count - 1);

                            //adds new item and looks if its one of the reserved words
                            text = stack.FindLast(x => x.StartsWith("@"));
                            stack.Add("@" + lines[0]);
                           

                            SwitchAddingItem(lines[0]);
                        }
                        else
                        {
                            //adds new op (@+token) to stack
                            stack.Add("@" + lines[0]);
                            //goes through a switch statement to determine how to handle the item
                            SwitchAddingItem(lines[0]);
                        }
                    }
                    else
                    {
                        //if its not an operator, add to stack 
                        stack.Add(lines[0]);
                    }
                }
                //AFTER FILE WAS READ
                while (stack.Count!=0)
                {
                    text = stack.Find(x => x.StartsWith("@"));
                    try { stack.RemoveRange(precpointer[precpointer.Count - 1] - 1, stack.Count - 1); }   
                    catch (System.ArgumentException)
                    {
                        for (int i = stack.Count - 1; i >= precpointer[precpointer.Count - 1]; i--)
                        {
                            stack.RemoveAt(i);
                        }
                    }
                    precpointer.RemoveAt(precpointer.Count - 1);


                }
             }
        }

        private void SwitchAddingItem(string item)
        {
            switch (item)
            {
                case "IF": case "WHILE": Console.WriteLine(item + ", -, -, -"); break;
                case "THEN": Console.WriteLine(item + ", L" + label + "-, -, -"); label++; break;

            }
        }
        private void PrintList()
        {
            Console.WriteLine("list");
            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }
           // Console.WriteLine();
        }
    }
}
