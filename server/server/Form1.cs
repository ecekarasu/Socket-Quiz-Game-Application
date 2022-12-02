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
using System.IO;

namespace server
{
    public partial class Form1 : Form
    {

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<string> connectedUsers = new List<string>(); // keeps the names of connected users
        Dictionary<string, Socket> clientSocketDict = new Dictionary<string, Socket>(); // keeps name -> client tuples
        List<string> questions = new List<string>();
        List<int> answers = new List<int>();

        bool terminating = false;
        bool listening = false;
        bool isGameStarted = false;
        bool isGameFinished = false;
        int numOfQuestions = 0;
        int numOfQuestionsAsked = 0;
        int numOfQuestionsControl = 0;
        int numAnswers = 0;
        bool isQuestionAsked = false;
        string filename = "questions.txt";
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
                    string givenFilename = textBox_clients.Text;
                    if (givenFilename == filename)
                    {
                        string[] lines = File.ReadAllLines(filename, Encoding.UTF8);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (i % 2 == 0)
                            {
                                questions.Add(lines[i]);
                            }
                            else
                            {
                                answers.Add(Int32.Parse(lines[i]));

                            }
                        }

                    }
                    Int32.TryParse(textBox_num.Text, out numOfQuestions); // we got the number of questions as input
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
                    if (checkClient(newClient, ref name) && clientSocketDict.Count < 2)
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
                        if (connectedUsers.Count == 2 && !isGameStarted)
                        {
                            foreach (string clientName in connectedUsers)
                            {
                                send_message(clientSocketDict[clientName], "THE GAME IS STARTED\n");
                                isGameStarted = true;
                            }
                            richTextBox_info.AppendText("THE GAME IS STARTED\n");
                            Thread.Sleep(500);
                        }
                        Thread receiveThread = new Thread(Receive);
                        receiveThread.Start();
                    }
                    else
                    {
                        if (clientSocketDict.Count == 2)
                        {
                            richTextBox_info.AppendText(name + " is trying to connect but maximum number of clients is reached\n");
                            richTextBox_info.ScrollToCaret();
                            send_message(newClient, "maximum reached");
                            newClient.Close();
                        }
                        else
                        {
                            richTextBox_info.AppendText(name + " is trying to connect again\n");
                            richTextBox_info.ScrollToCaret();
                            send_message(newClient, "already connected");
                            newClient.Close();
                        }
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

        struct ClientAnswer
        {
            public string name;
            public int answer;

            public ClientAnswer(string name, int answer)
            {
                this.name = name;
                this.answer = answer;
            }
        }

        List<ClientAnswer> clientsAnswers = new List<ClientAnswer>();
        List<ClientAnswer> tempList = new List<ClientAnswer>();


        double scorePlayer1 = 0;
        double scorePlayer2 = 0;

        private void Receive()
        {
            bool connected = true;
            bool flag = false;
            string name = connectedUsers[clientSocketDict.Count - 1]; // we got the username
            Socket thisClient = clientSocketDict[name]; // we got the socket that related to the username



            //while (connected && !terminating && !isGameFinished)
            while (connected && !terminating && !isGameFinished)
            {
                if (!isQuestionAsked && isGameStarted)
                {
                    if (numOfQuestionsControl == numOfQuestions)
                    {
                        foreach (string clientName in connectedUsers)
                        {
                            send_message(clientSocketDict[clientName], "Game is over! Congratulations... Final scores:\n");
                            
                            if (scorePlayer1 < scorePlayer2)
                            {
                                send_message(clientSocketDict[clientName], connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + " won the game!\n");
                                
                            }
                            else if (scorePlayer2 < scorePlayer1)
                            {
                                send_message(clientSocketDict[clientName], connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + " won the game!\n");
                                
                            }
                            else
                            {
                                send_message(clientSocketDict[clientName], connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n" + "It's a draw!\n");
                            }

                        }
                        richTextBox_info.AppendText("Game is over! Congratulations... Final scores:\n");
                        if (scorePlayer1 < scorePlayer2)
                        {
                            richTextBox_info.AppendText(connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + " won the game!\n");
                        }
                        else if (scorePlayer2 < scorePlayer1)
                        {
                            richTextBox_info.AppendText(connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + " won the game!\n");
                        }
                        else
                        {
                            richTextBox_info.AppendText(connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n" + "It's a draw!");
                        }
                        isGameFinished = true;
                       

                    }
                    else {
                        Thread.Sleep(500);
                        foreach (string clientName in connectedUsers)
                        {
                            if (numOfQuestionsAsked < questions.Count())
                            {
                                send_message(clientSocketDict[clientName], "Question is: " + questions[numOfQuestionsAsked] + "\n");
                            }
                            else
                            {
                                numOfQuestionsAsked = numOfQuestionsAsked % questions.Count();
                                send_message(clientSocketDict[clientName], "Question is: " + questions[numOfQuestionsAsked] + "\n");
                            }

                        }
                        richTextBox_info.AppendText("Question is: " + questions[numOfQuestionsAsked] + "\n");
                        isQuestionAsked = true;
                    }
                }
                else {
                    try
                    {
                        string incomingMessage = receiveOneMessage(thisClient); // if there are any messages we take them
                        //yeni
                        if (incomingMessage == "-DISCONNECT-")
                        {
                            connected = false;
                            richTextBox_info.AppendText(name + " has disconnected\n");
                            richTextBox_info.ScrollToCaret();
                        }
                        else
                        {
                            //yeni

                            Thread.Sleep(500);
                            ClientAnswer pair = new ClientAnswer(name, Int32.Parse(incomingMessage));
                            if (pair.name == connectedUsers[0] && clientsAnswers.Count() == 0 && tempList.Count() == 0)
                                clientsAnswers.Add(pair);
                            else if (pair.name == connectedUsers[1] && clientsAnswers.Count() == 1)
                                clientsAnswers.Add(pair);
                            else if (pair.name == connectedUsers[1] && clientsAnswers.Count() == 0)
                                tempList.Add(pair);
                            else if (pair.name == connectedUsers[0] && tempList.Count() == 1)
                            {
                                clientsAnswers.Add(pair);
                                clientsAnswers.Add(tempList[0]);
                            }

                            richTextBox_info.AppendText(name + ": " + incomingMessage + "\n");
                            if (isGameStarted)
                            {
                                numAnswers += 1;
                                if (numAnswers == clientSocketDict.Count)
                                {
                                    int correctAnswer = answers[numOfQuestionsAsked];
                                    int diffAnswer1 = Math.Abs(correctAnswer - clientsAnswers[0].answer);
                                    int diffAnswer2 = Math.Abs(correctAnswer - clientsAnswers[1].answer);
                                    if (diffAnswer1 < diffAnswer2)
                                    {
                                        scorePlayer1++;
                                    }
                                    else if (diffAnswer2 < diffAnswer1)
                                    {
                                        scorePlayer2++;
                                    }
                                    else
                                    {
                                        scorePlayer1 += 0.5;
                                        scorePlayer2 += 0.5;
                                    }

                                    foreach (string clientName in connectedUsers)
                                    {
                                        send_message(clientSocketDict[clientName], "The answer is: " + correctAnswer + "\n");
                                        send_message(clientSocketDict[clientName], clientsAnswers[0].name + "'s answer: " + clientsAnswers[0].answer + "\n");

                                        send_message(clientSocketDict[clientName], clientsAnswers[1].name + "'s answer: " + clientsAnswers[1].answer + "\n");

                                        send_message(clientSocketDict[clientName], "Scores: \n");

                                        if (scorePlayer1 < scorePlayer2)
                                        {
                                            send_message(clientSocketDict[clientName], connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + ": " + scorePlayer1 + "\n");

                                        }
                                        else
                                        {
                                            send_message(clientSocketDict[clientName], connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n");

                                        }
                                    }
                                    richTextBox_info.AppendText("The answer is: " + correctAnswer + "\n");
                                    richTextBox_info.AppendText("Scores: \n");
                                    if (scorePlayer1 < scorePlayer2)
                                        richTextBox_info.AppendText(connectedUsers[1] + ": " + scorePlayer2 + "\n" + connectedUsers[0] + ": " + scorePlayer1 + "\n");
                                    else
                                        richTextBox_info.AppendText(connectedUsers[0] + ": " + scorePlayer1 + "\n" + connectedUsers[1] + ": " + scorePlayer2 + "\n");

                                    isQuestionAsked = false;
                                    numAnswers = 0;
                                    numOfQuestionsAsked += 1;
                                    numOfQuestionsControl++;
                                    clientsAnswers.Clear();
                                    tempList.Clear();
                                }
                            }
                        }//else sonu
                    }
                    catch // if disconnected by closing the window
                    {
                        flag = true;
                        foreach (string clientName in connectedUsers)
                        {
                            if (clientName != name) // check for to do not send it to sender client
                            {
                                Socket tempSocket = clientSocketDict[clientName]; // we got the socket
                                send_message(tempSocket, (name + " has disconnected\n"));
                            }
                        }

                        //yeni
                        if (isGameStarted)
                        {

                            foreach (string clientName in connectedUsers)
                            {
                                send_message(clientSocketDict[clientName], "since " + name + " is disconnected, you won the game\n");
                            }
                            isGameFinished = true;
                            isGameStarted = false;
                            Thread.Sleep(500);
                        }
                        //yeni

                        richTextBox_info.AppendText(name + " has disconnected\n");
                        richTextBox_info.ScrollToCaret();
                        thisClient.Close();
                        connectedUsers.Remove(name);
                        clientSocketDict.Remove(name);
                        connected = false;
                    }
                }
            }

            //end of while loop

            if (!connected && !flag) // if clicked disconnect button
            {
                foreach (string clientName in connectedUsers)
                {
                    if (clientName != name) // check for to don't send it to sender client
                    {
                        Socket tempSocket = clientSocketDict[clientName]; // we got the socket
                        send_message(tempSocket, (name + " has disconnected\n"));
                    }
                }

                //yeni
                if (isGameStarted)
                {

                    foreach (string clientName in connectedUsers)
                    {
                        if (name != clientName)
                        {
                            send_message(clientSocketDict[clientName], "since " + name + " is disconnected, you won the game\n");
                        }
                    }
                    isGameFinished = true;
                    isGameStarted = false;
                    Thread.Sleep(500);
                }
                //yeni

                thisClient.Close();
                connectedUsers.Remove(name);
                clientSocketDict.Remove(name);
            }

            richTextBox_info.AppendText(name + " has disconnected\n");
            thisClient.Close();
            connectedUsers.Remove(name);
            clientSocketDict.Remove(name);

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
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
    
            Environment.Exit(0);
        }
    }
}