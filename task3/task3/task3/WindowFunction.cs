using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task3
{
    class WindowFunction
    {
        public static double[] Hamming(double[] array)
        {
            double[] result = new double[array.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0.53836 - 0.46164 * Math.Cos((2 * Math.PI * i) / (array.Length - 1));
                result[i] = result[i] * array[i];
            }
            return result;
        }

        public static double[] Triangular(double[] array)
        {
            int N = array.Length;
            double[] result = new double[array.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 1 - Math.Abs((i - 0.5 * (N - 1)) / 0.5 * (N + 1));

            }
            return result;
        }

        public static float[] Hamming(float[] array)
        {
            float[] result = new float[array.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (float)(0.53836 - 0.46164 * Math.Cos((2 * Math.PI * i) / (array.Length - 1)));
                result[i] = (float)(result[i] * array[i]);
            }
            return result;
        }

        public static double[] Hanning(double[] array)
        {
            double[] result = new double[array.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0.5 * (1 - Math.Cos((2 * Math.PI * i) / (array.Length - 1)));
                result[i] = result[i] * array[i];
            }
            return result;
        }

        public static double[][] sampling(double[] array, int sampRate)
        {
            int samplingRate = sampRate;
            if (isPowerOfTwo(sampRate))
            {
                samplingRate = sampRate;
            }

            double[][] result = new double[(int)Math.Ceiling(array.Length / (double)samplingRate)][];

            for (int i = 0; i < result.Length; i++)
            {
                if (i * samplingRate + samplingRate <= array.Length)
                {
                    result[i] = new double[samplingRate];
                }
                else
                {
                    result[i] = new double[array.Length - i * samplingRate];
                }

                for (int j = 0; j < samplingRate; j++)
                {
                    if (i * samplingRate + j >= array.Length)
                    {
                        break;
                    }

                    result[i][j] = array[i * samplingRate + j];
                }
            }

            return result;
        }

        public static bool isPowerOfTwo(int n)
        {
            return ((n & (n - 1)) == 0) && n > 0;
        }
    }
}
