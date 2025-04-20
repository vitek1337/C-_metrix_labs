using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_4
{
    public partial class Task : Form
    {
        private Random rand = new Random();

        public Task()
        {
            InitializeComponent();
        }

        private double GenerateGaussian(double mean, double variance)
        {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * z;
        }

        private double ComputeExactMean(double mean, double variance)
        {

            int sampleSize = 100000;
            double sumX2 = 0.0;
            int countPositive = 0;

            for (int i = 0; i < sampleSize; i++)
            {
                double x = GenerateGaussian(mean, variance);
                if (x > 0)
                {
                    sumX2 += x * x;
                    countPositive++;
                }
            }

            double pPositive = countPositive / (double)sampleSize;
            double meanX2Pos = (countPositive > 0) ? sumX2 / countPositive : 0.0;
            double exactMean = pPositive * meanX2Pos;
            return exactMean;
        }

        private double ComputeExactVariance(double mean, double variance)
        {
            int sampleSize = 100000;
            double sumY = 0.0;
            double sumY2 = 0.0;

            for (int i = 0; i < sampleSize; i++)
            {
                double x = GenerateGaussian(mean, variance);
                double y = (x > 0) ? x : 0.0;
                sumY += y;
                sumY2 += y * y;
            }

            double mY = sumY / sampleSize;
            double mY2 = sumY2 / sampleSize;
            double exactVariance = mY2 - mY * mY;
            return exactVariance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int N = int.Parse(textBox2.Text);
                double mean = double.Parse(textBox1.Text);
                double variance = double.Parse(textBox3.Text);

                if (N <= 0)
                {
                    MessageBox.Show("Число отсчётов должно быть > 0!",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double exactValue = ComputeExactMean(mean, variance);

                double[] est = new double[N];
                double[] truth = new double[N];

                double x = GenerateGaussian(mean, variance);
                double y = (x > 0) ? x * x : 0.0;
                est[0] = y;            
                truth[0] = exactValue;    

                for (int i = 1; i < N; i++)
                {
                    x = GenerateGaussian(mean, variance);
                    y = (x > 0) ? x * x : 0.0;
                    est[i] = ((i - 1.0) / i) * est[i - 1] + (1.0 / i) * y;
                    truth[i] = exactValue;
                }

                DrawChart(est, truth, "Рекуррентная оценка M[Y]", "Истинное значение M[Y]",
                          "Оценка математического ожидания", "M[Y]");
            }
            catch (FormatException)
            {
                MessageBox.Show("Убедитесь, что в полях введены корректные числовые значения.",
                                "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int N = int.Parse(textBox2.Text);
                double mean = double.Parse(textBox1.Text);
                double variance = double.Parse(textBox3.Text);

                if (N <= 0)
                {
                    MessageBox.Show("Число отсчётов должно быть > 0!",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double exactValue = ComputeExactVariance(mean, variance);

                double[] est = new double[N];
                double[] truth = new double[N];

                double x = GenerateGaussian(mean, variance);
                double y = (x > 0) ? x : 0.0;
                double m = y;
                double s = y * y; 
                est[0] = 0.0;   
                truth[0] = exactValue;

                for (int i = 1; i < N; i++)
                {
                    x = GenerateGaussian(mean, variance);
                    y = (x > 0) ? x : 0.0;
                    m = ((i - 1.0) / i) * m + (1.0 / i) * y;
                    s = ((i - 1.0) / i) * s + (1.0 / i) * y * y;
                    est[i] = s - m * m;    
                    truth[i] = exactValue;
                }

                DrawChart(est, truth, "Рекуррентная оценка D[Y]", "Истинное значение D[Y]",
                          "Оценка дисперсии", "D[Y]");

            }
            catch (FormatException)
            {
                MessageBox.Show("Убедитесь, что в полях введены корректные числовые значения.",
                                "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawChart(double[] estimate, double[] exact,
                               string estSeriesName, string exactSeriesName,
                               string chartTitle, string yAxisTitle)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();

            ChartArea area;
            if (chart1.ChartAreas.Count == 0)
            {
                area = new ChartArea("MainArea");
                area.InnerPlotPosition = new ElementPosition(10, 5, 85, 80);
                chart1.ChartAreas.Add(area);

                area.AxisX.Title = "Число отсчётов (n)";
                area.AxisY.Title = yAxisTitle;
            }
            else
            {
                area = chart1.ChartAreas[0];
                area.AxisY.Title = yAxisTitle;
            }

            Series seriesEst = new Series(estSeriesName)
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };
            Series seriesExact = new Series(exactSeriesName)
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2
            };

            int N = estimate.Length;
            for (int i = 0; i < N; i++)
            {
                seriesEst.Points.AddXY(i, estimate[i]);
                seriesExact.Points.AddXY(i, exact[i]);
            }

            chart1.Series.Add(seriesEst);
            chart1.Series.Add(seriesExact);


            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = N - 1;

            area.AxisX.Interval = Math.Max(1, N / 10);

            area.AxisX.LabelStyle.Format = "F0"; 

            double maxVal = Math.Max(estimate.Max(), exact.Max());
            double minVal = Math.Min(estimate.Min(), exact.Min());
            double margin = (maxVal - minVal) * 0.1;
            area.AxisY.Minimum = minVal - margin;
            area.AxisY.Maximum = maxVal + margin;
            area.AxisY.LabelStyle.Format = "F2";

            Title title = new Title(chartTitle, Docking.Top, new Font("Arial", 10F), Color.Black);
            chart1.Titles.Add(title);
        }
    }
}
