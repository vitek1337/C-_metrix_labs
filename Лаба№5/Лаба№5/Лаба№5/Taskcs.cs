using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Лаба_5
{
    public partial class Taskcs : Form
    {
        private Random random = new Random();

        public Taskcs()
        {
            InitializeComponent();
            BuildChart();
        }

        private void BuildChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "Номер интервала";
            dataGridView1.Columns[1].Name = "val1";
            dataGridView1.Columns[2].Name = "val2";
            dataGridView1.Columns[3].Name = "valSum";
            chart1.ChartAreas.Add(new ChartArea("MainArea"));

            AddSeries("val1", Color.Red);
            AddSeries("val2", Color.Blue);
            AddSeries("valSum", Color.Black);
        }

        private void AddSeries(string seriesName, Color color)
        {
            var newSeries = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                Color = color,
                BorderWidth = 2
            };
            chart1.Series.Add(newSeries);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            foreach (var s in chart1.Series)
            {
                s.Points.Clear();
            }

            int N = (int)numericUpDown1.Value;

            double rate1 = 10.0 * (N + 1) / (N + 4);
            double rate2 = 15.0 * (N + 1) / (N + 4);

            double intervalLength = 1.0; 

            int[] values1 = new int[24];
            int[] values2 = new int[24];

            double time = 0.0;
            while (time < 24)
            {
                double r = random.NextDouble();
                double deltaT = -Math.Log(r) / rate1;
                time += deltaT;
                int intervalIndex = (int)time;
                if (intervalIndex < 24)
                {
                    values1[intervalIndex]++;
                }
            }

            time = 0.0;

            while (time < 24)
            {
                double r = random.NextDouble();
                double deltaT = -Math.Log(r) / rate2;
                time += deltaT;
                int intervalIndex = (int)time;
                if (intervalIndex < 24)
                {
                    values2[intervalIndex]++;
                }
            }

            int[] valuesSum = new int[24];
            for (int i = 0; i < 24; i++)
            {
                valuesSum[i] = values1[i] + values2[i];
            }

            for (int i = 0; i < 24; i++)
            {
                dataGridView1.Rows.Add(i + 1, values1[i], values2[i], valuesSum[i]);
            }

            for (int i = 0; i < 24; i++)
            {
                chart1.Series["val1"].Points.AddXY(i + 1, values1[i]);
                chart1.Series["val2"].Points.AddXY(i + 1, values2[i]);
                chart1.Series["valSum"].Points.AddXY(i + 1, valuesSum[i]);
            }

            double averageSum = valuesSum.Average();
            double lambdaSum = averageSum / intervalLength;

            if (chart1.Legends.Count == 0)
            {
                chart1.Legends.Add(new Legend("Legend1"));
                chart1.Legends[0].Docking = Docking.Bottom;
                chart1.Legends[0].Alignment = StringAlignment.Center;
            }

            label2.Text = $"λ_sum(модельное) = {lambdaSum:F2}, λ1 + λ2 = {(rate1 + rate2):F2}";

            ChartArea area = chart1.ChartAreas[0];
            area.AxisX.Minimum = 1;
            area.AxisX.Maximum = 24;
            area.AxisX.Interval = Math.Max(1, 24 / 10.0);
            area.AxisX.LabelStyle.Format = "F0";

            double maxVal = Math.Max(valuesSum.Max(), Math.Max(values1.Max(), values2.Max()));
            double minVal = 0; 
            double margin = (maxVal - minVal) * 0.1;
            area.AxisY.Minimum = minVal;
            area.AxisY.Maximum = maxVal + margin;
            area.AxisY.LabelStyle.Format = "F0";
        }
    }
}
