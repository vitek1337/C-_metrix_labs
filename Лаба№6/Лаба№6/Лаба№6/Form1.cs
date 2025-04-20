using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_6
{
    public partial class Form1 : Form
    {
        Random random;

        Series series1;
        Series series2;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double V = Convert.ToDouble(textBox1.Text);
            double N = Convert.ToDouble(textBox2.Text);

            random = new Random();
            if (V != 0 && N != 0)
            {
                chart1.ChartAreas[0].AxisY.Title = "Pi";
                chart1.ChartAreas[0].AxisX.Title = "/\\";
                chart1.ChartAreas[0].AxisY.Maximum = 1;
                chart1.ChartAreas[0].AxisY.Interval = 0.1;

                

                double[] pi = new double[16];
                double[] pi2 = new double[16];

                for (int i = 0; i < 16; i++)
                {
                    pi[i] = pi1(V, i, V);
                    pi2[i] = pi1(V - 1, i, V);
                    if (pi[i] == pi2[i])
                    {
                        label9.Text = "Пересечение: X: " + i.ToString() + "; Y: " + pi[i].ToString("F3");
                    }
                }

                double Erl = 10 * (N + 1) / (N + 4);

                double PoVizovu = pi1(V, Erl, V);   
                double PoVremeni = pi1(V, Erl, V);   
                double PoNagruzke = pi1(V, Erl, V);    
                double Y = Erl * (1 - pi1(V, Erl, V));
                double R = Erl * pi1(V, Erl, V);
                double A = Erl;                 

                label3.Text = "Вероятность потери вызова Pb(λ): " + PoVizovu.ToString("F3");
                label4.Text = "Вероятность потерь по времени Pt(λ): " + PoVremeni.ToString("F3");
                label5.Text = "Вероятность потерь по нагрузке Pн(λ): " + PoNagruzke.ToString("F3");
                label6.Text = "Обслуженная нагрузка Y: " + Y.ToString("F3");
                label7.Text = "Избыточная нагрузка R: " + R.ToString("F3");
                label8.Text = "Потенциальная нагрузка A: " + A.ToString("F3");

                int[] num = new int[pi.Length];
                for (int i = 0; i < pi.Length; i++)
                {
                    num[i] = i + 1;
                }

                series1.Points.DataBindXY(num, pi);
                series2.Points.DataBindXY(num, pi2);
            }
        }

        public static double pi1(double i, double Erl, double v)
        {
            double sumErl = 0;
            double pi = 0;

            for (int j = 0; j <= (int)v; j++)
            {
                sumErl += Math.Pow(Erl, j) / factorial(j);
            }
            pi = (Math.Pow(Erl, i) / factorial((int)i)) / sumErl;

            return pi;
        }

        public static long factorial(long n)
        {
            if (n == 0)
                return 1;
            else
                return n * factorial(n - 1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            series1 = new Series();
            series1.BorderWidth = 4;
            series1.ChartType = SeriesChartType.Spline;
            series1.Name = "Pi";
            chart1.Series.Add(series1);

            series2 = new Series();
            series2.BorderWidth = 4;
            series2.ChartType = SeriesChartType.Spline;
            series2.Name = "Pi-1    ";
            chart1.Series.Add(series2);

        }
    }
}
