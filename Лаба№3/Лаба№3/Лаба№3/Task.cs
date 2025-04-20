using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_3
{
    public partial class Task : Form
    {
        private Random random = new Random();

        public Task()
        {
            InitializeComponent();
        }

        private void Task_Load(object sender, EventArgs e)
        {

        }

        private double Gauss(double mean, double variance)
        {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + Math.Sqrt(variance) * z;
        }

        private double[] GenerateWhiteNoise(int n)
        {
            double[] noise = new double[n];
            for (int i = 0; i < n; i++)
            {
                noise[i] = Gauss(0, 1);
            }
            return noise;
        }

        private double[] Generate(double dt, int length)
        {
            double[] k = new double[length];
            double[] window = new double[length];

            for (int i = 0; i < length; i++)
            {
                double t = i * dt;
                if (t >= 0 && t <= 5)
                {
                    k[i] = 0.2 * Math.Cos(0.5 * t + 7);

                    window[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (length - 1)));

                    k[i] *= window[i] * 0.1;
                }
                else
                {
                    k[i] = 0;
                }
            }
            return k;
        }

        private double[] Convolute(double[] x, double[] k)
        {
            int n = x.Length;
            int l = k.Length;
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int p = 0; p < l; p++)
                {
                    int idx = i - p;
                    if (idx >= 0 && idx < n)
                    {
                        sum += x[idx] * k[p];
                    }
                }
                y[i] = sum;
            }
            return y;
        }
        private void SetOptimalAxisRangeY(Chart chart, double marginFactor)
        {
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (Series s in chart.Series)
            {
                foreach (DataPoint p in s.Points)
                {
                    double y = p.YValues[0];
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }

            double yMargin = (maxY - minY) * marginFactor;

            ChartArea area = chart.ChartAreas[0];
            area.AxisY.Minimum = minY - yMargin;
            area.AxisY.Maximum = maxY + yMargin;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int N = int.Parse(textBox1.Text);  
                double dt = double.Parse(textBox2.Text); 

                if (N < 1)
                {
                    MessageBox.Show("Длина входной реализации должна быть больше 0!",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (dt <= 0)
                {
                    MessageBox.Show("Шаг дискретизации должен быть положительным!",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double[] x = GenerateWhiteNoise(N);

                int L = (int)(5.0 / dt) + 1;
                double[] k = Generate(dt, L);

                double[] y = Convolute(x, k);

                chart1.Series.Clear();

                Series seriesX = new Series("Input Noise")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Blue,
                    BorderWidth = 2
                };

                Series seriesY = new Series("Output Signal")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Red,
                    BorderWidth = 2
                };

                for (int i = 0; i < N; i++)
                {
                    int t = i + 1;
                    seriesX.Points.AddXY(t, x[i]);
                    seriesY.Points.AddXY(t, y[i]);
                }

                chart1.Series.Add(seriesX);
                chart1.Series.Add(seriesY);

                ChartArea area = chart1.ChartAreas[0];
                area.AxisX.Title = "Номер эксперимента";
                area.AxisY.Title = "Амплитуда";

                area.AxisX.Minimum = 1;
                area.AxisX.Maximum = N;

                area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                area.AxisX.LabelStyle.Format = "F0";
                area.AxisY.LabelStyle.Format = "F2";

                SetOptimalAxisRangeY(chart1, 0.05);
            }
            catch (FormatException)
            {
                MessageBox.Show("Убедитесь, что во все поля введены корректные числовые значения.",
                                "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
