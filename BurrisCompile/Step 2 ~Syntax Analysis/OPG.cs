namespace BurrisCompile
{
    //this class will build the precedence relation table using (<) = (=)(FIRST*)(FIRSTTERM), (>)=((LAST*)(LASTTERM))T(=)
    //it will do so using language.txt, which contains the grammar rules used by our language
    //output table is written in OPGTable.txt {refer to pg}

    internal class OPG
    {
        private string[] TokensArr;
        private string[] OperatorsArr;
        private string TextFile;

        public OPG()
        {
            TextFile = "Language.txt";
        }
        public void Whatwegonnado()
        {
            createTokenList();
            createOperatorList();
            PrintTable();
        }

        //populates OperatorsArr array with all operators used in the grammar

        /* an operator will be any word that is not surrounded by <>
        they must also be seperated by whitespace (no () it must be ( ) )
        no seperate line for operators ie.<addop> - + | -; 
        it must be seperated in the textfile itself beforehand */       
        private void createOperatorList()
        {
            List<string> OpList = new List<string>();
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    //this will ignore everything inside <> and whats before -
                    for (int i = 2; i < lines.Length; i++)
                    {
                        if (!lines[i].StartsWith("<") && !OpList.Contains(lines[i]))
                        {
                            OpList.Add(lines[i]);
                        }

                        if (lines[i].StartsWith("<=") && !OpList.Contains(lines[i]))
                        {
                            OpList.Add(lines[i]);
                        }

                        if (lines[i] == ("<") && !OpList.Contains(lines[i]))
                        {
                            OpList.Add(lines[i]);
                        }
                    }
                }
            }
            OperatorsArr = OpList.ToArray();
        }

        //populates TokensArr array with all tokens/words used in the grammar
        private void createTokenList()
        {
            List<string> TokenList = new List<string>();
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    //this will ignore - the little arrow and add unique tokens to the list
                    for (int i = 0; i < lines.Length; i++)
                    {

                        if (i != 1 & !TokenList.Contains(lines[i]))
                        {
                            TokenList.Add(lines[i]);

                        }
                    }
                }
                TokensArr = TokenList.ToArray();
            }
        }

        //outputs Firstterm table, an array indicating the first operator that appears in a rule
        private int[,] FirstTerm()
        {
            int size = TokensArr.Length;
            List<string> OpinList = new List<string>();
            int row = 0; int col = 0;
            int[,] FirstTerm = new int[size, size];
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                bool found = false;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    //goes through the line, looks through each word
                    //if an operator is found add it to the list then move on to next line

                    for (int m = 2; m < lines.Length; m++)
                    {
                        if (!OpinList.Contains(lines[m]))
                        {

                            for (int i = 0; i < OperatorsArr.Length; i++)
                            {

                                if (OperatorsArr[i] == lines[m])
                                {
                                    OpinList.Add(lines[m]);
                                    for (int j = 0; j < TokensArr.Length; j++)
                                    {
                                        if (OperatorsArr[i] == TokensArr[j])
                                        {
                                            col = j;
                                        }

                                        if (lines[0] == TokensArr[j])
                                        {
                                            row = j;
                                        }
                                        //if found stop for loop
                                    }
                                    FirstTerm[row, col] = 1;
                                    found = true;
                                    break;
                                }
                            }
                            if (found == true)
                            {
                                break;
                            }
                        }
                    }

                    found = false;
                }
            }
            return FirstTerm;
        }

        //outputs Lastterm table, an array indicating the last operator that appears in a rule
        private int[,] LastTerm()
        {
            int size = TokensArr.Length;
            List<string> OpinList = new List<string>();
            int row = 0; int col = 0;
            int[,] FirstTerm = new int[size, size];
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                bool found = false;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    //goes through the line, looks through each word
                    //if an operator is found add it to the list then move on to next line

                    for (int m = lines.Length - 1; m > 1; m--)
                    {
                        if (!OpinList.Contains(lines[m]))
                        {

                            for (int i = 0; i < OperatorsArr.Length; i++)
                            {

                                if (OperatorsArr[i] == lines[m])
                                {
                                    OpinList.Add(lines[m]);
                                    for (int j = 0; j < TokensArr.Length; j++)
                                    {
                                        if (OperatorsArr[i] == TokensArr[j])
                                        {
                                            col = j;
                                        }

                                        if (lines[0] == TokensArr[j])
                                        {
                                            row = j;
                                        }
                                        //if found stop for loop
                                    }
                                    FirstTerm[row, col] = 1;
                                    found = true;
                                    break;
                                }
                            }
                            if (found == true)
                            {
                                break;
                            }
                        }
                    }

                    found = false;
                }
            }
            return FirstTerm;
        }

        //outputs EqualOpTerm table, an array indicating all operators that follow each other in a rule
        private int[,] EqualOpTerm()
        {
            int size = TokensArr.Length;
            int row = 0; int col = 0;
            int[,] EqualTerm = new int[size, size];
            List<string> OpinList = new List<string>();

            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    //goes through the line, looks through each word
                    //if an operator is found add it to the list then move on to next line

                    for (int m = 2; m < lines.Length; m++)
                    {
                        for (int i = 0; i < OperatorsArr.Length; i++)
                        {
                            if (OperatorsArr[i] == lines[m])
                            {
                                OpinList.Add(lines[m]);
                            }
                        }

                    }
                    if (OpinList.Count > 1)
                    {
                        for (int i = 0; i < OpinList.Count - 1; i++)
                        {
                            for (int m = 0; m < TokensArr.Length; m++)
                            {
                                if (TokensArr[m] == OpinList[i])
                                {
                                    row = m;
                                }
                                if (TokensArr[m] == OpinList[i + 1])
                                {
                                    col = m;
                                }
                            }
                            EqualTerm[row, col] = 1;
                        }
                    }
                    OpinList.Clear();
                }
            }
            return EqualTerm;

        }

        //creates first table
        private int[,] First()
        {
            int size = TokensArr.Length;
            int row = 0, col = 0;
            int[,] FirstTable = new int[size, size];
            string fileName = System.IO.Path.GetFullPath
            (Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                string[] seperate = { " ", "\t", "\n" };

                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < TokensArr.Length; i++)
                    {
                        if (TokensArr[i] == lines[0])
                        {
                            row = i;
                        }
                        if (TokensArr[i] == lines[2])
                        {
                            col = i;
                        }
                    }
                    FirstTable[row, col] = 1;
                }
            }
            return FirstTable;
        }

        //creates the last table
        private int[,] Last()
        {
            int size = TokensArr.Length;
            int row = 0, col = 0;
            int[,] LastTable = new int[size, size];
            string fileName = System.IO.Path.GetFullPath
            (Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ",
                    StringSplitOptions.RemoveEmptyEntries); ;
                    for (int i = 0; i < TokensArr.Length; i++)
                    {
                        if (TokensArr[i] == lines[0])
                        {
                            row = i;
                        }
                        if (TokensArr[i] == lines[lines.Length - 1])
                        {
                            col = i;
                        }
                    }
                    LastTable[row, col] = 1;
                }
            }
            return LastTable;
        }

        //creates = table
        private int[,] Equal()
        {
            int size = TokensArr.Length;
            int row = 0, col = 0;
            int[,] EqualTable = new int[size, size];
            string fileName = System.IO.Path.GetFullPath
            (Directory.GetCurrentDirectory() + @"\" + TextFile);
            using (StreamReader sr = File.OpenText(fileName))
            {
                string text;
                string[] lines;
                while (sr.Peek() >= 0)
                {
                    text = sr.ReadLine();
                    lines = text.Trim().Split(" ",
                    StringSplitOptions.RemoveEmptyEntries); ;
                    for (int i = 2; i < lines.Length - 1; i++)
                    {
                        for (int m = 0; m < TokensArr.Length; m++)
                        {
                            //determine first token 
                            if (TokensArr[m] == lines[i])
                            {
                                row = m;
                            }
                            //determine following token 
                            if (TokensArr[m] == lines[i + 1])
                            {
                                col = m;
                            }
                        }
                        EqualTable[row, col] = 1;
                    }
                }
            }
            return EqualTable;
        }
        //tables get passed down to warshall to get their +
        private int[,] Warshalls(int[,] arr)
        {
            int size = TokensArr.Length;
            int[,] WarshallTablePlus = arr.Clone() as int[,];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (WarshallTablePlus[j, i] == 1)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            if (WarshallTablePlus[j, k] != 1 &&
                            WarshallTablePlus[i, k] == 1)
                            {
                                WarshallTablePlus[j, k] = 1;
                            }
                        }
                    }
                }
            }
            return WarshallTablePlus;
        }
        //this will achieve give tables their *, aka left diagonally fill one
        private int[,] OnePlusTable(int[,] arr)
        {
            int size = TokensArr.Length;
            int[,] OneTable = arr.Clone() as int[,];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (j == i)
                    {
                        OneTable[i, j] = 1;
                    }
                }
            }
            return OneTable;

        }
        private int[,] BooleanProd(int[,] arr, int[,] arr2)
        {
            int size = TokensArr.Length;
            int[] Bool = new int[size];
            int[,] BooleanProduct = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int m = 0; m < size; m++)
                    {
                        if (arr[i, m] == 1 && arr2[m, j] == 1)
                        {
                            BooleanProduct[i, j] = 1;
                        }
                    }
                }
            }
            return BooleanProduct;
        }
        private int[,] Tranpose(int[,] arr)
        {
            int size = TokensArr.Length;
            int[,] Transpose = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Transpose[j, i] = arr[i, j];
                }
            }
            return Transpose;
        }       
        //not needed or used here, but will be left
        private string[,] Precedence(int[,] equal, int[,] prec, int[,]yields)
        {
            int size = TokensArr.Length;
            string[,] ret = new string[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (prec[i, j] == 1)
                    {
                        ret[i, j] = ">";
                    }
                    if (yields[i, j] == 1)
                    {
                        ret[i, j] = "<";
                    }
                    if (equal[i, j] == 1)
                    {
                        ret[i, j] = ret[i, j] + "=";
                    }
                    else
                    {
                        if (prec[i, j] != 1 && yields[i, j] != 1)
                        {
                            ret[i, j]
                          = "0";
                        }
                    }
                }
            }
            return ret;
        }

        //outputs the OPG Table in question in the form of a string array 
        private string[,] OPGTableCreates()
        {
            // (<) = (=)(FIRST*)(FIRSTTERM), (>)=((LAST*)(LASTTERM))T(=)
            int[,] yields = BooleanProd(BooleanProd(Equal(), OnePlusTable(Warshalls(First()))), FirstTerm());
            int[,] prec = BooleanProd(Tranpose(BooleanProd(OnePlusTable(Warshalls(Last())), LastTerm())), Equal());
            int[,] equ = EqualOpTerm();

            int size = TokensArr.Length;
            int size2 = OperatorsArr.Length;

            int row = 0, col = 0;

            string[,] ret2 = new string[size2, size2];

            /*looks through and finds the matching rows and collumns from the arrays that contains all tokens (yields, prec, equ)
            to the array that contains only operators (our opg table)*/
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (prec[i, j] == 1)
                    {
                        for (int k = 0; k < size2; k++)
                        {
                            if (TokensArr[i] == OperatorsArr[k])
                            {
                                row = k;
                            }

                            if (TokensArr[j] == OperatorsArr[k])
                            {
                                col = k;
                            }
                        }
                        ret2[row, col] = ">";
                    }
                    if (yields[i, j] == 1)
                    {
                        for (int k = 0; k < size2; k++)
                        {
                            if (TokensArr[i] == OperatorsArr[k])
                            {
                                row = k;
                            }

                            if (TokensArr[j] == OperatorsArr[k])
                            {
                                col = k;
                            }
                        }
                        ret2[row, col] = "<";
                    }
                    if (equ[i, j] == 1)
                    {
                        for (int k = 0; k < size2; k++)
                        {
                            if (TokensArr[i] == OperatorsArr[k])
                            {
                                row = k;
                            }

                            if (TokensArr[j] == OperatorsArr[k])
                            {
                                col = k;
                            }
                        }
                        ret2[row, col] = "=";

                    }
                }
            }

            //fills the empty table slots with 0s, its not necessary but it helps to read the table
            for (int i = 0; i < size2; i++)
            {
                for (int j = 0; j < size2; j++)
                {
                    if (ret2[i, j] == null) ret2[i, j] = "0";                  
                }
            }
            return ret2;
        }

        //output table is written in OPGTable.txt {refer to pg} in appropriate formatting 
        private void PrintTable()
        {
            string file = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\OPGTable.txt");
            using StreamWriter sw = new StreamWriter(file);
            string[,] op = OPGTableCreates();
            string opprec = "";

            int size2 = OperatorsArr.Length;
            int fsize;

            //extra space for formating
            opprec += string.Format("{0,-7}", " ");
            //first row of tokens
            for (int i = 0; i < size2; i++)
            {
                fsize = OperatorsArr[i].Length + 2;
                opprec += string.Format("{0," + $"{fsize}" + "}", OperatorsArr[i]);
            }

            opprec += "\n";
            for (int i = 0; i < size2; i++)
            {
                //first col of tokens
                opprec += string.Format("{0,-7}", OperatorsArr[i]);
                //info inside array
                for (int j = 0; j < size2; j++)
                {
                    fsize = OperatorsArr[j].Length + 2;
                    opprec += string.Format("{0," + $"{fsize}" + "}", op[i, j]);
                }
                opprec += "\n";
            }
            //out
            sw.WriteLine("Operator Precedence Table:\n" + opprec);
        }
    }
}
