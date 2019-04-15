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
                
                errormessage.Text = "The system is connecting...";
                string ip_one = textboxTextOneIP.Text;
                string ip_two = textboxTextTwoIP.Text;
                string port_one = textboxPortOne.Text;
                string port_two = textboxPortTwo.Text;
            }

            LaunchCommandLineApp();
        }

        static void LaunchCommandLineApp()
        {
            //this is connecting to the bsc itself
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "bsc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "127.0.0.1 42020 42021";
            try
            {

                using (Process exeProcess = Process.Start(startInfo))
                {
                    
                    System.Threading.Thread.Sleep(1000);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);


                    //these should be read in from the create page's UI elements
                    IPAddress serverAddr = IPAddress.Parse("127.0.0.1");
                    IPEndPoint endPoint = new IPEndPoint(serverAddr, 42020);
                    IPEndPoint endPoint2 = new IPEndPoint(serverAddr, 42021);

                    BaseMessage bsc;
                    udp = new UdpClient(endPoint2);

                    //this should read in the list of commands in the queue of commands to be run
                    List<Command> commandQueue = new List<Command>();


                    List<Offset> tempCommand1Offsets = new List<Offset>();
                    Offset tempOffset1 = new Offset("0", "XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXA", "UINT", "none", "temp offset description");
                    tempOffset1.setMessage("00000000000000000000000000000001");

                    tempCommand1Offsets.Add(tempOffset1);
                    Command tempCommand1 = new Command(14, "battle short command", true, tempCommand1Offsets, "general reply", 10000000, "test description");
                    commandQueue.Add(tempCommand1);
                    commandQueue.Add(tempCommand1);
                    commandQueue.Add(tempCommand1);

                    


                    for (int i = 0; i < commandQueue.Count; i++)
                    {
                        bsc = new BaseMessage(commandQueue.ElementAt(i));
                        udp.Send(bsc.GetByteArray(commandQueue.ElementAt(i)), bsc.GetByteArray(commandQueue.ElementAt(i)).Length, endPoint);
                        System.Console.WriteLine("Sent Message successfully.");
                        udp.BeginReceive(new AsyncCallback(DataReceived), new object());

                    }

                    exeProcess.WaitForExit();
                    //bsc parameters: client ip
                } // end using
            }
            catch
            {
                System.Console.WriteLine("Error in starting file process.");
            } // end try-catch

        } // end LaunchCommandLineApp()

        private static void DataReceived(IAsyncResult ar)
        {
            System.Console.WriteLine("Waiting to receive...");
            IPAddress serverAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ip = new IPEndPoint(serverAddr, 42021);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            for(int j=0; j < bytes.Length; j++)
            {
                //This is where we would compare the returned values to the expected values in the read in document
                System.Console.WriteLine(bytes[j].ToString());
            }
            int offsetCount = bytes.Length / 4;
        }

    }
}

