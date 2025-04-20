using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_2
{
    public partial class FirstTask : Form
    {
        private Random random = new Random();


        public FirstTask()
        {
            InitializeComponent();
        }

        private void FirstTask_Load(object sender, EventArgs e)
        {

            textBox1.Text = "5";
            textBox2.Text = "2";
            textBox3.Text = "0,95";
            textBox4.Text = "-1000";
            textBox5.Text = "1000";
            textBox6.Text = "1000";

        }


        private double Gauss(double mean, double variance)
        {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * z;
        }


        private double[] GenerateCorrelatedProcess(double mean, double variance, double rho1, int n)
        {
            double[] process = new double[n];
            double[] whiteNoise = new double[n];
            for (int i = 0; i < n; i++)
            {
                whiteNoise[i] = Gauss(0, 1);
            }
            double phi = rho1;
            double noiseVariance = variance * (1 - phi * phi);
            double noiseStdDev = Math.Sqrt(noiseVariance);
            process[0] = mean + Math.Sqrt(variance) * whiteNoise[0];
            for (int i = 1; i < n; i++)
            {
                process[i] = mean + phi * (process[i - 1] - mean) + noiseStdDev * whiteNoise[i];
            }
            return process;
        }

        private double[] EstimateCorrelationFunctionLR1(double[] process, int mMin, int mMax, int n)
        {
            int range = mMax - mMin + 1;
            double[] R = new double[range];
            double mean = process.Average();
            double[] centeredProcess = new double[process.Length];
            for (int i = 0; i < process.Length; i++)
            {
                centeredProcess[i] = process[i] - mean;
            }
            for (int m = mMin; m <= mMax; m++)
            {
                double sum = 0;
                int count = 0;
                for (int i = Math.Max(0, -m); i < n && i + m < n; i++)
                {
                    sum += centeredProcess[i] * centeredProcess[i + m];
                    count++;
                }
                R[m - mMin] = count > 0 ? sum / count : 0;
            }
            double R0 = R[0 - mMin];
            if (R0 != 0)
            {
                for (int i = 0; i < range; i++)
                {
                    R[i] /= R0;
                }
            }
            return R;
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            double mean = double.Parse(textBox2.Text);
            double variance = double.Parse(textBox1.Text);
            double rho1 = double.Parse(textBox3.Text);
            int n = int.Parse(textBox6.Text);
            int mMin = int.Parse(textBox4.Text);
            int mMax = int.Parse(textBox5.Text);

            if (mMin >= mMax)
            {
                MessageBox.Show("mMin должен быть меньше mMax!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[] process = GenerateCorrelatedProcess(mean, variance, rho1, n);
            double[] R = EstimateCorrelationFunctionLR1(process, mMin, mMax, n);

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

            double minProcess = process.Min();
            double maxProcess = process.Max();
            double minR = R.Min();
            double maxR = R.Max();
            double combinedMin = Math.Min(minProcess, minR);
            double combinedMax = Math.Max(maxProcess, maxR);
            double margin = 0.1 * (combinedMax - combinedMin);
            double yAxisMin = combinedMin - margin;
            double yAxisMax = combinedMax + margin;
            if (Math.Abs(yAxisMax - yAxisMin) < 1e-6)
            {
                yAxisMin = 0;
                yAxisMax = 1;
            }
            areaMain.AxisY.Title = "Значения"; 
            areaMain.AxisY.Minimum = yAxisMin;
            areaMain.AxisY.Maximum = yAxisMax;
            double yInterval = CalculateNiceInterval(yAxisMax - yAxisMin, 10);
            areaMain.AxisY.Interval = yInterval;
            areaMain.AxisY.LabelStyle.Format = "F2";

            chart1.ChartAreas.Add(areaMain);
                
            Series seriesRealization = new Series("X(n)")
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2,
                XValueType = ChartValueType.Int32
            };
            for (int i = 0; i < n; i++)
            {
                seriesRealization.Points.AddXY(i + 1, process[i]);
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

            chart1.Titles.Add("Моделирование гауссовского процесса и его корреляционная функция");
            chart1.Titles[0].Position = new ElementPosition(0, 0, 100, 5);
        }

    }
}
