using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace CN_Project___Server__Player_1_
{
    public partial class Form1 : Form
    {
        
        private static string x="none";

        //this win variable is totally independent of the client sides win variable
        //in the sense that we dont need client and server to COMPARE each others win rounds
        //they can just compare the won and loss inside of themselves
        private static int win = 0;
        private static int loss = 0;

        private static Socket listenerSocket;
        private static Socket clientSocket;

        //iterator for determining rounds
        public static int i = 1;
        public static int num_rounds = 0;

        //display prev record
        string disp_rec = "";

        //end variables
        ///////////////////////////////////////////////////////////////////////////////////////
        
        public Form1()
        {
            InitializeComponent();
        }
        //no use
        private void label2_Click(object sender, EventArgs e)
        {

        }






        //thread func which establishes connection 
        private void threadfunc(object port)
        {
            //Create new server socket to listen for incoming connections
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            //Binds this socket to the port that was entered, and listens for connections (blocking call)
            try
            {
                listenerSocket.Bind(new IPEndPoint(IPAddress.Any, (int)port));
                listenerSocket.Listen(0);
            }
            catch
            {
                MessageBox.Show("This port is already in use, please enter another.");
                return;
            }

            //Accept an incoming connection and passes it to clientSocket if accepted.
            try
            {
                clientSocket = listenerSocket.Accept();
            }
            catch
            {
                MessageBox.Show("Received no connection!");
                listenerSocket.Close();
                return;
            }
            

            

        }

        //start listening button
        private void button1_Click(object sender, EventArgs e)
        {

            textBox2.Text = "Waiting for connection";


            
            //Extracts the port number from the text box and stores it in 'port'
            int port;
            Int32.TryParse(textBox1.Text, out port);

            //creates a new thread so that we can change UI elements in this event handler because
            //we can not alter labels/textboxes while blocking call is running
            //(vice versa is also possible)
            Thread t1 = new Thread(threadfunc);
            t1.IsBackground = true;
            t1.Start(port);
            t1.Join();

            //blocking user from making any decisions till connection established
            textBox2.Text = "Connected";
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            

            //send num of rounds to other players program here!
            byte[] sendData = Encoding.ASCII.GetBytes(num_rounds.ToString());
            clientSocket.Send(sendData);
            ///////////////////////

            //start game
            label6.Text = "Round 1";

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
            label7.Text = "press  ---->";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            x = "Paper";
            label6.Text = label6.Text + " (Locked in)";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            label7.Text = "press  ---->";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            x = "Scissor";
            label6.Text = label6.Text + " (Locked in)";
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            label7.Text = "press  ---->";

        }


        





        //a thread function to initiate UI change which indicate 
        //that you have selected the option and are waiting on OTHER player 
        private void waiting_player()
        {
            button5.Enabled= false;
            label7.Text = "";
            label8.Text = "(Waiting for other player)";


        }

        //SUBMIT BUTTON
        private void button5_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(waiting_player);
            t1.IsBackground = true;
            t1.Start();
            t1.Join();



            byte[] recvBuffer = new byte[clientSocket.SendBufferSize]; //recv buffer to store the choice from other player
            byte[] sendData = Encoding.ASCII.GetBytes(x); //sending our choice to other player
            int bytesReceived;
            string recvMessage=" ";
            
            //sending p1 choice
            clientSocket.Send(sendData);


            //Receive the message in a buffer
            bytesReceived = clientSocket.Receive(recvBuffer);
            Array.Resize(ref recvBuffer, bytesReceived);

            //Convert the raw bytes into encoded string
            recvMessage = Encoding.ASCII.GetString(recvBuffer);


            //now we will implement rock paper scissor rules
            //there are just 6 conditions (3 winning and 3 losing) so we can do it 
            //using if else statements    

            if (x == recvMessage)
            {
                disp_rec=disp_rec+ x + " VS " + recvMessage + " (DRAW)" + Environment.NewLine;
                
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


            //one round has been computed and now we move on to next by incrementing round counter
            //enabling all the buttons and checking if round limit has been reached 
            label6.Text = "Round " + (i + 1);
            i++;

            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = false; 
            //label7.Text = "";
            label8.Text = "";

            if (i == num_rounds+1)
            {
                close_sckt();
            }

              

        }

        //once round limit has been reached we can simply compute winner and close the socket
        private void close_sckt()
        {
            //sum to determine who won and who lost
            //will send this var to another form (class)
            int sum = 999; //1 for win, 0 for draw, -1 for loss


            label6.Text = "Finshed!";

            clientSocket.Close();

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


            //here we are sending the result to final window as well (just for aesthetic, no real use)
            EndScreen scr = new EndScreen(sum);
            scr.ShowDialog();

            this.Close();
        }







        //no use
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        //INITIALLY, we display a round selecting window first and foremost
        private void Form1_Load(object sender, EventArgs e)
        {

            using (StartScreen scr = new StartScreen())
            {
                var res = scr.ShowDialog();
                if (res == DialogResult.OK)
                {
                    //pass the num of rounds entered in prev form to this one for computation
                    num_rounds = scr.rounds;
                }
            }


            //also block all buttons before connection is established
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
        }
    }
}
