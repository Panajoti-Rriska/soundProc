using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Math;
using System.Diagnostics;

namespace task3
{
    class MFCC
    {
        String tag = "MFCC: ";

        private double ck;
        private double lk;
        private double rk;

        public MFCC() { }

        public double[][] GetMFCC(double[][] signal, int size, int fs)
        {
            int framerate = fs;
            int nframe = signal.Length;
            if (signal[nframe - 1].Length != size)
            {
                nframe--;
            }
            int K = 30;
            int d = 100;

            Debug.WriteLine(tag + "setting parameters");
            SetParameters(K, d);

            int F = 12;
            double[][] mfcc = new double[(int)nframe][];

            int N = size;
            double[] spectrum = new double[N];

            Complex[] s = new Complex[N];

            Debug.WriteLine(tag + "calculating MFCC for each block");
            for (int f = 0; f < nframe; f++)
            {
                double[] samples = signal[f];

                if (samples.Length != N)
                {
                    break;
                }

                samples = HammingWindow(samples);
                for (int i = 0; i < N; i++)
                {
                    s[i] = new Complex(samples[i], 0);
                }

                s = FFT.Forward(s);

                double[] c;
                for (int i = 0; i < N; i++)
                {
                    spectrum[i] = s[i].Re;
                }
                c = SetCn(spectrum, (int)framerate, K, d, F);
                mfcc[f] = c;
            }

            return mfcc;
        }

        public static double[] HammingWindow(double[] array)
        {
            double[] result = new double[array.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0.53836 - 0.46164 * Math.Cos((2 * Math.PI * i) / (array.Length - 1));
                result[i] = result[i] * array[i];
            }
            return result;
        }

        // (1)
        public double SetHk(double f)
        {
            if (f >= lk && f <= ck)
            {
                return (f - lk) / (ck - lk);
            }
            else if (f > ck && f <= rk)
            {
                return (rk - f) / (rk - ck);
            }
            else
            {
                return 0.0;
            }
        }

        // (2)
        public void SetParameters(double k, double d)
        {
            ck = MelsToFreq(k * d);
            lk = MelsToFreq((k - 1) * d);
            rk = MelsToFreq((k + 1) * d);
        }

        // (3)
        public double MelsToFreq(double x)
        {
            return 700 * (Math.Pow(10, (x / 2595.00)) - 1);
        }

        // (4)
        public double SetSk(double[] signal, int fs, int k, int d)
        {
            SetParameters(k, d);
            double result = 0;
            for (int i = 0; i < signal.Length / 2; i++)
            {
                result += Math.Abs(signal[i]) * SetHk((fs / signal.Length) * i);
            }
            return result;

        }

        // (5)
        public double[] SetCn(double[] signal, int fs, int K, int d, int F)
        {
            double[] c = new double[F];
            for (int n = 1; n <= c.Length; n++)
            {
                double result = 0;
                for (int k = 0; k < K - 1; k++)
                {
                    double s_prim_value = SetSprim(signal, fs, k, d);
                    result += s_prim_value
                            * Math.Cos(AngToRad(2 * Math.PI
                                            * ((2 * k + 1) * n) / 4 * K));
                }
                c[n - 1] = result;
            }
            return c;
        }

        public double AngToRad(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        // (6)
        public double SetSprim(double[] signal, int fs, int k, int d)
        {
            double s1 = SetSk(signal, fs, k, d);
            double s2 = Math.Log(s1);
            return Math.Pow(s2, 2);
        }
    }
}
