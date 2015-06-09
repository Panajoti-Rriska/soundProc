using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using NAudio.Dsp;
using NAudio.Wave;

namespace SoundSynth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveOut waveOut; // represents a wave out devise
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task StopSound(int duration)
        {
            await Task.Delay(duration);
            this.StopWaveOut(waveOut);
        }

        private void StartSineWaveSound(float frequency, int duration)
        {
            if(waveOut == null)
            {
                var sineWaveProvide = new SineWaveProvider32();
                sineWaveProvide.SetWaveFormat(16000, 1); //16kHz 
                sineWaveProvide.Frequency = frequency;
                sineWaveProvide.Amplitude = 0.20f;
                waveOut = new WaveOut();
                
                waveOut.Init(sineWaveProvide);
                              
                waveOut.Play();
                Task stopPlaying = this.StopSound(duration);
                
            }else
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

        }

        private void StopWaveOut(WaveOut waveOut)
        {
             waveOut.Stop();
             waveOut.Dispose();
             waveOut = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            StartSineWaveSound(391.9f,400);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f,400);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f,400);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(349.6f,400);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(293.7f,400);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(293.7f,400);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(261.6f,200);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f,200);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(391.9f,400);
        }
    }

    public abstract class WaveProvider32 : IWaveProvider
    {
        private WaveFormat wFormat;

        public WaveProvider32() : this(44100, 1)
        { }

        public WaveProvider32(int sampleRate, int channels)
        {
            
        }

        public void SetWaveFormat(int sampleRate, int channels)
        {
            this.wFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        }

        public abstract int Read(float[] buffer, int offset, int sampleCount);

        public int Read(byte[] buffer, int offset, int count)
        {

            WaveBuffer waveBuffer = new WaveBuffer(buffer);

            int samplesRequired = count /4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);

            return samplesRead * 4;
        }

        public WaveFormat WaveFormat
        {
            get { return wFormat; }
        }
        

    }

    public class SineWaveProvider32 : WaveProvider32
    {
        int sample;
        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f; // care for the ears     
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;

            for(int n = 0; n < sampleCount; n++)
            {

                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));

                sample++;

                if(sample>= sampleRate)
                    sample = 0;
            }

            return sampleCount;
        }
    }
}
