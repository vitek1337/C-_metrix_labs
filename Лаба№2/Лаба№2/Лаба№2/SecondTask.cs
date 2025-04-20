using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_2
{
    public partial class SecondTask : Form
    {
        private Random rand = new Random();

        public SecondTask()
        {
            InitializeComponent();
        }

        private void SecondTask_Load(object sender, EventArgs e)
        {
            textBox1.Text = "1000";  
            textBox2.Text = "7";      
            textBox3.Text = "0,05";   
            textBox4.Text = "-1000";   
            textBox5.Text = "1000";     

        }

        private double Gauss(double mean, double variance, Random rand)
        {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * z;
        }

        private double[] GenerateCorrelatedProcess(double variance, double a, int n, Random rand)
        {
            int burnIn = 20000;
            int totalLength = n + burnIn;
            double[] process = new double[totalLength];
            double[] whiteNoise = new double[totalLength];
            for (int i = 0; i < totalLength; i++)
                whiteNoise[i] = Gauss(0, 1, rand);
            double rho1 = Math.Exp(-a);
            double phi = rho1;
            double noiseVariance = variance * (1 - phi * phi);
            double noiseStdDev = Math.Sqrt(noiseVariance);
            process[0] = Math.Sqrt(variance / (1 - phi * phi)) * Gauss(0, 1, rand);
            for (int i = 1; i < 100; i++)
                process[i] = phi * process[i - 1] + noiseStdDev * whiteNoise[i];
            for (int i = 100; i < totalLength; i++)
                process[i] = phi * process[i - 1] + noiseStdDev * whiteNoise[i];
            double[] finalProcess = new double[n];
            Array.Copy(process, burnIn, finalProcess, 0, n);
            double empiricalMean = finalProcess.Average();
            double empiricalVariance = finalProcess.Select(x => Math.Pow(x - empiricalMean, 2)).Average();
            return finalProcess;
        }

        private double[] GenerateProcessY(double[] X, double b, int n)
        {
            double[] Y = new double[n];
            Y[0] = X[0];
            for (int i = 1; i < n; i++)
                Y[i] = X[i] - b * X[i - 1];
            double empiricalMean = Y.Average();
            double empiricalVariance = Y.Select(y => Math.Pow(y - empiricalMean, 2)).Average();
            return Y;
        }

        private double[] EstimateCorrelationFunction(double[] process, int mMin, int mMax)
        {
            int range = mMax - mMin + 1;
            double[] R = new double[range];
            double mean = process.Average();
            double[] centeredProcess = process.Select(x => x - mean).ToArray();
            for (int m = mMin; m <= mMax; m++)
            {
                double sum = 0.0;
                int count = 0;
                for (int i = Math.Max(0, -m); i < process.Length && i + m < process.Length; i++)
                {
                    sum += centeredProcess[i] * centeredProcess[i + m];
                    count++;
                }
                R[m - mMin] = count > 0 ? sum / count : 0;
            }
            double R0 = R[0 - mMin];
            if (R0 != 0)
                for (int i = 0; i < range; i++)
                    R[i] /= R0;
            return R;
        }

        double CalculateNiceInterval(double range, int ticksDesired)
        {
            double tempInterval = range / ticksDesired;
            double exponent = Math.Floor(Math.Log10(tempInterval));
            double fraction = tempInterval / Math.Pow(10, exponent);
            double niceFraction;
            if (fraction < 1.5)
                niceFraction = 1;
            else if (fraction < 3)
                niceFraction = 2;
            else if (fraction < 7)
                niceFraction = 5;
            else
                niceFraction = 10;
            return niceFraction * Math.Pow(10, exponent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
          

            Random rand = new Random();
            int n = int.Parse(textBox1.Text);
            int mMin = int.Parse(textBox4.Text);
            int mMax = int.Parse(textBox5.Text);
            double D = double.Parse(textBox2.Text);
            double a = double.Parse(textBox3.Text);
            double b = Math.Exp(-a);

            if (mMin >= mMax)
            {
                MessageBox.Show("mMin должен быть меньше mMax.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[] X = GenerateCorrelatedProcess(D, a, n, rand);
            double[] Y = GenerateProcessY(X, b, n);
            double[] R = EstimateCorrelationFunction(Y, mMin, mMax);

            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            ChartArea areaMain = new ChartArea("MainArea");
            areaMain.InnerPlotPosition = new ElementPosition(10, 5, 85, 85);

            double xMin = 0;
            double xMax = Math.Max(n, mMax);
            areaMain.AxisX.Title = "Номер эксперимента";
            areaMain.AxisX.Minimum = xMin;
            areaMain.AxisX.Maximum = xMax;
            double xInterval = CalculateNiceInterval(xMax - xMin, 10);
            areaMain.AxisX.Interval = xInterval;
            areaMain.AxisX.LabelStyle.Format = "F0";

            double minY_Value = Y.Min();
            double maxY_Value = Y.Max();
            double minR_Value = R.Min();
            double maxR_Value = R.Max();

            double combinedMin = Math.Min(minY_Value, minR_Value);
            double combinedMax = Math.Max(maxY_Value, maxR_Value);

            double margin = 0.1 * (combinedMax - combinedMin);
            areaMain.AxisY.Title = "Значения";
            areaMain.AxisY.Minimum = combinedMin - margin;
            areaMain.AxisY.Maximum = combinedMax + margin;
            double yInterval = CalculateNiceInterval(areaMain.AxisY.Maximum - areaMain.AxisY.Minimum, 10);
            areaMain.AxisY.Interval = yInterval;
            areaMain.AxisY.LabelStyle.Format = "F2";

            chart1.ChartAreas.Add(areaMain);

            Series seriesRealization = new Series("Y(n)")
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2,
                XValueType = ChartValueType.Int32
            };
            for (int i = 0; i < n; i++)
            {
                seriesRealization.Points.AddXY(i + 1, Y[i]);
            }

            Series seriesCorrelation = new Series("R(m)")
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                XValueType = ChartValueType.Int32
            };
            for (int m = mMin; m <= mMax; m++)
            {
                seriesCorrelation.Points.AddXY(m, R[m - mMin]);
            }

            chart1.Series.Add(seriesRealization);
            chart1.Series.Add(seriesCorrelation);

            if (chart1.Legends.Count == 0)
            {
                chart1.Legends.Add(new Legend("Legend1"));
                chart1.Legends[0].Docking = Docking.Bottom;
                chart1.Legends[0].Alignment = StringAlignment.Center;
            }

            chart1.Titles.Add("Моделирование процесса Y(n) и его корреляционная функция");
            chart1.Titles[0].Position = new ElementPosition(0, 0, 100, 5);
        }

    }
}
