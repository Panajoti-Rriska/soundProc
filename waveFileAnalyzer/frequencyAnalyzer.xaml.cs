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
        private double xScale = 0;
        private int bins = 512; // guess a 1024 size FFT, bins is half FFT size
        private const int binsPerPoint = 2;
        private const int samplingFrequency = 44100;
        private bool calculated = false;

        private int updateCount;
        private List<double> dbListX = new List<double>();
        private List<double> dbListY = new List<double>();
        private Dictionary<double, double> dataDictionary = new Dictionary<double, double>();
        private List<Point> pointList = new List<Point>();
        private List<double> frequenciesList = new List<double>();
        private List<double> peaksList = new List<double>();
        private List<double> frequenciesDifferenciesList = new List<double>();


        public frequencyAnalyzer()
        {
            calculated = false;
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
           // return intensityDB;
            return intensityDB;
        }

        private void addResultsToGraph(int index, double power)
        {          
            Point p = new Point(calculateFrequency(index), power);

            pointList.Add(p);
            //As key we use dB and value we use Hz
            dataDictionary.Add(p.Y, p.X);
            dbListX.Add(p.X);
            dbListY.Add(p.Y);

        }

        private void showMeThePicks()
        {
            //var result = dbListY.OrderByDescending(x => x);
            
            //Finding peaks
            for (int i = 1; i < pointList.Count-1; i++)
            {
                if (Math.Floor(dbListY[i - 1]) < Math.Floor(dbListY[i]) && Math.Floor(dbListY[i + 1]) < Math.Floor(dbListY[i]))
                {
                    //threshold value
                    if (dbListY[i] > -40)
                    {
                        //var flooredDecibel = Math.Floor(dbListY[i]);
                        peaksList.Add(dbListY[i]);
                       // Console.WriteLine("Possible edges" + dbListY[i]);
                    }
                }
            }

            //Check the difference between pair peak frequencies and find the median which is the fundamental frequency
            for( int i = 0; i < peaksList.Count-1; i+=2)
            {
                //the pairs
                var frequencyDifferences =Math.Abs(dataDictionary[peaksList[i]] - dataDictionary[peaksList[i + 1]]);
                frequenciesDifferenciesList.Add(frequencyDifferences);
               // Console.WriteLine("Peaks compared " + peaksList[i] + " " + peaksList[i + 1] +" with difference of " + frequencyDifferences);
            }

            //Sort the array and find median which is the average of middle values
            if(frequenciesDifferenciesList.Count != 0)
            {
                frequenciesDifferenciesList.Sort();
                int size = frequenciesDifferenciesList.Count;
                int mid = size / 2;
                double median = 0;
                if(size % 2 == 0)
                {
                    median = (frequenciesDifferenciesList[mid] + frequenciesDifferenciesList[mid - 1]) / 2.0;
                }else
                {
                    median = frequenciesDifferenciesList[mid];
                }

                Console.WriteLine("The median is " + median);
            }

            dbListX.Clear();
            dbListY.Clear();
            peaksList.Clear();
            frequenciesDifferenciesList.Clear();
            dataDictionary.Clear();
            pointList.Clear();
            frequenciesList.Clear();
        }


        //When update count is 42 it ends.
        public void Update(Complex[] fftResults)
        {
            // no need to repaint too many frames per second
            if (updateCount++ % 2 == 0)
            {
                return;
            }

           // if (!calculated )
           // {
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

                //Draw the graph
                var frequencyDataSource = new EnumerableDataSource<Point>(pointList);
                frequencyDataSource.SetXMapping(x => x.X);
                frequencyDataSource.SetYMapping(y => y.Y);

                frequencyChart.AddLineGraph(frequencyDataSource, Colors.Blue, 2, "frequency");

                showMeThePicks();
               // updateCount = 0;
                calculated = true;
           // }
        }
    }
}
