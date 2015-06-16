using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ocean_generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int duration = 20; //duration
        private int sampleRate = 44100; //sample rate
        private int channels = 2; //channels number
        private int LFOAmplitude = 800; //LFO Amplitude
        private float Q = 0.8f; //resonance
        private int LFOMinVal = 300; //minimum cutoff frequency value for low pass filter

        private void generate_button_Click(object sender, EventArgs e)
        {
            lowpass f = new lowpass();

            List<float> samples = WhiteNoiseGen();

            float[] sineWave = GenerateSineWave(samples.Count, LFOAmplitude);

            List<float> output = new List<float>();

            for (int i = 0; i < samples.Count; i++)
            {
                f.Setup(sineWave[i] + LFOAmplitude + LFOMinVal, 44100, Q, lowpass.PassType.Lowpass);
                f.Update(samples[i]);
                output.Add(f.Value);
            }

            byte[] byteOut = byteConverter(output); 

            playByteArray(byteOut);
        }

        private List<float> WhiteNoiseGen()
        {
            int samples = duration * sampleRate * channels * 2;
            List<float> rawsamples = new List<float>();
            Random rnd1 = new System.Random();

            for (int i = 0; i < samples; i++)
            {
                rawsamples.Add((float)rnd1.Next(-100,100));
                rawsamples[i] /= 100;
            }
            return rawsamples;
        }

        private void playByteArray(byte[] input)
        {
            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(input), new WaveFormat(sampleRate, channels));

            WaveOut wo = new WaveOut();
            wo.Init(provider);
            wo.Play();
        }

        private void SaveByteArray(String path, byte[] source)
        {
            using (WaveFileWriter writer = new WaveFileWriter(path, new WaveFormat(sampleRate, channels)))
            {
                writer.Write(source, 0, source.Length);
            }
        }

        private float[] GenerateSineWave(int size, int amplitude)
        {
            float[] buffer = new float[size * 4];
            double frequency = 0.05;
            for (int n = 0; n < buffer.Length; n++)
            {
                buffer[n] = (float)(amplitude * Math.Sin((2 * Math.PI * n * frequency) / 44100));
            }

            return buffer;
        }

        private byte[] byteConverter(List<float> input)
        {
            byte[] buffer = new byte[input.Count];
            for (int i = 0; i < input.Count; i++)
            {
                int val = (int)(input[i] * 128.0 + 0.5);
                if (val > 127)
                    val = 127;
                else if (val < -128)
                    val = -128;
                buffer[i] = (byte)(val);
            }
            return buffer;
        }
    }
}
