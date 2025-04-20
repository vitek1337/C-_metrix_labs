using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_1
{
    public partial class ThirdTask : Form
    {
        private Random random = new Random();

        int num = 100000;

        double mean = 3;
        double variance = 2;
        double aZ = 5;
        double bZ = 7;

        public ThirdTask()
        {
            InitializeComponent();
        }

        private void ThirdTask_Load(object sender, EventArgs e)
        {


            List<double> yValues = new List<double>();
            for (int i = 0; i < num; i++)
            {
                double x = NextGaussian(mean, variance);
                double z = GenerateUniform(aZ, bZ);
                double y = x + z;
                yValues.Add(y);
            }

            BuildChart(yValues);

        }

        private void BuildChart(List<double> data)
        {
            chart1.Series.Clear();

            Series series = new Series
            {
                Name = "Плотность",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Line,
                BorderWidth = 5
            };

            double min = data.Min();
            double max = data.Max();
            int numBins = 100;
            double binSize = (max - min) / numBins;
            double[] histogramData = new double[numBins];

            foreach (double value in data)
            {
                int binIndex = (int)((value - min) / binSize);
                if (binIndex < 0)
                    binIndex = 0;
                if (binIndex >= numBins)
                    binIndex = numBins - 1;
                histogramData[binIndex]++;
            }

            double totalCount = data.Count;
            for (int i = 0; i < numBins; i++)
            {
                histogramData[i] /= (totalCount * binSize);
            }

            for (int i = 0; i < numBins; i++)
            {
                double xValue = min + i * binSize + binSize / 2;
                series.Points.AddXY(xValue, histogramData[i]);
            }

            chart1.ChartAreas[0].AxisX.Title = "Y";
            chart1.ChartAreas[0].AxisY.Title = "Плотность";
            chart1.ChartAreas[0].AxisX.Minimum = min;
            chart1.ChartAreas[0].AxisX.Maximum = max;

            chart1.Series.Add(series);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            int num = int.Parse(textBox1.Text); 

            double mean = double.Parse(textBox2.Text);   
            double variance = double.Parse(textBox5.Text); 
            double aZ = double.Parse(textBox3.Text); 
            double bZ = double.Parse(textBox4.Text); 

            List<double> yValues = new List<double>();
            for (int i = 0; i < num; i++)
            {
                double x = NextGaussian(mean, variance);
                double z = GenerateUniform(aZ, bZ);
                double y = x + z;
                yValues.Add(y);
            }

            BuildChart(yValues);
        }

        private double NextGaussian(double mean, double variance)
        {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * randStdNormal;
        }

        private double GenerateUniform(double aZ, double bZ)
        {
            return aZ + random.NextDouble() * (bZ - aZ);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
