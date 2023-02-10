using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurrisCompile.Step_4__Code_Generation
{
    internal class Generate
    {
        public void Run ()
        {
            FirstPart();
            Read();
            LastPart();
        }
        private void FirstPart()
        {
            Console.WriteLine("sys_exit \t equ \t 1");
            Console.WriteLine("sys_read \t equ \t 3");
            Console.WriteLine("sys_write \t equ \t 4");
            Console.WriteLine("stdin     \t equ \t 0");
            Console.WriteLine("stdout    \t equ \t 1");
            Console.WriteLine("stderr    \t equ \t 3");

            Console.WriteLine("");
            Console.WriteLine("section .data");
            Console.WriteLine("\tuserMsg \t db \t 'Enter an int(less than 32,765): '");
            Console.WriteLine("\tlenUserMsg \t equ \t	$-userMsg");


            Console.WriteLine("\tdisplayMsg      \t db  \t 'You entered: '");
            Console.WriteLine("\tlenDisplayMsg   \t equ \t  $-displayMsg");
            Console.WriteLine("\tnewline         \t db  \t  0xA");
            Console.WriteLine("\tTen             \t DW  \t  10");
            Console.WriteLine("\tprintTempchar   \t db  \t  'Tempchar = : '");
            Console.WriteLine("\tlenprintTempchar\t equ \t  $-printTempchar");
            Console.WriteLine("\tResult          \t db  \t  'Ans = '");
            Console.WriteLine("\tResultValue     \t db  \t  'aaaaa'");
            Console.WriteLine("                  \t db  \t  0xA");
            Console.WriteLine("\tResultEnd       \t equ \t  $-Result");
            Console.WriteLine("\tnum             \t times 6 db 'ABCDEF'");
            Console.WriteLine("\tnumEnd          \t equ	\t $-num");

            using (StreamReader sr = File.OpenText("SymbolTable.txt"))
            {
                string[] stringSeparators = new string[] { "\n", "\t", " " };

                string text, temp;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (lines[1] == "CONST")
                    {
                        Console.WriteLine($"\t{lines[0]}          \t RESW	1  ");

                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("section .bss");
            Console.WriteLine("\tTempChar       \t RESW	1  ");
            Console.WriteLine("\ttestchar       \t RESW	1  ");
            Console.WriteLine("\tReadInt        \t RESW	1  ");
            Console.WriteLine("\ttempint        \t RESW	1  ");
            Console.WriteLine("\tnegflag        \t RESW	1  ");
            Console.WriteLine("\tT1       \t RESW	1  ");
            Console.WriteLine("\tT2        \t RESW	1  ");
            Console.WriteLine("\tT3        \t RESW	1  ");
            Console.WriteLine("\tT4        \t RESW	1  ");
            Console.WriteLine("\tT5        \t RESW	1  ");
            using (StreamReader sr = File.OpenText("SymbolTable.txt"))
            {
                string[] stringSeparators = new string[] { "\n", "\t", " " };

                string text, temp;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (lines[1] == "VAR")
                    {
                        Console.WriteLine($"\t{lines[0]}          \t RESW	1  ");

                    }
                }
            }
            Console.WriteLine("");
            Console.WriteLine("\tglobal main");
            Console.WriteLine("");
            Console.WriteLine("section .text");
        }
        private void LastPart()
        {
            using (StreamReader sr = File.OpenText("AssemblyEndProcedures.txt"))
            {
               Console.WriteLine(sr.ReadToEnd());
            }
        }
        private void Read()
        {
            string procedure = "";

            using (StreamReader sr = File.OpenText("Quads.txt"))
            {               
                string[] stringSeparators = new string[] { "\n", "\t", " "};
              
                string text, temp;
                List<string> whilelabels = new List<string>();
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    //*, A B, C
                    bool numlitContains = false;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("#"))
                        {
                            numlitContains = true;
                        }
                    }
                    


                    if (numlitContains==true)
                    {
                        //modifying string format to either have brackets or just a numlit
                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (lines[i].Contains("#"))
                            {                               
                                lines[i] = lines[i].Substring(1);
                            }
                            else
                            {
                                lines[i] = "[" + lines[i] + "]";
                            }
                        }

                        switch (lines[0])
                    {
                        case "*":
                            Console.WriteLine("mov al, " + lines[1] + "");
                            Console.WriteLine("mov bl, " + lines[2] + "");
                            Console.WriteLine("mul bl");
                            Console.WriteLine($"mov {lines[3]}, ax");
                            break;
                        case "/":
                            Console.WriteLine("mov ax, " + lines[1] + "");
                            Console.WriteLine("mov bl, " + lines[2] + "");
                            Console.WriteLine("div bl");
                            Console.WriteLine($"mov {lines[3]}, ah");
                            break;
                         case "+":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("add ax, " + lines[2] );
                            Console.WriteLine("mov " + lines[3] + ", ax");
                            break;
                        case "-":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("sub ax, " + lines[2] );
                            Console.WriteLine("mov " + lines[3] + ", ax");
                            break;
                        case "=":
                            Console.WriteLine("mov ax, " + lines[2] + "");
                            Console.WriteLine("mov " + lines[1] + ", ax");
                            break;
                        case ">":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("cmp ax, " + lines[2] );
                            Console.Write("JLE ");
                            break;
                        case "<":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("cmp ax, " + lines[2] );
                            Console.Write("JGE ");
                            break;
                        case ">=":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("cmp ax, " + lines[2] );
                            Console.Write("JL ");
                            break;
                        case "<=":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("cmp ax, " + lines[2] );
                            Console.Write("JG ");
                            break;

                        case "!=":
                            Console.WriteLine("mov ax, " + lines[1] );
                            Console.WriteLine("cmp ax, " + lines[2] );
                            Console.Write("JE ");
                            break;

                        case "ODD":
                            Console.WriteLine("mov ax, " + lines[1]);
                            Console.WriteLine("div ax, 2");
                            Console.WriteLine("cmp ah, 0");
                            Console.Write("JE ");
                            break;
                        case "COUT":
                            Console.WriteLine("mov     bx,OFFSET EnterMessage ");//?
                            Console.WriteLine("call    PrintString");
                            Console.WriteLine("call    GetAnInteger");
                            break;
                            
                            
                            default:
                        Console.WriteLine("U havent done this yet");
                        break;


                    }
                }
                    else
                    {

                   
                    switch (lines[0])
                    {
                        case "*":
                            Console.WriteLine("mov al, [" + lines[1] + "]");
                            Console.WriteLine("mov bl, [" + lines[2] + "]");
                            Console.WriteLine("mul bl");
                            Console.WriteLine($"mov [{lines[3]}], ax");
                            break;
                        case "/":
                            Console.WriteLine("mov al, [" + lines[1] + "]");
                            Console.WriteLine("mov bl, [" + lines[2] + "]");
                            Console.WriteLine("div bl");
                            Console.WriteLine($"mov [{lines[3]}], ax");
                                break;
                        case "+":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("add ax, [" + lines[2] + "]");
                            Console.WriteLine("mov [" + lines[3] + "], ax");
                            break;
                        case "-":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("sub ax, [" + lines[2] + "]");
                            Console.WriteLine("mov [" + lines[3] + "], ax");
                            break;
                        case "=":
                            Console.WriteLine("mov ax, [" + lines[2] + "]");
                            Console.WriteLine("mov [" + lines[1] + "], ax");
                            break;
                        case ">":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("cmp ax, [" + lines[2] + "]");
                            Console.Write("JLE ");
                            break;
                        case "<":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("cmp ax, [" + lines[2] + "]");
                            Console.Write("JGE ");
                            break;
                        case ">=":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("cmp ax, [" + lines[2] + "]");
                            Console.Write("JL ");
                            break;
                        case "<=":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("cmp ax, [" + lines[2] + "]");
                            Console.Write("JG ");
                            break;
                        case "!=":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("cmp ax, [" + lines[2] + "]");
                            Console.Write("JE ");
                            break;
                        case "ODD":
                            Console.WriteLine("mov ax, [" + lines[1] + "]");
                            Console.WriteLine("div ax, 2");
                            Console.WriteLine("cmp ah, 0");
                            Console.Write("JE ");
                            break;
                        case "PROCEDURE":
                            Console.WriteLine(lines[1]+": ");
                            procedure = lines[1];
                            break;
                        case "IF":                          
                            break;
                        case "THEN":
                            Console.WriteLine(lines[1]);
                            break;
                        case "WHILE":
                            whilelabels.Add(lines[1]);
                            Console.WriteLine(lines[1] + ": NOP");
                            break;
                        case "DO":
                            Console.WriteLine(lines[1]);
                            break;
                        case "WHILEFIN":
                            Console.WriteLine("JMP " + whilelabels[whilelabels.Count - 1]);
                            whilelabels.RemoveAt(whilelabels.Count - 1);    
                            Console.WriteLine(lines[1] + ": NOP");
                            break;
                        case "FIN":
                            Console.WriteLine(lines[1] + ": NOP");
                            break;
                        case "FINI":
                            Console.WriteLine("fini:");
                            Console.WriteLine("mov eax,sys_exit");
                            Console.WriteLine("xor ebx,ebx");
                            Console.WriteLine("int 80h");
                            break;
                        case "START":
                            Console.WriteLine("main:	nop");
                            break;
                        case "CALL":
                            Console.WriteLine(text);
                            break;
                       case "RECURSIVE":
                                Console.WriteLine("jmp " + lines[2]);
                                break;
                            case "END":
                            Console.WriteLine("ret");
                            //Console.WriteLine(procedure + "\t ENDP\n");
                            break;
                        case "COUT":
                            Console.WriteLine($"mov ax,[{lines[1]}]");
                            Console.WriteLine("call ConvertIntegerToString");
                            Console.WriteLine("mov eax, 4");
                            Console.WriteLine("mov ebx, 1");
                            Console.WriteLine("mov ecx, Result	");
                            Console.WriteLine("mov edx, ResultEnd");
                            Console.WriteLine("int 80h");
                            break;

                        case "CIN":
                            Console.WriteLine("call PrintString");
                            Console.WriteLine("call GetAnInteger");
                            Console.WriteLine("mov ax,[ReadInt]");
                            Console.WriteLine($"mov [{lines[1]}], ax");
                            break;


                            default:
                            Console.WriteLine("U havent done this yet");
                            break;
                    }
                    }
                }
            }
        }
    }
}
