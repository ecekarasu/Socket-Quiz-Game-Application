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

namespace step2_server
{
    public partial class Form1 : Form
    {
        //initialize some variables
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clientSockets = new List<Socket>();
        static List<Player> playerList = new List<Player>();
        Dictionary<Player, Socket> playerSocketDict = new Dictionary<Player, Socket>();
        List<string> questions = new List<string>();
        List<int> answers = new List<int>();
        static List<Player> waitingList = new List<Player>();
        static List<Socket> waitingSocket = new List<Socket>();

        bool isGameStarted = false;
        bool isGameFinished = false;
        bool playerleft = false; //
        bool tie = false; //
        bool isQuestionAsked = false;

        int numOfQuestions = 0;
        int numOfQuestionsAsked = 0;
        int numOfQuestionsControl = 0;
        string receiveAnswer = ""; //
        int numAnswers = 0;
        int connectCount = 0; //

        static bool terminating = false;
        static bool accept = true; //

        delegate void Del(string text);
        Thread acceptThread, receiveThread, gameThread;

        public class Player : IComparable<Player>
        {
            public String name;
            public int id, answer, diff;
            public double score;

            public int CompareTo(Player other)
            {
                return name.CompareTo(other.name);
            }

            public override bool Equals(object obj)
            {
                var other = obj as Player;
                if (other == null) { return false; }
                return other.name == this.name;
            }
        }
        

        public Form1()
        {
            InitializeComponent();
            //...
            Control.CheckForIllegalCrossThreadCalls = false;
            //this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

       
        private void Accept()
        {
            //...
            while (accept)
            {
                try
                {
                    if (!isGameStarted)
                    {
                        clientSockets.Add(serverSocket.Accept());

                        if (clientSockets.Count > 1) { this.Invoke((MethodInvoker)delegate { button_startgame.Enabled = true; }); }

                        receiveThread = new Thread(new ThreadStart(Receive));
                        receiveThread.IsBackground = true; 
                        receiveThread.Start();
                    }
                }
                catch 
                {
                    if (terminating)
                    {
                        richTextBox_info.AppendText("Server stopped working...");
                        accept = false;
                    }

                    else { richTextBox_info.AppendText("Problem occured in accept function..."); }
                }
            }
        }

        private void Receive()
        {
            //...
            bool connected = true;
            int lenClientSocket = clientSockets.Count();
            Socket thisClient = clientSockets[lenClientSocket - 1];
            Player myplayer = new Player();

            bool isExist = false;

            //1 -> no accept
            //2 -> quit
            //3 -> answer

            while (connected && !terminating)
            {
                try
                {
                    

                    if (!isExist)
                    {
                        Byte[] name = new byte[20];
                        int rec = thisClient.Receive(name);
                        Player temp = new Player();
                        string tempname = Encoding.ASCII.GetString(name);
                        tempname = tempname.Substring(0, tempname.IndexOf("\0"));
                        temp.name = tempname;

                        if (playerList.Contains(temp))
                        {
                            byte[] exist_error = BitConverter.GetBytes(200);
                            thisClient.Send(exist_error);
                        }

                        else
                        {
                            byte[] ok = BitConverter.GetBytes(100);
                            thisClient.Send(ok);

                            if (!isGameStarted)
                            {
                                richTextBox_info.AppendText(tempname + " connected\n");

                                myplayer.name = tempname;
                                myplayer.id = connectCount;
                                myplayer.score = 0;
                                playerList.Add(myplayer);
                                connectCount ++;
                                playerSocketDict.Add(myplayer, thisClient);
                                isExist = true;
                            }

                            else      //burda 1 var
                            {
                                byte[] westarted = Encoding.Default.GetBytes("The game has already started. To join, wait for the game to end.");
                                myplayer.name = tempname;
                                waitingList.Add(myplayer);
                                waitingSocket.Add(thisClient);

                                thisClient.Send(westarted);
                            }
                        }
                    }

                    else
                    {                                               
                        Byte[] message = new Byte[2048];
                        thisClient.Receive(message);
                        receiveAnswer = Encoding.Default.GetString(message);
                        string[] type = receiveAnswer.Split('&');
                                                                    
                        if (type[0] == "3") 
                        {
                            int ans = Int32.Parse(type[1]);
                            myplayer.answer = ans;
                            numAnswers++;
                        }

                        else if (type[0] == "2") 
                        {
                            playerSocketDict[myplayer].Shutdown(SocketShutdown.Both);  
                            playerSocketDict[myplayer].Close();
                            playerSocketDict.Remove(myplayer);
                            playerList.Remove(myplayer);

                            if (playerSocketDict.Count == 1) 
                            {
                                isGameFinished = true;
                            }

                            else
                            {
                                playerleft = true;
                            }

                            richTextBox_info.AppendText(myplayer.name + " has disconnected");
                            foreach (Player client in playerList)
                            {
                                send_message(playerSocketDict[client], myplayer.name + " has disconnected\n");
                            }

                            break; 
                        }

                        else
                        {
                            richTextBox_info.AppendText("ERROR");
                        }
                    }
                }

                catch
                {
                    richTextBox_info.AppendText("Cannot receive");
                    if (!terminating) { thisClient.Close(); }
                    clientSockets.Remove(thisClient);
                    connected = false;
                }
            }
        }

      
        private void game()
        {
            //...
            while (isGameStarted && (numOfQuestionsControl != numOfQuestions))
            {
                if (!isQuestionAsked)
                {
                    foreach (Player clientName in playerList)
                    {
                        if (numOfQuestionsAsked < questions.Count())//burda 0lar var
                        {
                            send_message(playerSocketDict[clientName], "Question is: " + questions[numOfQuestionsAsked] + "\n");
                        }
                        else
                        {
                            numOfQuestionsAsked = numOfQuestionsAsked % questions.Count();
                            send_message(playerSocketDict[clientName], "Question is: " + questions[numOfQuestionsAsked] + "\n");
                        }

                    }
                    richTextBox_info.AppendText("Question is: " + questions[numOfQuestionsAsked] + "\n");
                    isQuestionAsked = true;
                }
                else
                {

                    while (numAnswers != (playerList.Count) && !isGameFinished && !playerleft)
                    {
                    } 


                    if (playerleft)
                    {
                        playerleft = false;
                        richTextBox_info.AppendText("A player left the game unexpectedly.\nGame will continue without that player.");
                        while (numAnswers != (playerList.Count) && !isGameFinished)
                        {
                        } 
                    }
                   

                    int correctAnswer = answers[numOfQuestionsAsked];
                    int min = 2147483647; // INT MAX
                    foreach (Player player in playerList)  
                    {
                        player.diff = Math.Abs(correctAnswer - player.answer);
                        richTextBox_info.AppendText(player.name + ": " + player.answer + "\n");

                        if (player.diff < min)
                        {
                            min = player.diff; 
                        }
                    }

                    List<Player> winner = new List<Player>();
                    List<string> sortedList = new List<string>();
                    foreach (Player player in playerList)   
                    {
                        if (player.diff == min)
                            winner.Add(player);
                    }

                    richTextBox_info.AppendText("The answer is: " + correctAnswer + "\n");
                    richTextBox_info.AppendText("Scores: \n");

                    if (winner.Count == 1)
                        winner[0].score += 1;
                    else
                    {
                        foreach (Player win in winner)
                            win.score += 1.0 / winner.Count;
                    }

                    playerList.Sort((a, b) => b.score.CompareTo(a.score)); 

                    foreach (Player client in playerList)
                    {
                        send_message(playerSocketDict[client], "The answer is: " + correctAnswer + "\n");

                        foreach (Player client2 in playerList)
                        {
                            if (client.name != client2.name)
                            {
                                send_message(playerSocketDict[client], "" + client2.name + "'s answer is: " + client2.answer + "\n");
                            }

                            send_message(playerSocketDict[client], "" + client2.name + ": " + client2.score + "\n");
                            
                        }
                        richTextBox_info.AppendText(client.name + ": " + client.score + "\n");
                    }

                    isQuestionAsked = false;
                    numAnswers = 0;
                    numOfQuestionsAsked += 1;
                    numOfQuestionsControl++;

                }

            }

            
            string winners = "";
            List<string> winnerlist = new List<string>();
            winnerlist = gamewinner(playerList);
            
            try
            {
                if (isGameFinished) 
                {
                    foreach (Player answerer in playerList) 
                    {
                        Byte[] gover = Encoding.Default.GetBytes("All other players left the game and " + answerer.name + " won the game!");
                        playerSocketDict[answerer].Send(gover);
                    }
                    //richTextBox_info.AppendText("All other players left the game and " + answerer.name + " won the game!");
                }

                else
                {
                    if (tie) 
                    {
                        foreach (string word in winnerlist) 
                        {
                            winners += word + "\n";
                        }

                        foreach (Player answerer in playerList) 
                        {
                            Byte[] tied = Encoding.Default.GetBytes("There was a tie and winners are: \n" + winners);
                            playerSocketDict[answerer].Send(tied);
                        }
                        richTextBox_info.AppendText("There was a tie and winners are: \n" + winners);
                    }

                    else 
                    {
                        foreach (Player answerer in playerList) 
                        {
                            Byte[] street = Encoding.Default.GetBytes("" + winnerlist[0] + " is the winner!");
                            playerSocketDict[answerer].Send(street);
                        }
                        richTextBox_info.AppendText(winnerlist[0] + " is the winner!");
                    }
                }
            }

            catch
            {
                richTextBox_info.AppendText("ERROR");
                terminating = true;
                serverSocket.Close();
            }
            numOfQuestionsControl = 0;
            numOfQuestionsAsked = 0;
            numAnswers = 0;
            richTextBox_info.AppendText("The game is over.");
            //playerList.Clear();
            /*
            foreach (Player p in playerList)
            {
                p.score = 0.0;
            }
             * */
            //isGameFinished = false;
        }
  

  
        public List<string> gamewinner(List<Player> list)
        {
            List<string> winners = new List<string>();      
            int element = (list.Count) - 2;                

            playerList.Sort((a, b) => a.score.CompareTo(b.score));

            winners.Add(list[(list.Count) - 1].name);

            double winnersscore = list[list.Count - 1].score;

            for (; element >= 0; element--) 
            {
                if (winnersscore == list[element].score) 
                {
                    winners.Add(list[element].name);
                    tie = true;
                }

                else { break; }
            }

            return winners;
        }

       
        private void send_message(Socket clientSocket, string message) 
        {
            Byte[] buffer = new Byte[10000000];
            buffer = Encoding.Default.GetBytes(message);
            clientSocket.Send(buffer);
        }

      
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string terminate = "Shutting Down...";
            Byte[] close = Encoding.Default.GetBytes(terminate);

            if (playerSocketDict.Count != 0)
            {
                foreach (Player player in playerList)
                {
                    playerSocketDict[player].Send(close);
                    playerSocketDict[player].Shutdown(SocketShutdown.Both);
                    playerSocketDict[player].Close();
                }
            }

            Application.Exit();
            Environment.Exit(Environment.ExitCode);
            return;
        }

        private void button_listen_Click_1(object sender, EventArgs e)
        {
            int serverPort;

            if (Int32.TryParse(textBox_port.Text, out serverPort)) 
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

                    button_listen.Enabled = false;
                    button_listen.Text = "Listening";
                    button_listen.BackColor = Color.Green;

                     
                    acceptThread = new Thread(Accept);
                    acceptThread.IsBackground = true;
                    acceptThread.Start();

                    richTextBox_info.AppendText("Started listening on port: " + serverPort + "\n");
                    richTextBox_info.ScrollToCaret();
                    string filename = textBox_filename.Text;

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


                    Int32.TryParse(textBox_num.Text, out numOfQuestions);
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

        private void button_startgame_Click(object sender, EventArgs e)
        {

            foreach (Player p in waitingList)//adding players from the waiting list to the game
            {
                playerList.Add(p);
                /*
                Socket newSocket =
                playerSocketDict.Add(myplayer, thisClient);
                 * */
            }


            foreach (Player p in playerList)
            {
                p.score = 0.0;
            }
            isGameStarted = true;
            tie = false;
            Int32.TryParse(textBox_num.Text, out numOfQuestions);
            gameThread = new Thread(new ThreadStart(game));
            gameThread.IsBackground = true;
            gameThread.Start();
            //gameThread.Join();
        }

    }
   
}
