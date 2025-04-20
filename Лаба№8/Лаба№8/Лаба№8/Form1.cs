using System;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Лаба_8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            chart1.Series.Add(new Series("1 График"));
            chart1.Series.Add(new Series("2 График"));
            chart1.Series.Add(new Series("3 График"));
        }

        static double CalculateErlangB(int numberOfChannels, double trafficIntensity)
        {
            double inverseB = 1.0;
            double fact = 1.0;

            for (int i = 1; i <= numberOfChannels; i++)
            {
                fact *= i / trafficIntensity;
                inverseB += fact;
            }

            return 1.0 / inverseB;
        }

        static double ErlangB(double A, int c)
        {
            double sum = 1.0;
            double term = 1.0;

            for (int i = 1; i <= c; i++)
            {
                term *= A / i;
                sum += term;
            }

            return term / sum;
        }

        static int[] FindChannels(double A, double targetPw)
        {
            int c = (int)Math.Ceiling(A);
            int start = -1;
            while ((ErlangB(A, c) > targetPw) && (ErlangB(A, c) < 1))
            {

                c++;
                if (start == -1)
                {
                    start = c;
                }
            }
            return new int[] { start, c };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            dataGridView1.RowCount = 1;
            dataGridView2.RowCount = 1;
            dataGridView3.RowCount = 1;

            double gamma = Convert.ToDouble(textBox1.Text);
            int t = 10;

            double a1 = 10 * ((t + 1) / (double)(t + 4));
            double a2 = 15 * ((t + 1) / (double)(t + 4));
            double a3 = 20 * ((t + 1) / (double)(t + 4));

            chart1.Series[0].ChartType = SeriesChartType.Spline;
            chart1.Series[1].ChartType = SeriesChartType.Spline;
            chart1.Series[2].ChartType = SeriesChartType.Spline;

            for (int n = FindChannels(a1, gamma)[0]; n <= FindChannels(a1, gamma)[1]; n++)
            {
                double trafficIntensity = 10 * ((double)(n + 1) / (n + 4));
                double result = CalculateErlangB(n, trafficIntensity);
                chart1.Series[0].Points.AddXY(n, result);
                dataGridView1.Rows.Add(n, result);
            }

            for (int n = FindChannels(a2, gamma)[0]; n <= FindChannels(a2, gamma)[1]; n++)
            {
                double trafficIntensity = 15 * ((double)(n + 1) / (n + 4));
                double result = CalculateErlangB(n, trafficIntensity);
                chart1.Series[1].Points.AddXY(n, result);
                dataGridView2.Rows.Add(n, result);
            }

            for (int n = FindChannels(a3, gamma)[0]; n <= FindChannels(a3, gamma)[1]; n++)
            {
                double trafficIntensity = 20 * ((double)(n + 1) / (n + 4));
                double result = CalculateErlangB(n, trafficIntensity);
                chart1.Series[2].Points.AddXY(n, result);
                dataGridView3.Rows.Add(n, result);
            }
            int threadsForGamma1 = (int)Math.Round(1.5 * a1, 0);

            label2.Text = "Вероятность превышения очередью трёх вызовов: " + Math.Round(Math.Pow(a1 / threadsForGamma1, 4) * CalculateErlangB(threadsForGamma1, a1), 4);
            label3.Text = "Вероятность ожидания: " + Math.Round(CalculateErlangB(threadsForGamma1, a1), 4);
            label5.Text = "Среднее время ожидания для любого вызова: " + Math.Round(CalculateErlangB(threadsForGamma1, a1) * (threadsForGamma1 - a1), 4);
            label6.Text = "Средняя длина очереди: " + Math.Round((a1 * CalculateErlangB(threadsForGamma1, a1)) / (threadsForGamma1 - a1), 4);
        }
    }
}
