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
 
    public partial class UserControlHome : UserControl
    {
        private static UdpClient udp;
        bool runCommandList = false;

        public UserControlHome()
        {
            InitializeComponent();
            if (runCommandList)
            {
                LaunchCommandLineApp();
            }
        }

        static void LaunchCommandLineApp()
        {
            //this is connecting to the bsc itself
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "bsc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "127.0.0.1 52020 52021";

            try
            {

                using (Process exeProcess = Process.Start(startInfo))
                {
                    System.Threading.Thread.Sleep(1000);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
                    IPAddress serverAddr = IPAddress.Parse("127.0.0.1");
                    IPEndPoint endPoint = new IPEndPoint(serverAddr, 52020);
                    //this should read in the list of commands in the queue of commands to be run
                    //BattleShortCmd bsc = new BattleShortCmd();

                    //this should probably be looped and the .getCommand function should just get the next command in the queue
                    //the loop should go command->return value ->command->return value->command->return value
                    //sock.SendTo(bsc.getCMD(), bsc.getCMD().Length, SocketFlags.None, endPoint);
                    udp = new UdpClient(52021);
                    udp.BeginReceive(DataReceived, new object());
                    exeProcess.WaitForExit();
                    //bsc parameters: client ip
                } // end using
            }
            catch
            {
                System.Console.WriteLine("Error in starting file process.");
            } // end try-catch

        } // end LaunchCommandLineApp()

        //this never gets called, no idea how this works or if this works
        private static void DataReceived(IAsyncResult ar)
        {
            System.Console.WriteLine("Waiting to receive...");
            IPAddress serverAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ip = new IPEndPoint(serverAddr, 52021);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine(message);
        }
    }
}
