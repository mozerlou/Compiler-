namespace BurrisCompile
{
    public class FSAScanner
    {
        private int[,] FSA;
        private int row { get; set; }
        private int col { get; set; }

        public FSAScanner(String textfile)
        {
            DetermineTableSize(textfile);
            FSA = new int[row,col];//31,18
            ReadTable();            
        }

        //determines the FSA Table size based on file input
        public void DetermineTableSize(String file)
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\"+file);
            using (StreamReader sr = File.OpenText(fileName))
            {
                //ADD SAFETY
                string text = sr.ReadLine();
                
                //determines col length
                string[] lines = text.Split('\t');
                this.col = lines.Length-1;
                
                //determines row length
                int count = 0;
                while ((text = sr.ReadLine()) != null)
                {
                    count++;
                }
                this.row = count;
            }
        }

        //reads table and transfers it to array
        public void ReadTable()
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\fsatext2.txt");
            Console.WriteLine("Your file content is:");
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
                        for (int j = 0; j < lines.Length-1 ; j++)
                        {
                            if (int.TryParse(lines[j+1], out int mf)) FSA[i, j] = int.Parse(lines[j+1]); //in case user adds extra whitespace                            
                        }
                    }                   
                }                
            }
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    str += FSA[i, j] + " ";
                }
                str += "\n";
            }

            return str;
        }

        public String ReadProgram()
        {
            string fileName = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\SampleProgram.txt");

            string str = "";
            Console.WriteLine("Your file content is:");
            using (StreamReader sr = File.OpenText(fileName))
            {
             
                    str += sr.ReadToEnd;
                    Console.WriteLine(str);
                
                
            }
            return str;
        }


    }
}

