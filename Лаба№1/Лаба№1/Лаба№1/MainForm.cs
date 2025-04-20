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
    public partial class MainForm : Form
    {
        private double a = -2;
        private double b = 7;
        private int bins = 100;

        public MainForm()
        {
            InitializeComponent();

            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.AxisX.Title = "Значения X";
            chartArea.AxisY.Title = "Плотность появления";
            chartArea.AxisX.Minimum = a;
            chartArea.AxisX.Maximum = b;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisY.LabelStyle.Format = "0.###";

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = "-2";    
            textBox2.Text = "7";
            textBox3.Text = "100";

            chart1.Series.Clear();
            GenerateSeries(100, Color.Blue);
            GenerateSeries(1000, Color.Red);
            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private List<double> GenerateRandomValues(int n)
        {
            Random random = new Random();
            List<double> values = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double value = a + random.NextDouble() * (b - a);
                values.Add(value);
            }
            return values;
        }

        private double[] CalculateHistogram(List<double> data)
        {
            double binSize = (b - a) / bins;
            double[] histogram = new double[bins];

            foreach (var value in data)
            {
                int binIndex = (int)((value - a) / binSize);
                if (binIndex < 0)
                    binIndex = 0;
                if (binIndex >= bins)
                    binIndex = bins - 1;
                histogram[binIndex]++;
            }

            double totalCount = data.Count;
            for (int i = 0; i < bins; i++)
            {
                histogram[i] /= (totalCount * binSize);
            }
            return histogram;
        }

        private void GenerateSeries(int experimentCount, Color color)
        {
            List<double> values = GenerateRandomValues(experimentCount);
            double[] histogram = CalculateHistogram(values);

            Series series = new Series()
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = color,
                Name = experimentCount.ToString() 
            };

            double binSize = (b - a) / bins;
            for (int i = 0; i < bins; i++)
            {
                double xValue = a + i * binSize + binSize / 2;
                series.Points.AddXY(xValue, histogram[i]);
            }
            chart1.Series.Add(series);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            double newA = double.Parse(textBox1.Text);
            double newB = double.Parse(textBox2.Text);

            int experimentCount = int.Parse(textBox3.Text);

            this.a = newA;
            this.b = newB;

            try
            {
                int experimentCount2 = int.Parse(textBox4.Text);
                GenerateSeries(experimentCount, Color.Blue);
                GenerateSeries(experimentCount2, Color.Red);
            } catch (FormatException)
            {
                GenerateSeries(experimentCount, Color.Blue);
            }

            chart1.ChartAreas[0].AxisX.Minimum = newA;
            chart1.ChartAreas[0].AxisX.Maximum = newB;
            chart1.ChartAreas[0].RecalculateAxesScale();

        }

        private void button2_Click(object sender, EventArgs e)
        {
           


        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
