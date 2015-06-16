using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ocean_generator
{
    class lowpass
    {

        private float resonance;
        private float cutoffFrequency;
        private int sampleRate;
        private float s, c, alpha, r, a1, a2, a3, b1, b2;

        // Array of input values, latest are in front
        private float[] inputHistory = new float[2];

        // Array of output values, latest are in front
        private float[] outputHistory = new float[3];

        public void Setup(float cutoffFrequency, int sampleRate, float resonance, PassType passType)
        {
            this.resonance = resonance;
            this.cutoffFrequency = cutoffFrequency;
            this.sampleRate = sampleRate;

            switch (passType)
            {
                case PassType.Lowpass:

                    s = (float)Math.Sin((2 * Math.PI * cutoffFrequency) / sampleRate);
                    c = (float)Math.Cos((2 * Math.PI * cutoffFrequency) / sampleRate);
                    alpha = s / (2 * resonance);
                    r = 1 / (1 + alpha);

                    a1 = 0.5f * (1 - c) * r;
                    a2 = (1 - c) * r;
                    a3 = a1;
                    b1 = -2.0f * c * r;
                    b2 = (1 - alpha) * r;
                    break;

                case PassType.Highpass:

                    s = (float)Math.Sin((2 * Math.PI * cutoffFrequency) / sampleRate);
                    c = (float)Math.Cos((2 * Math.PI * cutoffFrequency) / sampleRate);
                    alpha = s / (2 * resonance);
                    r = 1 / (1 + alpha);

                    a1 = 0.5f * (1 + c) * r;
                    a2 = -1 * (1 + c) * r;
                    a3 = a1;
                    b1 = -2.0f * c * r;
                    b2 = (1 - alpha) * r;
                    break;
            }

        }

        public enum PassType
        {
            Highpass,
            Lowpass,
        }

        public void Update(float newInput)
        {
            float newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];

            this.inputHistory[1] = this.inputHistory[0];
            this.inputHistory[0] = newInput;

            this.outputHistory[2] = this.outputHistory[1];
            this.outputHistory[1] = this.outputHistory[0];
            this.outputHistory[0] = newOutput;
        }

        public float Value
        {
            get { return this.outputHistory[0]; }
        }
    }
}
