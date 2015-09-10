using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Math;
using System.Diagnostics;

namespace task3
{
    class MelCepstrum
    {
        private double ck;
        private double lk;
        private double rk;

        public MelCepstrum() { }

        public double[][] getMelCepstrum(double[][] signal, int size, int fs)
        {
            int framerate = fs;
            int nframe = signal.Length;
            if (signal[nframe - 1].Length != size)
            {
                nframe--;
            }
            int K = 30;
            int d = 100;

            setParameters(K, d);

            int F = 12;
            double[][] melCoeff = new double[(int)nframe][];

            int N = size;
            double[] spectrum = new double[N];

            Complex[] s = new Complex[N];

            for (int f = 0; f < nframe; f++)
            {
                double[] samples = signal[f];

                if (samples.Length != N)
                {
                    break;
                }

                //Hamming
                samples = WindowFunction.Hamming(samples);
                for (int i = 0; i < N; i++)
                {
                    s[i] = new Complex(samples[i], 0);
                }

                //FFT
                s = FFT.Forward(s);

                double[] c;
                for (int i = 0; i < N; i++)
                {
                    spectrum[i] = s[i].Re;
                }
                c = setCn(spectrum, (int)framerate, K, d, F);
                melCoeff[f] = c;
            }

            return melCoeff;
        }

        // (1)
        public double setHk(double f)
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
        public void setParameters(double k, double d)
        {
            ck = melsToFreq(k * d);
            lk = melsToFreq((k - 1) * d);
            rk = melsToFreq((k + 1) * d);
        }

        // (3)
        public double melsToFreq(double x)
        {
            return 700 * (Math.Pow(10, (x / 2595.00)) - 1);
        }

        // (4)
        public double setSk(double[] signal, int fs, int k, int d)
        {
            setParameters(k, d);
            double result = 0;
            for (int i = 0; i < signal.Length / 2; i++)
            {
                result += Math.Abs(signal[i]) * setHk((fs / signal.Length) * i);
            }
            return result;

        }

        // (5)
        public double[] setCn(double[] signal, int fs, int K, int d, int F)
        {
            double[] c = new double[F];
            for (int n = 1; n <= c.Length; n++)
            {
                double result = 0;
                for (int k = 0; k < K - 1; k++)
                {
                    double s_prim_value = setSprim(signal, fs, k, d);
                    result += s_prim_value
                            * Math.Cos(ToRadians(2 * Math.PI
                                            * ((2 * k + 1) * n) / 4 * K));
                }
                c[n - 1] = result;
            }
            return c;
        }

        public double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        // (6)
        public double setSprim(double[] signal, int fs, int k, int d)
        {
            double s1 = setSk(signal, fs, k, d);
            double s2 = Math.Log(s1);
            return Math.Pow(s2, 2);
        }
    }
}
