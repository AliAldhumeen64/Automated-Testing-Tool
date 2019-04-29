using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using System.Diagnostics;
using Xceed.Words.NET;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace NavigationDrawerPopUpMenu2
{

    //the console class is very barebones but it works by setting the consoleinput value to the string you want displayed
    //then you call the runcommand method and it is sent to the console


    public partial class UserControlConsole : UserControl
    {
        public static ConsoleContent dc = new ConsoleContent();

        public UserControlConsole()
        {

            InitializeComponent();

            DataContext = dc;
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InputBlock.KeyDown += InputBlock_KeyDown;
            InputBlock.Focus();
        }

        void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dc.ConsoleInput = InputBlock.Text;
                dc.RunCommand();
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
        }
    }

    public class ConsoleContent
    {
        static string consoleInput = string.Empty;
        static List<string> consoleOutput = new List<string>() { "Starting console." };

        //adds a string to the console
        public string ConsoleInput
        {
            get
            {
                return consoleInput;
            }
            set
            {
                consoleInput = value;
            }
        }

        public List<string> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {

                consoleOutput = value;
            }
        }

        //displays the string in the console
        public void RunCommand()
        {
            string tempValue = ConsoleInput;
            while (tempValue.Length > 80)
            {
                ConsoleOutput.Add(tempValue.Substring(0,80));
                tempValue = tempValue.Substring(80);
            }
            ConsoleOutput.Add(tempValue);
            ConsoleInput = String.Empty;
        }


    }
}
