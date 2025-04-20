using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Лаба_2
{
    public partial class ChooseTask : Form
    {
        public ChooseTask()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FirstTask firstTask = new FirstTask();

            firstTask.ShowDialog();
            this.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            SecondTask secondTask = new SecondTask();

            secondTask.ShowDialog();
            this.Show();
        }
    }
}
