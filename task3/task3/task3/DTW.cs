using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace task3
{
    class DTW
    {
        private List<int> rows;
        private List<int> cols;
        private double[][] g;
        private double[][] model; // array of model signal
        private double[][] analyzed; // array of analyzed signal
        private bool itakura;
        private double gMax = 0.0;

        private double minimalPath = 0;

        public DTW(String modelFile, String analyzedFile, bool itakura)
        {

            this.itakura = itakura;
            this.model = readMelFromFile(modelFile);
            this.analyzed = readMelFromFile(analyzedFile);
            this.g = new double[model.Length][];

            // fill array with random high numbers, leave starting point
            g[0] = new double[analyzed.Length];
            g[0][0] = 0;

            for (int i = 1; i < model.Length; i++)
            {
                g[i] = new double[analyzed.Length];
                g[i][0] = 99999;
            }

            for (int j = 1; j < analyzed.Length; j++)
            {
                g[0][j] = 99999;
            }
        }

        // (17) 
        public void calculatePath()
        {
            for (int j = 1; j < model.Length; j++)
            {
                for (int i = 1; i < analyzed.Length; i++)
                {
                    if (!itakuraConstraint(i, j, analyzed.Length, model.Length))
                    {
                        g[j][i] = 99999;
                    }
                    else
                    {
                        List<double> temp = new List<double>();
                        temp.Add(g[j][i - 1]);
                        temp.Add(g[j - 1][i - 1]);
                        temp.Add(g[j - 1][i]);
                        g[j][i] = round(euclideanDistance(model[j], analyzed[i])
                                + min(temp), 2);
                        if (g[j][i] > gMax)
                        {
                            gMax = g[j][i];
                        }
                    }
                    //                System.out.print("g[" + j + "][" + i + "]=" + g[j][i] + " ");
                }
                //            System.out.println("");
            }

            // normalize
            minimalPath = g[model.Length - 1][analyzed.Length - 1] / (model.Length + analyzed.Length);
            findBestPath();
        }

        // (16)
        public bool itakuraConstraint(int i, int j, int I, int J)
        {
            if (itakura)
            {
                int lowerTop = 2 * (i - I) + J;
                int lowerBottom = (int)(0.5 * (i - 1) + 1);
                int upperBottom = 2 * (i - 1) + 1;
                int upperTop = (int)(0.5 * (i - I) + J);
                return j >= lowerTop && j >= lowerBottom && j <= upperBottom && j <= upperTop;
            }
            else
            {
                return true;
            }
        }

        // (7) calculate Euclidean distance
        private double euclideanDistance(double[] s, double[] t)
        {
            double sum = 0.0;
            for (int i = 0; i < s.Length; i++)
            {
                sum += Math.Pow(Math.Abs(s[i] - t[i]), 2);
            }
            return Math.Sqrt(sum);
        }

        private double min(List<double> values)
        {
            double min = 99999;

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] < min)
                {
                    min = values[i];
                }
            }
            return min;
        }

        public void findBestPath()
        {
            rows = new List<int>();
            cols = new List<int>();

            int j = g.Length;
            int i = g[0].Length;

            rows.Add(i);
            cols.Add(j);
            j--;
            i--;

            while ((j >= 0) && (i >= 0))
            {
                double diagCost;
                double leftCost;
                double upCost;

                if ((j > 0) && (i > 0))
                {
                    diagCost = g[j - 1][i - 1];
                }
                else
                {
                    diagCost = 99999;
                }

                if (j > 0)
                {
                    upCost = g[j - 1][i];
                }
                else
                {
                    upCost = 99999;
                }

                if (i > 0)
                {
                    leftCost = g[j][i - 1];
                }
                else
                {
                    leftCost = 99999;
                }

                if ((diagCost <= leftCost) && (diagCost <= upCost))
                {
                    j--;
                    i--;
                }
                else if ((leftCost < diagCost) && (leftCost < upCost))
                {
                    i--;
                }
                else if ((upCost < diagCost) && (upCost < leftCost))
                {
                    j--;
                }
                else
                {
                    i--;
                }

                rows.Add(i);
                cols.Add(j);

                double sum = 0.0;
                if (i >= 0 && j >= 0)
                {
                    sum += g[j][i];
                }
            }
        }

        public double getMinimalPath()
        {
            return minimalPath;
        }

        public static double round(double value, int places)
        {

            long factor = (long)Math.Pow(10, places);
            value = value * factor;
            long tmp = (long)Math.Round(value);
            return (double)tmp / factor;
        }

        private double[][] readMelFromFile(String filePath)
        {
            List<Double[]> array = new List<Double[]>();

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                String[] row = line.Split('\t');
                if (row.Length > 1)
                {
                    array.Add(arrayToDouble(row));
                }
                else
                {
                    array.Add(new Double[] { Double.Parse(line) });
                }
                Console.WriteLine(line);
                counter++;
            }

            file.Close();


            return arrayListToDouble(array);
        }

        public static double[][] arrayListToDouble(List<Double[]> array)
        {
            double[][] res = new double[array.Count()][];

            for (int i = 0; i < array.Count(); i++)
            {
                res[i] = arrayToDouble(array[i]);
            }

            return res;
        }

        public static double[] arrayListToDouble2(List<int> array)
        {
            double[] res = new double[array.Count()];

            for (int i = 0; i < array.Count(); i++)
            {
                res[i] = array[i];
            }

            return res;
        }

        public static Double[] arrayToDouble(String[] array)
        {
            Double[] res = new Double[array.Length];
            
            for (int i = 0; i < array.Length; i++)
            {
                res[i] = Double.Parse(array[i]);
            }

            return res;
        }

        public static double[] arrayToDouble(Double[] array)
        {
            double[] res = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                res[i] = array[i];
            }

            return res;
        }

        public static double[] twoDtoOneD(double[][] array)
        {
            double[] res = new double[array.Length * array[0].Length];

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    res[i * array[i].Length + j] = array[i][j];
                }
            }

            return res;
        }
    }
}
