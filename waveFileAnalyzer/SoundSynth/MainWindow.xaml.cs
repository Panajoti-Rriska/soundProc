using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace SoundSynth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveOut waveOut; // represents a wave out devise
        private MixingSampleProvider mixer;
        private Dictionary<string, ISampleProvider> sounds = new Dictionary<string, ISampleProvider>();

        public MainWindow()
        {
            InitialPlayback();
            InitializeComponent();
        }

        private async Task StopSound(int duration, string name)
        {
            ISampleProvider mixerInput;
            await Task.Delay(duration);

            if (sounds.TryGetValue(name, out mixerInput))
            {
                //   mixer.RemoveMixerInput(mixerInput);
                mixer.RemoveAllMixerInputs();
                sounds.Remove(name);
            }
        }

        public void InitialPlayback()
        {
            waveOut = new WaveOut();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1));
            mixer.ReadFully = true;
            waveOut.Init(mixer);
            waveOut.Play();
        }

        private void StartSineWaveSound(float frequency, int duration, string nameOfSound)
        {
            var sawTooth = new SignalGenerator(44100, 1);
            sawTooth.Gain = 0.1f;
            sawTooth.Type = SignalGeneratorType.SawTooth;
            sawTooth.Frequency = frequency;

            var sineWaveProvide = new SineWaveProvider32();
            sineWaveProvide.SetWaveFormat(44100, 1); //16kHz
            sineWaveProvide.Frequency = frequency;
            sineWaveProvide.Amplitude = 0.35f;

            var data = new byte[26460];
            sineWaveProvide.Read(data, 0, 26460);
            var sampleStream = new RawSourceWaveStream(new MemoryStream(data), new WaveFormat(44100, 32, 1));
            var samppleProvider = sampleStream.ToSampleProvider();

            var filterSawToothSound = new LowPassFilterProvider(sawTooth);
            var filterSineWaveSound = new LowPassFilterProvider(sineWaveProvide.ToSampleProvider());
   

            //some dictionary fr saving our notes to remove them later
            if (!sounds.ContainsKey(nameOfSound))
            {
                sounds.Add(nameOfSound, sineWaveProvide.ToSampleProvider());        
                mixer.AddMixerInput(filterSawToothSound);
                mixer.AddMixerInput(filterSineWaveSound);
        
            }

            //Responsible for stopping Audio
            Task stopPlaying = this.StopSound(duration, nameOfSound);
        }

        private void StopWaveOut()
        {
            // mixer.RemoveAllMixerInputs();
            //  waveOut.Stop();
            // waveOut.Dispose();
            // waveOut = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(391.9f, 400, "tone1");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f, 400, "tone2");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f, 400, "tone3");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(349.6f, 400, "tone4");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(293.7f, 400, "tone5");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(293.7f, 400, "tone6");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(261.6f, 200, "tone7");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(329.6f, 200, "tone8");
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            StartSineWaveSound(391.9f, 400, "tone9");
        }
    }

    public abstract class WaveProvider32 : IWaveProvider
    {
        private WaveFormat wFormat;

        public WaveProvider32()
            : this(44100, 1)
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

            int samplesRequired = count / 4;
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
        private int sample;

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

            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));

                sample++;

                if (sample >= sampleRate)
                    sample = 0;
            }

            return sampleCount;
        }

    }
}