using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Math;
using System.IO;
using System.Diagnostics;

namespace task3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        String patternsDirectory = @"C:\Users\Maciek\Desktop\sound\speech_rec(1)\speech_rec\patterns_txt";
        NAudio.Wave.WaveChannel32 wave = null;
        NAudio.Wave.DirectSoundOut output = null;
        String waveFileName = null;
        List<double> samples = new List<double>();
        private double[][] blocks;
        String analyzedFile = "analyzed.txt";
        DTW dtw;
        String tag = "MAIN: ";

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void browse_btn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(tag + "loading wav file");

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave file(*.wav)|*.wav;";

            if (open.ShowDialog() != DialogResult.OK)
                return;

            waveFileName = open.FileName;
            label2.Text = open.SafeFileName;

            byte[] buffer = new byte[16384];
            int read = 0;

            DisposeWave();
            wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(waveFileName));

            while (wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);


                for (int i = 0; i < read / 4; i++)
                {
                    samples.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }
        }

        private void play_btn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(tag + "playing wav file");

            DisposeWave();
            wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(waveFileName));
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(wave));
            output.Play();
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }

            if (wave != null)
            {
                wave.Dispose();
                wave = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeWave();
        }

        private void find_btn_Click(object sender, EventArgs e)
        {
            MFCC cep = new MFCC();

            Debug.WriteLine(tag + "dividing samples into blocks");
            ConvertToBlocks();

            Debug.WriteLine(tag + "calculating mel-frequency cepstral coefficients");
            double[][] mfcc = cep.GetMFCC(blocks, 1024, 44100);

            Debug.WriteLine(tag + "saving analyzed coefficients to file");
            MFCCToFile(mfcc);

            Debug.WriteLine(tag + "comparing analyzed coefficients with patterns");
            CompareWithPatterns();
        }

        private void ConvertToBlocks()
        {
            double[] samp = new double[samples.Count / 2];
            double max = 0;

            for (int i = 0; i < samples.Count; i += 2)
            {
                if (Math.Abs(samples[i]) > max)
                {
                    max = Math.Abs(samples[i]);
                }
                samp[i / 2] = samples[i];
            }

            for (int i = 0; i < samp.Length; i++)
            {
                samp[i] = samp[i] * (1 / max);
            }

            blocks = DivideToBlocks(samp, 1024);
        }

        public static double[][] DivideToBlocks(double[] array, int sampRate)
        {
            int samplingRate = sampRate;
            if (IsPowerOfTwo(sampRate))
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

        public static bool IsPowerOfTwo(int n)
        {
            return ((n & (n - 1)) == 0) && n > 0;
        }

        private void MFCCToFile(double[][] melCoeff)
        {
            String entry = "";

            for (int i = 0; i < melCoeff.Length - 1; i++)
            {
                for (int j = 0; j < melCoeff[i].Length; j++)
                {
                    entry += melCoeff[i][j];
                    if (j != melCoeff[i].Length - 1)
                    {
                        entry += '\t';
                    }
                }
                entry += '\n';
            }
            File.WriteAllText(analyzedFile, entry);
        }

        private void CompareWithPatterns()
        {
            string[] filePaths = Directory.GetFiles(patternsDirectory);
            double minimal = 999999;
            String bestMatch = "None.";
            Dictionary<String, Double> filePathValue = new Dictionary<String, Double>();
            foreach (string file in filePaths)
            {
                String patternFile = file;
                dtw = new DTW(patternFile, analyzedFile, true);
                dtw.CalculatePath();
                if (dtw.GetMinimalPath() < minimal)
                {
                    minimal = dtw.GetMinimalPath();
                    bestMatch = file;
                }
                filePathValue.Add(file, dtw.GetMinimalPath());
                Debug.WriteLine(file + " result = " + dtw.GetMinimalPath());
            }
            Debug.WriteLine("BEST MATCH: " + bestMatch);
        }

    }
}
