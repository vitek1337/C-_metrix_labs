using System;
using System.Drawing;
using System.Windows.Forms;

namespace Лаба_7
{
    public partial class Form1 : Form
    {
        Random random;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = 7;
            dataGridView1.Columns[0].Name = "№";
            dataGridView1.Columns[1].Name = "r";
            dataGridView1.Columns[2].Name = "z";
            dataGridView1.Columns[3].Name = "ξ";
            dataGridView1.Columns[4].Name = "t_post";
            dataGridView1.Columns[5].Name = "t_osv";
            dataGridView1.Columns[6].Name = "Канал/Потеря";

            textBox5.Multiline = true;
            textBox5.ScrollBars = ScrollBars.Vertical;
            textBox5.Size = new Size(150, 150);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            double N = double.Parse(textBox1.Text);
            int v = int.Parse(textBox2.Text);
            double h = double.Parse(textBox3.Text);

            double t1 = N + 1;
            double t2 = N + 200;

            double lambda = 10.0 * (N + 1) / (N + 4);

            random = new Random();
            double currentTime = t1;
            double[] t_channel = new double[v];

            t_channel[0] = t1 + (-h * Math.Log(random.NextDouble()));
            t_channel[1] = t1 + (-h * Math.Log(random.NextDouble()));

            for (int i = 2; i < v; i++)
            {
                t_channel[i] = t1;
            }

            int Kvyz = 0;
            int Kpot = 0;

            while (currentTime < t2)
            {
                double r_z = random.NextDouble();
                double z = -Math.Log(r_z) / lambda;
                currentTime += z;
                if (currentTime > t2) break;

                Kvyz++;

                bool served = false;
                double r_xi = 0;
                double serviceTime = 0;

                for (int i = 0; i < v; i++)
                {
                    if (currentTime >= t_channel[i])
                    {
                        served = true;
                        r_xi = random.NextDouble();
                        serviceTime = -h * Math.Log(r_xi);
                        t_channel[i] = currentTime + serviceTime;

                        dataGridView1.Rows.Add(
                            Kvyz.ToString(),                  
                            r_z.ToString("F3"),            
                            z.ToString("F3"),               
                            serviceTime.ToString("F3"),      
                            currentTime.ToString("F3"),      
                            t_channel[i].ToString("F3"),        
                            (i + 1).ToString()                
                        );
                        break;
                    }
                }

                if (!served)
                {
                    Kpot++;
                    dataGridView1.Rows.Add(
                        Kvyz.ToString(),                 
                        r_z.ToString("F3"),              
                        z.ToString("F3"),                
                        "-",                             
                        currentTime.ToString("F3"),    
                        "-",                              
                        "Потеря"                        
                    );
                }
            }

            double P_sim = (double)Kpot / Kvyz;
            double P_Erl = CalculateErlangB(lambda, v);

            textBox5.Text = $"Всего вызовов (Kвыз) = {Kvyz}\r\n" +
                             $"Потеряно вызовов (Kпот) = {Kpot}\r\n";
            label4.Text = $"Модельная вероятность потери вызова: {P_sim:F4}";
            label5.Text = $"Pв по формуле Эрланга: {P_Erl:F4}";
        }

        public static double factorial(int n)
        {
            if (n == 0)
                return 1;
            else
                return n * factorial(n - 1);
        }

        public static double CalculateErlangB(double a, int c)
        {
            double sum = 0;
            for (int i = 0; i <= c; i++)
            {
                sum += Math.Pow(a, i) / factorial(i);
            }
            double B = (Math.Pow(a, c) / factorial(c)) / sum;
            return B;
        }
    }
}
