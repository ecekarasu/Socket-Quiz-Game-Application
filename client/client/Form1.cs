using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        string name;

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }
        private string receiveOneMessage() // this function receives only one message
        {
            Byte[] buffer = new Byte[10000000];
            clientSocket.Receive(buffer);
            string incomingMessage = Encoding.Default.GetString(buffer);
            incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
            return incomingMessage;
        }
        private void send_message(string message)
        {
            Byte[] buffer = new Byte[10000000];
            buffer = Encoding.Default.GetBytes(message);
            clientSocket.Send(buffer);
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            terminating = false; // to connect after disconnect
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_ip.Text;
            int portNum;
            name = textBox_name.Text;
            string serverResponse = "";

            if (name != "" && name.Length <= 10000000) // if name is not empty and longer than 10m
            {
                if (Int32.TryParse(textBox_port.Text, out portNum))
                {
                    if (textBox_ip.Text != "")
                    {
                        try
                        {
                            clientSocket.Connect(IP, portNum);
                            send_message(name); // we send our username to server and wait for respond
                            serverResponse = receiveOneMessage(); // we got our respond
                            if (serverResponse != "already connected" && serverResponse != "maximum reached")
                            {
                                button_connect.Enabled = false;
                                connected = true;
                                //button_disconnect.Enabled = true;
                                //button_submit.Enabled = true;
                                button_connect.Text = "Connected";
                                button_connect.BackColor = System.Drawing.Color.Green;
                                logs.AppendText("Connection established...\n");
                                logs.ScrollToCaret();

                                Thread receiveThread = new Thread(Receive);
                                receiveThread.Start();
                            }
                            else if (serverResponse == "already connected")
                            {
                                logs.AppendText("You are already connected.\n");
                                logs.ScrollToCaret();
                            }
                            else if (serverResponse == "maximum reached")
                            {
                                logs.AppendText("Cannot connect, maximum number of clients is reached.\n");
                                logs.ScrollToCaret();
                            }
                            else
                            {
                                logs.AppendText("BUG DETECTED, Check client's serverResponse.\n");
                                logs.ScrollToCaret();
                            }
                        }
                        catch
                        {
                            logs.AppendText("Could not connect to the server!\n");
                            logs.ScrollToCaret();
                        }
                    }
                    else
                    {
                        logs.AppendText("Check the IP\n");
                        logs.ScrollToCaret();
                    }
                }
                else
                {
                    logs.AppendText("Check the port\n");
                    logs.ScrollToCaret();
                }
            }
            else
            {
                textBox_name.Text = "";
                logs.AppendText("Name length must between 1 and 10m\n");
                logs.ScrollToCaret();
            }
        }
        private void Receive()
        {
            while (connected)
            {
                try
                {
                    string incomingMessage = receiveOneMessage();
                    logs.AppendText(incomingMessage);
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("The server has disconnected\n");
                        logs.ScrollToCaret(); 
                    }
                    clientSocket.Close();
                    connected = false;
                }
            }
        }
        private void button_submit_Click(object sender, EventArgs e)
        {
            string answer = textBox_answer.Text;
            if (answer != "" && answer.Length <= 10000000)
            {
                textBox_answer.Text = "";
                send_message(answer);
                logs.AppendText("Me: " + answer + "\n");
                logs.ScrollToCaret();
            }
            else
            {
                textBox_answer.Text = "";
                logs.AppendText("Message length must between 1 and 10m\n");
                logs.ScrollToCaret();
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {

            logs.AppendText("You have disconnected.\n");
            button_connect.Enabled = true;
            connected = false;
            clientSocket.Close();
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
        }
    }
}
