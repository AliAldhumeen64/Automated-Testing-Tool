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

        //we wanted to save the ips and ports if you made a connection and come back, and it does save them successfully
        //the problem is you have to re-enter these values in order to create the connection
        //we wanted these values to display in the input boxes but they didn't play nice
        private static string this_Ip ="";
        private static string system_Ip = "";
        private static string this_Port = "";
        private static string system_Port = "";
        private static List<BaseMessage> replies = new List<BaseMessage>();
        //this "commandQueue" value is the queue of commands that gets sent to the black box over udp
        //if you want to add something to the queue, you add it to this variable
        //it is static so that you can access it on other xaml pages of this project
        public static List<Command> commandQueue = new List<Command>();

        //these indexes are used to keep track of what command we're reading the input for, as well as what offset we're reading the input for
        //these are reused based on the context, and could easily be the cause of bugs
        public static int commandIndex = -1;
        public static int offsetIndex = -1;

        //the udpclient does not like being created twice, we're using a boolean value to prevent making a 2nd connection
        //this could likely be changed
        private static bool hasConnection = false;

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
            udp.Close();
            hasConnection = false;

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

                    //these should be read in from the create page's UI elements
                    IPAddress serverAddr = IPAddress.Parse(system_Ip);
                    IPEndPoint endPoint = new IPEndPoint(serverAddr, Int32.Parse(system_Port));
                    IPEndPoint endPoint2 = new IPEndPoint(serverAddr, Int32.Parse(this_Port));

                    BaseMessage bsc;
                    //only connect if there is not an existing connection
                    if (!hasConnection)
                    {
                        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
                        udp = new UdpClient(endPoint2);
                        hasConnection = true;
                    }

                    //for debugging

                    //List<Offset> tempCommand1Offsets = new List<Offset>();
                    //Offset tempOffset1 = new Offset("0", "XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXA", "UINT", "none", "temp offset description");
                    //tempOffset1.setMessage("00000000000000000000000000000001");

                    //tempCommand1Offsets.Add(tempOffset1);
                    //Command tempCommand1 = new Command(14, "battle short command", true, tempCommand1Offsets, "general reply", 10000000, "test description");
                    //commandQueue.Add(tempCommand1);



                    commandIndex = 0;
                    //this is where messages are sent to the black box system we are connected to
                    //there could easily be bugs involved with the asynchronous function and the timing
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

                    //empty the queue after everything was tested
                    while(commandQueue.Count > 0)
                    {
                        commandQueue.RemoveAt(0);
                        commandIndex--;
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

            //get the replies from the black box
            byte[] bytes = udp.EndReceive(ar, ref ip);
            UInt32[] replyValues = new uint[(bytes.Length/4)];

            int j = 0;

            //these are used to log information about each of these commands to the user to give context to the values we output
            UserControlConsole.dc.ConsoleInput = "Reply name: " + commandQueue.ElementAt(commandIndex).getReplyName();
            UserControlConsole.dc.RunCommand();

            UserControlConsole.dc.ConsoleInput = "Reply description: " + commandQueue.ElementAt(commandIndex).getDescription();
            UserControlConsole.dc.RunCommand();

            UserControlConsole.dc.ConsoleInput = "Returned header values: ";
            UserControlConsole.dc.RunCommand();

            List<Offset> tempOffsets = commandQueue.ElementAt(commandIndex).getOffsetList();

            //outputs the header of each message, used for debugging
            for (int i=0; i < 6; i++)
            {
                //This is where we WOULD compare the returned values to the expected values in the read in document
                //due to time constraints we opted to log these outputs instead as you can still find discrepancies between sending something that shouldnt work and being told it worked
                replyValues[i] = BitConverter.ToUInt32(bytes, j);
                Console.WriteLine(replyValues[i]);
                //these values being outputted are just uint32s, it may be more useful to compare these values to the ICD then output information describing we we read back
                //due to time constraints we just logged the values
                UserControlConsole.dc.ConsoleInput = replyValues[i].ToString();
                UserControlConsole.dc.RunCommand();
                j += 4;
            }

            UserControlConsole.dc.ConsoleInput = "Returned payload values: ";
            UserControlConsole.dc.RunCommand();

            for (int i = 6; i < bytes.Length / 4; i++)
            {
                //This is where we WOULD compare the returned values to the expected values in the read in document
                //due to time constraints we opted to log these outputs instead as you can still find discrepancies between sending something that shouldnt work and being told it worked
                replyValues[i] = BitConverter.ToUInt32(bytes, j);
                Console.WriteLine(replyValues[i]);
                //these values being outputted are just uint32s, it may be more useful to compare these values to the ICD then output information describing we we read back
                //due to time constraints we just logged the values
                UserControlConsole.dc.ConsoleInput = replyValues[i].ToString();
                UserControlConsole.dc.RunCommand();
                j += 4;
            }

            UserControlConsole.dc.ConsoleInput = "Descriptions for individual return values from the reply: ";
            UserControlConsole.dc.RunCommand();

            //more information so the user can know what the values they get mean
            //it would be more helpful to output these alongside each header value
            for (int k=0; k < tempOffsets.Count; k++)
            {
                UserControlConsole.dc.ConsoleInput = "Offset #: " + tempOffsets.ElementAt(k).getOffsetValue();
                UserControlConsole.dc.RunCommand();
                UserControlConsole.dc.ConsoleInput = "Offset description: " + tempOffsets.ElementAt(k).getDescription();
                UserControlConsole.dc.RunCommand();
            }

            //as it was most common, we thought having a default message for general replies would be helpful
            if (commandQueue.ElementAt(commandIndex).getReplyName().Equals("general reply"))
            {
                if (replyValues[replyValues.Length - 1] == 1)
                {
                    UserControlConsole.dc.ConsoleInput = "System replied with a successful " + commandQueue.ElementAt(commandIndex).getReplyName() + ". The command executed correctly.";
                    UserControlConsole.dc.RunCommand();
                }
                else if (replyValues[replyValues.Length - 1] == 0)
                {
                    UserControlConsole.dc.ConsoleInput = "System replied with a failed " + commandQueue.ElementAt(commandIndex).getReplyName() + ". The command did not execute correctly.";
                    UserControlConsole.dc.RunCommand();
                }
            }


            //incrememnet the counter by 1
            commandIndex++;
        }

    }
}

