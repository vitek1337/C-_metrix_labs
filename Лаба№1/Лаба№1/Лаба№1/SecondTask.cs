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
    public partial class SecondTask : Form
    {
        private Random random = new Random();

        public SecondTask()
        {
            InitializeComponent();
        }

        private void SecondTask_Load(object sender, EventArgs e)
        {
        }

        private int SimulateX()
        {
            double r = random.NextDouble();

            if (r < 0.01)
                return 5;
            else if (r < 0.06)
                return 7;
            else if (r < 0.36)
                return 17;
            else if (r < 0.66)
                return 19;
            else if (r < 0.96)
                return 21;
            else if (r < 0.98)
                return 25;
            else
                return 55;
        }

        private void BuildChart(Dictionary<int, int> frequency, int numExperiments)
        {
            chart1.Series.Clear();

            Series series = new Series
            {
                Name = "Частота",
                IsVisibleInLegend = false,
                ChartType = SeriesChartType.Column
            };

            chart1.ChartAreas[0].AxisX.Title = "Значение X";
            chart1.ChartAreas[0].AxisY.Title = "Количество";
            chart1.ChartAreas[0].AxisX.Interval = 1;

            chart1.ChartAreas[0].AxisX.Minimum = 4;   
            chart1.ChartAreas[0].AxisX.Maximum = 56;  
            chart1.ChartAreas[0].AxisX.Interval = 1;

            chart1.ChartAreas[0].AxisX.CustomLabels.Clear();
            int[] possibleValues = { 5, 7, 17, 19, 21, 25, 55 };
            for (int i = 0; i < possibleValues.Length; i++)
            {
                chart1.ChartAreas[0].AxisX.CustomLabels.Add(possibleValues[i] - 0.5, possibleValues[i] + 0.5, possibleValues[i].ToString());
            }

            foreach (var kvp in frequency.OrderBy(k => k.Key))
            {
                series.Points.AddXY(kvp.Key, kvp.Value);
            }

            chart1.Series.Add(series);
            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int numExperiments;

            if (!int.TryParse(textBox1.Text, out numExperiments) || numExperiments <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное число экспериментов.");
                return;
            }

            Dictionary<int, int> frequency = new Dictionary<int, int>();

            int[] possibleValues = { 5, 7, 17, 19, 21, 25, 55 };
            foreach (int value in possibleValues)
            {
                frequency[value] = 0;
            }

            for (int i = 0; i < numExperiments; i++)
            {
                int outcome = SimulateX();
                frequency[outcome]++;
            }

            BuildChart(frequency, numExperiments);

        }
    }


}
