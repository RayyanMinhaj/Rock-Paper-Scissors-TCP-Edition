using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace CN_Project___Client__Player_2_
{
    public partial class Form1 : Form
    {
        private static Socket serverSocket;
        private static int win = 0;
        private static int loss = 0;
        private static string x="none";

        public static int i=1;
        public static int num_rounds = 0;
        //display prev record
        string disp_rec = "";

        //end variables
        /// /////////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
        }






        //a thread function to initiate UI change which indicate 
        //that you have selected the option and are waiting on OTHER player 
        private void waiting_player()
        {
            button5.Enabled = false;
            label7.Text = "";
            label8.Text = "(Waiting for other player)";


        }

        //SUBMIT BUTTON - same functionality as server side submit button
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Thread t1 = new Thread(waiting_player);
                t1.IsBackground = true;
                t1.Start();
                t1.Join();

                //Receive the message in a buffer and resize the buffer to fit the message
                string recvMessage = " ";
                byte[] recvBuffer = new byte[serverSocket.SendBufferSize];
                int bytesReceived = serverSocket.Receive(recvBuffer);
                Array.Resize(ref recvBuffer, bytesReceived);

                
                recvMessage = Encoding.ASCII.GetString(recvBuffer);


                byte[] sendData = Encoding.ASCII.GetBytes(x);
                serverSocket.Send(sendData);





                if (x == recvMessage)
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (DRAW)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    //incase of draw we increment both
                    win++;
                    loss++;

                }
                //winning conditions

                else if (x == "Rock" && recvMessage == "Scissor")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (WON)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    win++;
                }
                else if (x == "Paper" && recvMessage == "Rock")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (WON)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    win++;
                }
                else if (x == "Scissor" && recvMessage == "Paper")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (WON)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    win++;

                }
                //losing conditions
                else if (x == "Scissor" && recvMessage == "Rock")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (LOST)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    loss++;
                }
                else if (x == "Rock" && recvMessage == "Paper")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (LOST)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    loss++;
                }
                else if (x == "Paper" && recvMessage == "Scissor")
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (LOST)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    loss++;

                }
                else
                {
                    disp_rec = disp_rec + x + " VS " + recvMessage + " (INVALID)" + Environment.NewLine;

                    textBox4.Text = disp_rec;
                    //incase of invalid response we dont increment anything
                }


                label6.Text = "Round " + (i + 1);
                i++;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = false;
                label8.Text = "";
                //label7.Text = "";



                if (i == num_rounds+1)
                {
                    close_sckt();
                }

            }
            catch
            {
                MessageBox.Show("No connection established yet!");
            }
            
            


        }

        private void close_sckt()
        {
            int sum = 999;

            label6.Text = "Finished!";

            serverSocket.Close();

            if (win > loss)
            {
                textBox3.Text = "YOU WON!!!";
                sum = 1;
            }
            else if (win == loss)
            {
                textBox3.Text = "DRAW!!!";
                sum = 0;
            }
            else
            {
                textBox3.Text = "YOU LOST!!!";
                sum = -1;
            }

            textBox2.Text = "Not Connected";

            EndScreen scr = new EndScreen(sum);
            scr.ShowDialog();

            this.Close();
        }








        //CONNECT button
        private void button1_Click(object sender, EventArgs e)
        {
            int port;
            string[] address = textBox1.Text.Split(':');
            string ip = address[0];
            int.TryParse(address[1], out port);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);


            //Try to connect to the host and set the status to connected if it succeeds
            try
            {
                serverSocket.Connect(localEndPoint);
                textBox2.Text = "Connected";
                label6.Text = "Round 1";
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                //button5.Enabled = true;

                //recv num of rounds here
                byte[] recvBuffer = new byte[serverSocket.SendBufferSize];
                int bytesReceived = serverSocket.Receive(recvBuffer);
                string recvMessage = Encoding.ASCII.GetString(recvBuffer);

                //recvs num of rounds from server (player 1 - who inputs num of rounds)
                num_rounds=Convert.ToInt32(recvMessage);
                /////////////////////////

            }
            catch
            {
                MessageBox.Show("No connection found!");
                return;
            }
            
            
            

        }







        //The following 3 buttons are for selecting ROCK, PAPER OR SCISSOR
        //after selecting your option, the other options are blocked out
        //and it prompts you to click the submit button 
        private void button2_Click(object sender, EventArgs e)
        {
            x = "Rock";
            label6.Text = label6.Text + " (Locked in)";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            label7.Text = "press ---->";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            x = "Paper";
            label6.Text = label6.Text + " (Locked in)";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            label7.Text = "press ---->";

        }

        private void button4_Click(object sender, EventArgs e)
        {
             x = "Scissor";
             label6.Text = label6.Text + " (Locked in)";
             button2.Enabled = false;
             button3.Enabled = false;
             button4.Enabled = false;
             button5.Enabled = true;
             label7.Text = "press ---->";

        }







        //Before form loads, we start up the start screen form firstly
        private void Form1_Load(object sender, EventArgs e)
        {
            StartScreen scr = new StartScreen();
            scr.ShowDialog();

            button2.Enabled = false;
            button3.Enabled= false;
            button4.Enabled= false;
            button5.Enabled = false;
        }
    }
}
