namespace BurrisCompile
{
    public class LSTM
    {
        public Matrix[] x;
        public Matrix[] w;

        public Matrix wo, wa, wi, wf;

        public Matrix u = new Matrix(1, 4);
        public float uo, ua, ui, uf;

        public float bo, ba, bi, bf;

        //backward values 
        float[] dOut;
        float[] ObservedOut;


        public struct lstm
        {
            public float Out;
            public float State;
            public float o, a, i, f;

            //for backward phase
            public float dState;
        }

        public lstm[] I = new lstm[50];

        public void Initialization()
        {
            I[0].Out = 0;
            I[0].State = 0;
        }
        public void SetValues(int t)
        {
            //needs to be changed to t inputs
            x = new Matrix[2];
            w = new Matrix[2];

            float[] x1 = new float[] { 1f, 2f };
            float[] x2 = new float[] { 0.5f, 3f };


            float[] fwa = new float[] { 0.45f, 0.25f };
            float[] fwi = new float[] { 0.95f, 0.8f };
            float[] fwf = new float[] { 0.7f, 0.45f };
            float[] fwo = new float[] { 0.6f, 0.4f };
            float[] fw = new float[] { 0.45f, 0.25f, 0.95f, 0.8f, 0.7f, 0.45f, 0.6f, 0.4f };



            for (int i = 0; i < 2; i++)
            {
                x[i] = new Matrix(2, 1);
                w[i] = new Matrix(2, 4, fw);
            }
            x[0].Fill(x1);
            x[1].Fill(x2);

            wa = new Matrix(1, 2, fwa);
            wi = new Matrix(1, 2, fwi);
            wf = new Matrix(1, 2, fwf);
            wo = new Matrix(1, 2, fwo);

            // Matrix of Ws of a,i,f,o
            //make it the last value of input
            // w[1]= new Matrix (2,4,fw);

            ua = .15f; ui = .8f; uf = .1f; uo = .25f;
            float[] fu = new float[] { ua, ui, uf, uo };
            u.Fill(fu);

            ba = .2f; bi = .65f; bf = .15f; bo = .1f;
            //string a = 1;

            //backwards
            ObservedOut = new float[2] { .5f, 1.25f };
            dOut = new float[t + 1];//needs to demonstrate -1*/
        }
        public void forward(int t)
        {
            //* is dot product
            if (t == 0)
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t].Out + bo);
                I[t].State = I[t].a * I[t].i + I[t].f * I[t].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
            else
            {
                I[t].a = Tanh(wa * x[t] + ua * I[t - 1].Out + ba);
                I[t].i = Sigmoid(wi * x[t] + ui * I[t - 1].Out + bi);
                I[t].f = Sigmoid(wf * x[t] + uf * I[t - 1].Out + bf);
                I[t].o = Sigmoid(wo * x[t] + uo * I[t - 1].Out + bo);
                I[t].State = I[t].a * I[t].i + I[t].f * I[t - 1].State;
                I[t].Out = Tanh(I[t].State) * I[t].o;
            }
            Console.WriteLine($"Iteration {t}: A {I[t].a}, I {I[t].i}, F {I[t].f}, O {I[t].o}, State {I[t].State}, Out {I[t].Out}");
        }

        public void backward(int t)
        {
            float dt;
            Matrix dx;
            float[] dG = new float[4];

            dt = I[t].Out - ObservedOut[t]; //linear
            Console.WriteLine("out value " + dOut[t]);

            if (dOut[t + 1] == 0) dOut[t + 1] = dt + dOut[t + 1];
            Console.WriteLine("next out value " + dOut[t + 1]);

            I[t].dState = dOut[t + 1] * I[t].o * (1 - Tanh2(I[t].State)) + (1 - Tanh2(I[t + 1].dState)) * I[t + 1].f;

            //GATES
            //a
            dG[0] = (float)(I[t].dState * I[t].i * (1 - Math.Pow(I[t].a, 2)));
            //i
            dG[1] = I[t].dState * I[t].a * I[t].i * (1 - I[t].i);
            //f
            if (t == 0) dG[2] = I[t].dState * I[0].State * I[t].f * (1 - I[t].f);
            else dG[2] = I[t].dState * I[t - 1].State * I[t].f * (1 - I[t].f);
            //o
            dG[3] = dOut[t + 1] * Tanh(I[t].State) * I[t].o * (1 - I[t].o);
            Matrix dGates = new Matrix(4, 1, dG);

            //update w vector
            //updating u vector
            //updating b vector;';;;
            //learning rate


            dx = Matrix.DotProductMatrix(w[1], dGates);
            w[0] = Matrix.OuterProduct(dGates, x[1]);


            dOut[t] = u * dGates;//out t-1
            Console.WriteLine($"\n\nBackwards {t}: dt {dt}, dOut {dOut[t + 1]}, F { I[t].dState},\n dGates: \n{dGates.ToString()} dx: \n{dx}dOut-1: {dOut[t]} \nwt gate:\n{w[t]}w0 gate:\n{w[0]} ");
            Console.WriteLine("The problem W:\ndgates: " + dGates + "\nxt" + x[t]);
            t -= 1;
            for (int i = t; i >= 0; i--)
            {
                backward(i);
            }
        }


        public void Run(int t)
        {
            Initialization();
            SetValues(2);
            for (int i = 0; i <= t; i++)
            {
                forward(i);
            }
            backward(1);
        }





        public static float Sigmoid(float value)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-value));
        }

        public static float Tanh(float x)
        {
            return (float)((Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1));
        }
        public static float Tanh2(float x)
        {
            return (float)(Math.Pow((Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1), 2));
        }
    }

}