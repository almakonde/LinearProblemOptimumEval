using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using lpsolve55;

namespace LpSolveExample
{
    class Data
    {
        public double[] x1;
        public double[] x2;
        public double[] x3;

        public double[] y1;
        public double[] y2;
        public double[] y3;

        public double[] liambda;
        public double[] func;
        public double tetta = 1;
        public int size;

        public Data()
        {
            size = getRowCount();
            x1 = new double[size + 2];
            x2 = new double[size + 2];
            x3 = new double[size + 2];

            y1 = new double[size + 2];
            y2 = new double[size + 2];
            y3 = new double[size + 2];

            liambda = new double[size + 2];
            func = new double[size + 2];
        }

        public void loadFile()
        {
            func[size + 1] = tetta;
            StreamReader sr = File.OpenText("../../RB2008.csv");


            string str = sr.ReadLine();
            string[] arrStr = str.Split(';');

            if (arrStr[0].Equals("ID"))
                str = sr.ReadLine();

            for (int i = 1; str != null; i++)
            {

                arrStr = str.Split(';');

                x1[i] = Convert.ToDouble(arrStr[2]);
                x2[i] = Convert.ToDouble(arrStr[3]);
                x3[i] = Convert.ToDouble(arrStr[5]);

                y1[i] = Convert.ToDouble(arrStr[4]);
                y2[i] = Convert.ToDouble(arrStr[6]);
                y3[i] = Convert.ToDouble(arrStr[1]);

                str = sr.ReadLine();
                liambda[i] = 1;


            }
            sr.Close();
        }

        public int getRowCount()
        {
            int count = 0;

            StreamReader sr = File.OpenText("../../RB2008.csv");


            string str = sr.ReadLine();
            string[] arrStr = str.Split(';');

            if (arrStr[0].Equals("ID"))
                str = sr.ReadLine();
            else count++;

            for (int i = 1; str != null; i++)
            {
                str = sr.ReadLine();
                count++;
            }
            sr.Close();
            return count;
        }


        public void writeToFile(double[] arr, int count)
        {
            StreamWriter sw = null;
            if (count == 0)
            {

                sw = new StreamWriter("../../Output.csv", false, System.Text.Encoding.UTF8);
            }
            else
            {

                sw = new StreamWriter("../../Output.csv", true, System.Text.Encoding.UTF8);
            }


            sw.Write("ID;Value;\n");
            for (int i = 0; i < arr.Length; i++)
            {

                sw.Write((i + 1) + ";" + arr[i] + ";\n");
            }

            sw.Close();


        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            Console.SetBufferSize(80, 6000);
            Data data = new Data();


            data.loadFile();


            for (int i = 0; i < data.size; i++)
            {

                lpsolve.Init(".");


                int lp = lpsolve.make_lp(0, (data.size + 1));




                data.x1[201] = -data.x1[i + 1];
                lpsolve.add_constraint(lp, data.x1, lpsolve.lpsolve_constr_types.LE, 0);
                data.x2[201] = -data.x2[i + 1];
                lpsolve.add_constraint(lp, data.x2, lpsolve.lpsolve_constr_types.LE, 0);
                data.x3[201] = -data.x3[i + 1];
                lpsolve.add_constraint(lp, data.x3, lpsolve.lpsolve_constr_types.LE, 0);


                lpsolve.add_constraint(lp, data.y1, lpsolve.lpsolve_constr_types.GE, data.y1[i + 1]);
                lpsolve.add_constraint(lp, data.y2, lpsolve.lpsolve_constr_types.GE, data.y2[i + 1]);
                lpsolve.add_constraint(lp, data.y3, lpsolve.lpsolve_constr_types.GE, data.y3[i + 1]);

                lpsolve.add_constraint(lp, data.liambda, lpsolve.lpsolve_constr_types.EQ, 1);




                lpsolve.set_obj_fn(lp, data.func);



                lpsolve.lpsolve_return statuscode = lpsolve.solve(lp);
                Console.WriteLine("Номер решения " + (i + 1));




                if (statuscode == lpsolve.lpsolve_return.OPTIMAL)
                {

                    double obj = lpsolve.get_objective(lp);

                    long iter = lpsolve.get_total_iter(lp);

                    double[] Col = new double[lpsolve.get_Ncolumns(lp)];
                    lpsolve.get_variables(lp, Col);

                    data.writeToFile(Col, i);
                }


                lpsolve.delete_lp(lp);
                Console.WriteLine();
            }


        }
    }
}
