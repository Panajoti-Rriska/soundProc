using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Math;

namespace task3
{
    class FFT
    {
        public static Complex[] Forward(Complex[] signal)
        {
            FourierTransform.DFT(signal, FourierTransform.Direction.Forward);
            return signal;
        }

        public static Complex[] Backward(Complex[] signal)
        {
            FourierTransform.FFT(signal, FourierTransform.Direction.Backward);
            return signal;
        }

    }
}
