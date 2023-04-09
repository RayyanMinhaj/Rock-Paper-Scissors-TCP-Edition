using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CN_Project___Server__Player_1_
{
    public partial class StartScreen : Form
    {
        public int rounds { get; set; }
        public StartScreen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.rounds=Convert.ToInt32(textBox1.Text);
            if (this.rounds > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter correct value!");
            }
            

        }
    }
}
