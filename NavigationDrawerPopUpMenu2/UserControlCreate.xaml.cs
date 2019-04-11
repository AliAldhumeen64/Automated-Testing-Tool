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

namespace NavigationDrawerPopUpMenu2
{
    
    public partial class UserControlCreate : UserControl
    {
        

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
            }

    
    }
    }

