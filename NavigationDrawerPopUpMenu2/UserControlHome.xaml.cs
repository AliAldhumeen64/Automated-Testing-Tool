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
using System.Text.RegularExpressions;

namespace NavigationDrawerPopUpMenu2
{
 
    public partial class UserControlHome : UserControl
    {

        private readonly SharedLayoutCoordinator example;

        List<Commands> items = new List<Commands>();

        public UserControlHome()
        {
            InitializeComponent();

            if (NavigationDrawerPopUpMenu2.UserControlImport.hasReadFile)
            {
                //this literally grabs the list of commands from the import page
                List<Command> readCommandList = UserControlImport.commandList;

           

                for (int i = 0; i < readCommandList.Count; i++)
                {
                    //if a command doesnt have a reply type, it isnt a command
                    if (!(readCommandList.ElementAt(i).getReplyName().Equals("None")))
                        items.Add(new UserControlHome.Commands() { cmd = readCommandList.ElementAt(i), Name = readCommandList.ElementAt(i).getPayloadName(), Id = readCommandList.ElementAt(i).getDescription() });

                }
                CommandList.ItemsSource = items;
                
            }
        }
        //Object command to test
        public class Commands
        {
            public Command cmd
            {
                get; set;
            }
           
 
            public string Name { get; set; }

            public string Id { get; set; }


        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Commands test = new Commands();

            Commands selectedItem;

            if (CommandList.SelectedItem != null)
            {
                if (CommandList.SelectedItem.GetType().Equals(test.GetType()))
                {
                    selectedItem = (Commands)(CommandList.SelectedItem);
                    ToProcess.Items.Add(selectedItem);
                    UserControlCreate.commandQueue.Add(selectedItem.cmd);
                    UserControlCreate.commandIndex++;
                }
            }
          
        }

        private void ListViewItem_OffestList(object sender, MouseButtonEventArgs e)
        {
            Commands test = new Commands();

            Command actualCommand;

            if(ToProcess.SelectedItem != null)
            {
                if (ToProcess.SelectedItem.GetType().Equals(test.GetType()))
                {
                    Commands selectedItemsTwo = (Commands)(ToProcess.SelectedItem);
                    actualCommand = selectedItemsTwo.cmd;

                    for(int i = offsetListView.Items.Count-1; i >= 0; i--)
                    {
                        offsetListView.Items.RemoveAt(i);
                    }

                    List<Offset> tempOffsetList = selectedItemsTwo.cmd.getOffsetList();
                    for (int j = 0; j < tempOffsetList.Count; j++)
                    {
                        offsetListView.Items.Add(tempOffsetList.ElementAt(j));
                    }
                    UserControlCreate.offsetIndex = 0;
                }
            }
            
        }

        public string[] ToStringArray(string value, char separator)
        {
            return value.Split(',');
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if(offsetsInput.Text != "")
            {
                string text = offsetsInput.Text.Trim();
                string[] inputs = ToStringArray(text, ',');
                offsetsInput.Text = "";
                if (UserControlCreate.commandQueue.Count > 0)
                {
                    Command current = UserControlCreate.commandQueue.ElementAt(UserControlCreate.commandIndex);
                    List<Offset> offsetList = current.getOffsetList();
                    int maxIndexNum = current.getOffsetList().Count;

                    if (UserControlCreate.offsetIndex < maxIndexNum)
                    {
                        offsetList.ElementAt(UserControlCreate.offsetIndex).setMessage(inputs);
                        UserControlCreate.offsetIndex++;
                    }
                }
            }
            
            
        }
    }

}
