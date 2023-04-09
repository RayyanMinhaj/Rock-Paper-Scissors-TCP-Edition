using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CN_Project___Client__Player_2_
{
    public partial class EndScreen : Form
    {
        public static int x = 0;
        public EndScreen(int sum)
        {
            InitializeComponent();
            x = sum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EndScreen_Load(object sender, EventArgs e)
        {
            if (x == 1)
            {
                label3.Text = "You Won :)";
            }
            else if (x == -1)
            {
                label3.Text = "You Lost :(";
            }
            else if (x == 0)
            {
                label3.Text = "Draw :|";
            }
            else
            {
                label3.Text = "Invalid result >:(";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

            StartScreen scr=new StartScreen();  
            scr.Show();   
        }
    }
}
