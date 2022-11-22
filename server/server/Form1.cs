using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : Form
    {

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<string> connectedUsers = new List<string>(); // keeps the names of connected users
        Dictionary<string, Socket> clientSocketDict = new Dictionary<string, Socket>(); // keeps name -> client tuples

        bool terminating = false;
        bool listening = false;
        

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;

            if (Int32.TryParse(textBox_port.Text, out serverPort)) // if we can parse the input port number
            {
                if (serverPort <= 65535 && serverPort >= 0)
                {
                    try
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                        serverSocket.Bind(endPoint);
                        serverSocket.Listen(300);
                    }
                    catch (Exception ex)
                    {
                        richTextBox_info.AppendText("Fail: " + ex.ToString() + "\n");
                        richTextBox_info.ScrollToCaret();
                    }

                    listening = true;
                    button_listen.Enabled = false;
                    button_listen.Text = "Listening";
                    button_listen.BackColor = Color.Green;

                    Thread acceptThread = new Thread(Accept);
                    acceptThread.Start();

                    richTextBox_info.AppendText("Started listening on port: " + serverPort + "\n");
                    richTextBox_info.ScrollToCaret();
                }
                else
                {
                    richTextBox_info.AppendText("Port number should be between 0 and 65535\n");
                    richTextBox_info.ScrollToCaret();
                }
            }
            else
            {
                richTextBox_info.AppendText("Please check port number \n");
                richTextBox_info.ScrollToCaret();
            }
        }
        private void Accept()
        {
            while (listening)
            {
                try
                {
                    string name = ""; // we initialize name to empty string
                    Socket newClient = serverSocket.Accept(); // first we accept the new connection request
                    if (checkClient(newClient, ref name))
                    { // gets the name and check if name is registered
                        send_message(newClient, "authorized\n");
                        clientSocketDict.Add(name, newClient);
                        connectedUsers.Add(name);
                        richTextBox_info.AppendText(name + " is connected.\n");
                        richTextBox_info.ScrollToCaret();
                        foreach (string clientName in connectedUsers)
                        {
                            if (clientName != name) // do not send it to sender client
                            {
                                Socket tempSocket = clientSocketDict[clientName]; // we got the socket
                                send_message(tempSocket, (name + " is connected\n"));
                            }
                        }
                        Thread receiveThread = new Thread(Receive);
                        receiveThread.Start();
                    }
                    else
                    {
                        richTextBox_info.AppendText(name + " is trying to connect again\n");
                        richTextBox_info.ScrollToCaret();
                        send_message(newClient, "already connected");
                        newClient.Close();
                    }
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        richTextBox_info.AppendText("The socket stopped working.\n");
                        richTextBox_info.ScrollToCaret();
                    }
                }
            }
        }
        private bool checkClient(Socket thisClient, ref string name) 
        {
            try
            {
                string incomingName = receiveOneMessage(thisClient); // get the name
                if (!connectedUsers.Contains(incomingName)) 
                {
                    name = incomingName;
                    return true;
                }
                else
                {
                    name = incomingName;
                    return false;
                }
            }
            catch (Exception ex)
            {
                richTextBox_info.AppendText("Fail: " + ex.ToString() + "\n");
                richTextBox_info.ScrollToCaret();
                throw;
            }
        }
        private string receiveOneMessage(Socket clientSocket) // this function receives only one message and returns it
        {
            Byte[] buffer = new Byte[10000000];
            clientSocket.Receive(buffer);
            string incomingName = Encoding.Default.GetString(buffer);
            incomingName = incomingName.Substring(0, incomingName.IndexOf("\0"));
            return incomingName;
        }
        private void send_message(Socket clientSocket, string message) // takes socket and message then sends the message to that socket
        {
            Byte[] buffer = new Byte[10000000];
            buffer = Encoding.Default.GetBytes(message);
            clientSocket.Send(buffer);
        }
        private void Receive() // updated
        {
            bool connected = true;
            bool flag = false;
            string name = connectedUsers[connectedUsers.Count() - 1]; // we got the username
            Socket thisClient = clientSocketDict[name]; // we got the socket that related to the username

            while (connected && !terminating)
            {
                try
                {
                    string incomingMessage = receiveOneMessage(thisClient); // if there are any messages we take it
                    richTextBox_info.AppendText(name + " " + incomingMessage);
                }
                catch
                {
                    flag = true;
                    foreach (string clientName in connectedUsers)
                    {
                        if (clientName != name) // check for to don't send it to sender client
                        {
                            Socket tempSocket = clientSocketDict[clientName]; // we got the socket
                            send_message(tempSocket, (name + " has disconnected\n"));
                        }
                    }
                    richTextBox_info.AppendText(name + " has disconnected\n");
                    richTextBox_info.ScrollToCaret();
                    thisClient.Close();
                    connectedUsers.Remove(name);
                    clientSocketDict.Remove(name);
                    connected = false;
                }
            }
            if (!connected && !flag)
            {
                foreach (string clientName in connectedUsers)
                {
                    if (clientName != name) // check for to don't send it to sender client
                    {
                        Socket tempSocket = clientSocketDict[clientName]; // we got the socket
                        send_message(tempSocket, (name + " has disconnected\n"));
                    }
                }
                thisClient.Close();
                connectedUsers.Remove(name);
                clientSocketDict.Remove(name);
            }
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }
    }
}