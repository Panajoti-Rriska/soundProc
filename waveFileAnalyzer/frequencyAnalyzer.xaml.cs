using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAudio.Dsp;
using System.Windows.Media; // Pen

using System.IO;
using Microsoft.Research.DynamicDataDisplay; // Core functionality
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers; // CirclePointMarker


namespace NAudioWpfDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class frequencyAnalyzer : UserControl
    {
        private double xScale = 43;
        private int bins = 512; // guess a 1024 size FFT, bins is half FFT size
        private const int binsPerPoint = 2;
        private const int samplingFrequency = 44100;

        private int updateCount;
        private List<double> dbListX = new List<double>();
        private List<double> dbListY = new List<double>();
        private List<Point> pointList = new List<Point>();
        private List<double> frequenciesList = new List<double>();
        public frequencyAnalyzer()
        {
            InitializeComponent();
            CalculateXScale();
            //this.SizeChanged += SpectrumAnalyser_SizeChanged;
        }

        //x scale of the graph
        private void CalculateXScale()
        {
            this.xScale = (samplingFrequency / bins);
        }

        void SpectrumAnalyser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateXScale();
        }

        private double calculateFrequency(int bin)
        {
            if (bin == 0) return 0;
            return bin * xScale; 
        }

        private double calculateDB(Complex c)
        {
            //Magnitude 
            double intensityDB = 10 * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            frequenciesList.Add(intensityDB);
            double minDB = -90;

            //Checking if its below minimum
            if (intensityDB < minDB) intensityDB = minDB;
           // double percent = intensityDB; /// minDB;
            // we want 0dB to be at the top (i.e. yPos = 0)

            //double yPos = percent;//* this.ActualHeight;
            //return yPos;
            return intensityDB;
        }

        private void addResultsToGraph(int index, double power)
        {          
            Point p = new Point(calculateFrequency(index), power);

            pointList.Add(p);
            dbListX.Add(p.X);
            dbListY.Add(p.Y);

        }

        private void showMeThePicks()
        {
            var result = dbListY.OrderByDescending(x => x);
                
            foreach(var item in result)
            {
                Console.WriteLine(item);
            }
                
                //Console.WriteLine(pointList[i].X + " Frequencies");
                //Console.WriteLine(pointList[i].Y + " DB");
     
        }


        //When update count is 42 it ends.
        public void Update(Complex[] fftResults)
        {
            // no need to repaint too many frames per second
            if (updateCount++ % 2 == 0)
            {
                return;
            }


            //End of song
            if (updateCount >= 42)
            {
                //Calculate fft results at the end of the sound file

                if (fftResults.Length / 2 != bins)
                {
                    this.bins = fftResults.Length / 2;
                    CalculateXScale();
                }

                for (int n = 0; n < fftResults.Length / 2; n += binsPerPoint)
                {
                    // averaging out bins
                    double db = 1;
                    for (int b = 0; b < binsPerPoint; b++)
                    {
                        db += calculateDB(fftResults[n + b]);
                    }
                    addResultsToGraph(n / binsPerPoint, db / binsPerPoint);
                }

                var frequencyDataSource = new EnumerableDataSource<Point>(pointList);
                frequencyDataSource.SetXMapping(x => x.X);
                frequencyDataSource.SetYMapping(y => y.Y);

                frequencyChart.AddLineGraph(frequencyDataSource, Colors.Blue, 2, "frequency");
                Console.WriteLine("This is the max db or whatever is called "+frequenciesList.Max());

                showMeThePicks();
              /*  var decibelOpenDataSource = new EnumerableDataSource<double>(dbListY);
                decibelOpenDataSource.SetYMapping(x => x);

                CompositeDataSource compositeDataSource1 = new
                CompositeDataSource(frequencyDataSource, decibelOpenDataSource);

                frequencyChart.AddLineGraph(compositeDataSource1,
                 new Pen(Brushes.Blue, 1),
                 new CirclePointMarker { Size = 0.1, Fill = Brushes.Blue },
                 new PenDescription("Line"));

                frequencyChart.FitToView();*/

                //updateCount = 0;
            }
        }
    }
}
