using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ocean_generator
{
    class oldfilter
    {
        private ISampleProvider sourceProvider;
        private int cutOffFreq; //low pass filter cutoff (tests)
        private int channels;
        private int duration;
        private int sampleRate;
        private int inputSamplesSize;
        private int amplitude = 3000; //amplitude for sine
        private int offset = 350; //offset for sine
        private BiQuadFilter[] filters;
        private List<int> sine = new List<int>();

        public void setValues(ISampleProvider sourceProvider, int cutOffFreq, int duration, int sampleRate, int channels, int inputSamplesSize)
        {
            Debug.WriteLine("FILTER: SET VALUES");
            this.sourceProvider = sourceProvider;
            this.cutOffFreq = cutOffFreq;
            this.channels = channels;
            this.duration = duration;
            this.sampleRate = sampleRate;
            this.inputSamplesSize = inputSamplesSize;

            Debug.WriteLine("FILTER: SET VALUES: source provider, cutoff freq, channels, duration, sample rate, samples count");

            double[] temp = GenerateSineWave(inputSamplesSize, amplitude);
            for (int i = 0; i < temp.Length; i++)
            {
                sine.Add((int)temp[i] + amplitude + offset);
            }
            Debug.WriteLine("FILTER: SET VALUES: created sine with length: " + sine.Count.ToString());

            setFilters();
        }

        private void setFilters()
        {
            Debug.WriteLine("FILTER: LOW PASS");
            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels];

            for (int n = 0; n < channels; n++)
                if (filters[n] == null)
                    filters[n] = BiQuadFilter.LowPassFilter(44100, cutOffFreq, 1);
                else
                    filters[n].SetLowPassFilter(44100, cutOffFreq, 1);
            Debug.WriteLine("FILTER: LOW PASS: created filters, lenght: " + filters.Length.ToString());
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public byte[] FilterFloatArray(float[] buffer, int offset, int count)
        {
            Debug.WriteLine("FILTER: READ");

            int samplesRead = sourceProvider.Read(buffer, offset, count);

            Debug.WriteLine("FILTER: READ: input (buffer) lenght before Transformation: " + buffer.Length.ToString());

            for (int i = 0; i < buffer.Length; i++)
            {
                filters[i % channels].SetLowPassFilter(44100, sine[sine.Count - 1 - i], 1);
                buffer[offset + i] = filters[(i % channels)].Transform(buffer[offset + i]);
            }

            Debug.WriteLine("FILTER: READ: input (buffer) lenght after Transformation: " + buffer.Length.ToString());

            //Buffer.BlockCopy(buffer, 0, byteArray, 0, byteArray.Length);
            
            List<byte> tempOutputList = new List<byte>();
            float min = 1;
            for (int i = 0; i < buffer.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(buffer[i]);
                for (int j = 0; j < temp.Length; j++)
                {
                    tempOutputList.Add(temp[j]);
                }
            }

            Debug.WriteLine("FILTER: READ: 'converting output to stereo' ");

            byte[] byteArray = new byte[tempOutputList.Count];

            for (int k = 0; k < tempOutputList.Count; k++)
            {
                if (k % 4 == 0 || k % 4 == 1)
                {
                    byteArray[k] = tempOutputList[k+2];
                }
                else
                {
                    byteArray[k] = tempOutputList[k];
                }
            }
            
            return byteArray;
        }

        private double[] GenerateSineWave(int size, int amplitude)
        {
            double[] buffer = new double[size * 4];
            double frequency = 0.2;
            for (int n = 0; n < buffer.Length; n++)
            {
                buffer[n] = (amplitude * Math.Sin((2 * Math.PI * n * frequency) / 44100));
            }

            return buffer;
        }
    }
}
