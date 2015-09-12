using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace task3
{
    class DTW
    {
        private double[][] g;
        private double[][] pattern; // array of patern signal
        private double[][] analyzed; // array of analyzed signal
        private bool scband;
        private double gMax = 0.0;

        private double minimalPath = 0;

        public DTW(String patternFile, String analyzedFile, bool scband)
        {

            this.scband = scband;
            this.pattern = ReadMelFromFile(patternFile);
            this.analyzed = ReadMelFromFile(analyzedFile);
            this.g = new double[pattern.Length][];

            // fill array with random high numbers, leave starting point
            g[0] = new double[analyzed.Length];
            g[0][0] = 0;

            for (int i = 1; i < pattern.Length; i++)
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
        public void CalculatePath()
        {
            for (int j = 1; j < pattern.Length; j++)
            {
                for (int i = 1; i < analyzed.Length; i++)
                {
                    if (!SCBand(i, j))
                    {
                        g[j][i] = 99999;
                    }
                    else
                    {
                        List<double> temp = new List<double>();
                        temp.Add(g[j][i - 1]);
                        temp.Add(g[j - 1][i - 1]);
                        temp.Add(g[j - 1][i]);
                        g[j][i] = Round(EuclideanDistance(pattern[j], analyzed[i])
                                + Min(temp), 2);
                        if (g[j][i] > gMax)
                        {
                            gMax = g[j][i];
                        }
                    }
                }
            }

            // normalize
            minimalPath = g[pattern.Length - 1][analyzed.Length - 1] / (pattern.Length + analyzed.Length);
            FindBestPath();
        }

        // (16)
        public bool SCBand(int i, int j)
        {
            if (scband)
            {
                return Math.Abs(i - j) <= 2;
            }
            else
            {
                return true;
            }
        }

        // (7) calculate Euclidean distance
        private double EuclideanDistance(double[] s, double[] t)
        {
            double sum = 0.0;
            for (int i = 0; i < s.Length; i++)
            {
                sum += Math.Pow(Math.Abs(s[i] - t[i]), 2);
            }
            return Math.Sqrt(sum);
        }

        private double Min(List<double> values)
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

        public void FindBestPath()
        {
            int j = g.Length;
            int i = g[0].Length;

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

                double sum = 0.0;
                if (i >= 0 && j >= 0)
                {
                    sum += g[j][i];
                }
            }
        }

        public double GetMinimalPath()
        {
            return minimalPath;
        }

        public static double Round(double value, int places)
        {

            long factor = (long)Math.Pow(10, places);
            value = value * factor;
            long tmp = (long)Math.Round(value);
            return (double)tmp / factor;
        }

        private double[][] ReadMelFromFile(String filePath)
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
                    array.Add(ArrayToDouble(row));
                }
                else
                {
                    array.Add(new Double[] { Double.Parse(line) });
                }
                counter++;
            }

            file.Close();


            return ArrayListToDouble(array);
        }

        public static double[][] ArrayListToDouble(List<Double[]> array)
        {
            double[][] res = new double[array.Count()][];

            for (int i = 0; i < array.Count(); i++)
            {
                res[i] = ArrayToDouble(array[i]);
            }

            return res;
        }

        public static Double[] ArrayToDouble(String[] array)
        {
            Double[] res = new Double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                res[i] = Double.Parse(array[i]);
            }

            return res;
        }

        public static double[] ArrayToDouble(Double[] array)
        {
            double[] res = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                res[i] = array[i];
            }

            return res;
        }
    }
}
