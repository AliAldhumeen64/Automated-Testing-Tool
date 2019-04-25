using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace NavigationDrawerPopUpMenu2
{
    
    public partial class UserControlCreate : UserControl
    {
        private static UdpClient udp;
        private static string this_Ip;
        private static string system_Ip;
        private static string this_Port;
        private static string system_Port;
        private static List<BaseMessage> replies = new List<BaseMessage>();
        public static List<Command> commandQueue = new List<Command>();
        public static int commandIndex = -1;
        public static int offsetIndex = -1;

        public UserControlCreate()
        {
            InitializeComponent();
          
        }
        public void Reset()
        {
            textboxTextOneIP.Text = "";
            textboxTextTwoIP.Text = "";
            textboxPortOne.Text = "";
            textboxPortTwo.Text = "";
            errormessage.Text = "";


        }
        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (textboxTextOneIP.Text.Length == 0)
            {
                errormessage.Text = "Enter the server IP Address";
                textboxTextOneIP.Focus();
            }
            else if (textboxPortOne.Text.Length == 0)
            {
                errormessage.Text = "Enter the server port number";
                textboxPortOne.Focus();
            }
            else if (textboxTextTwoIP.Text.Length == 0)
            {
                errormessage.Text = "Enter the client IP Address";
                textboxTextTwoIP.Focus();
            }
            else if (textboxPortTwo.Text.Length == 0)
            {
                errormessage.Text = "Enter the client port number";
                textboxPortTwo.Focus();
            }

            else
            {
                errormessage.Text = "The system sent the queue";
                system_Ip = textboxTextOneIP.Text;
                this_Ip = textboxTextTwoIP.Text;
                system_Port = textboxPortOne.Text;
                this_Port = textboxPortTwo.Text;
                LaunchCommandLineApp(system_Ip, this_Ip, system_Port, this_Port);
            }


        }

        static void LaunchCommandLineApp(string system_Ip, string this_Ip, string system_Port, string this_Port)
        {
            //this is connecting to the bsc itself
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "bsc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            string bscSimArgs = this_Ip + " " + system_Port + " " + this_Port;
            startInfo.Arguments = bscSimArgs;
            try
            {

                using (Process exeProcess = Process.Start(startInfo))
                {
                    
                    System.Threading.Thread.Sleep(1000);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);


                    //these should be read in from the create page's UI elements
                    IPAddress serverAddr = IPAddress.Parse(system_Ip);
                    IPEndPoint endPoint = new IPEndPoint(serverAddr, Int32.Parse(system_Port));
                    IPEndPoint endPoint2 = new IPEndPoint(serverAddr, Int32.Parse(this_Port));

                    BaseMessage bsc;
                    udp = new UdpClient(endPoint2);

                    //this should read in the list of commands in the queue of commands to be run



                    //List<Offset> tempCommand1Offsets = new List<Offset>();
                    //Offset tempOffset1 = new Offset("0", "XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXA", "UINT", "none", "temp offset description");
                    //tempOffset1.setMessage("00000000000000000000000000000001");

                    //tempCommand1Offsets.Add(tempOffset1);
                    //Command tempCommand1 = new Command(14, "battle short command", true, tempCommand1Offsets, "general reply", 10000000, "test description");
                    //commandQueue.Add(tempCommand1);



                    commandIndex = 0;
                    for (int i=0; i < commandQueue.Count; i++)
                    {
                        bool skip = false;
                        Command next = commandQueue.ElementAt(i);
                        List<Offset> currentoffsetlist = next.getOffsetList();
                        for (int j =0; j < currentoffsetlist.Count; j++)
                        {
                            if(currentoffsetlist[j].getMessage() == 0)
                            {
                                //this command was unfinished because this offset had a message that was too short, so we skip it outright
                                UserControlConsole.dc.ConsoleInput = ("ERROR: Command unfinished, " + next.getPayloadName() + " skipped");
                                UserControlConsole.dc.RunCommand();

                                skip = true;
                                break;
                            }
                        }
                        if (skip)
                        {
                            commandIndex++;
                            continue;
                        }
                        bsc = new BaseMessage(next);
                        byte[] message = bsc.GetByteArray(next);
                        udp.Send(message, message.Length, endPoint);
                        Console.WriteLine("Sent Message successfully.");
                        UserControlConsole.dc.ConsoleInput = ("Sent Message successfully.");
                        UserControlConsole.dc.RunCommand();
                        udp.BeginReceive(new AsyncCallback(DataReceived), new object());
                    }

                    exeProcess.WaitForExit();

                    while(commandQueue.Count > 0)
                    {
                        commandQueue.RemoveAt(0);
                    }

                } // end using
            }
            catch
            {
                Console.WriteLine("Error in starting file process.");
            } // end try-catch

        } // end LaunchCommandLineApp()

        private static void DataReceived(IAsyncResult ar)
        {
            Console.WriteLine("Waiting to receive...");
            IPAddress serverAddr = IPAddress.Parse(system_Ip);
            IPEndPoint ip = new IPEndPoint(serverAddr, Int32.Parse(system_Port));
            byte[] bytes = udp.EndReceive(ar, ref ip);
            UInt32[] replyValues = new uint[(bytes.Length/4)];

            int j = 0;

            for(int i=0; i < bytes.Length / 4; i++)
            {
                //This is where we would compare the returned values to the expected values in the read in document
                replyValues[i] = BitConverter.ToUInt32(bytes, j);
                Console.WriteLine(replyValues[i]);
                UserControlConsole.dc.ConsoleInput = replyValues[i].ToString();
                UserControlConsole.dc.RunCommand();
                j += 4;
            }
            if(replyValues[replyValues.Length-1] == 1)
            {
                UserControlConsole.dc.ConsoleInput = "System replied with a successful " + commandQueue.ElementAt(commandIndex).getReplyName() + ". The command executed correctly.";
                UserControlConsole.dc.RunCommand();
            }
            else if (replyValues[replyValues.Length-1] == 0)
            {
                UserControlConsole.dc.ConsoleInput = "System replied with a failed " + commandQueue.ElementAt(commandIndex).getReplyName() + ". The command did not execute correctly.";
                UserControlConsole.dc.RunCommand();
            }

            //this is where we'd properly parse the message
            //Command thisReply = commandQueue.ElementAt(commandIndex);
            //BaseMessage thisMessage = new BaseMessage(bytes, thisReply);


        }

    }
}

