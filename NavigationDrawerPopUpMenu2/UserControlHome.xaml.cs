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

        public UserControlHome()
        {
            InitializeComponent();
            //if (runCommandList)
            //{
            //    LaunchCommandLineApp();

            //}



            //pretty sure this goes here
            //if (NavigationDrawerPopUpMenu2.UserControlImport.hasReadFile && !hasDisplayedCommands)
            //{
            //    //this literally grabs the list of commands from the import page
            //    List<Command> readCommandList = NavigationDrawerPopUpMenu2.UserControlImport.commandList;

            //    List<Commands> items = new List<Commands>();

            //    for (int i = 0; i < readCommandList.Count; i++)
            //    {
            //        //if a command doesnt have a reply type, it isnt a command
            //        if (!(readCommandList.ElementAt(i).getReplyName().Equals("None")))
            //            items.Add(new Commands() { Name = readCommandList.ElementAt(i).getPayloadName(), Description = readCommandList.ElementAt(i).getDescription() });

            //    }
            //    CommandList.ItemsSource = items;
            //    hasDisplayedCommands = true;
            //}

            CommandList.Items.Add(new Commands { Name = "First Command", Id = "DESCRIPTION" });
            CommandList.Items.Add(new Commands { Name = "Second Command", Id = "DESCRIPTION" });
            CommandList.Items.Add(new Commands { Name = "Third Command", Id = "DESCRIPTION" });

            ToProcess.Items.Add(new Commands { Name = "Third Command", Id = "DESCRIPTION" });
            ToProcess.Items.Add(new Commands { Name = "Second Command", Id = "DESCRIPTION" });


        }
        //Object command to test
        public class Commands
        {
            public string Name { get; set; }

            public string Id { get; set; }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

}
