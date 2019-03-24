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
        private bool hasDisplayedCommands = false;

        public UserControlCreate()
        {
            InitializeComponent();


            //
            //NEXT STEPS HERE
            //
            //We need to display the offset values for each command, with their descriptions
            //we need to let the user put commands into the queue of commands, and enter the offset values for the command, maybe only show the offsets and their descriptions when the user wants to enter them?
            //once the user is happy with what they put into the queue, the home tab needs to access the queue with the parameter values entered so that they can be sent to the BSC

            // AND REMEMBER
            // WE CAN DO IT


            if (NavigationDrawerPopUpMenu2.UserControlImport.hasReadFile && !hasDisplayedCommands)
            {
                //this literally grabs the list of commands from the import page
                List<Command> readCommandList = NavigationDrawerPopUpMenu2.UserControlImport.commandList;

                List<Commands> items = new List<Commands>();

                for (int i = 0; i < readCommandList.Count; i++)
                {
                    //if a command doesnt have a reply type, it isnt a command
                    if (!(readCommandList.ElementAt(i).getReplyName().Equals("None")))
                        items.Add(new Commands() { Name = readCommandList.ElementAt(i).getPayloadName(), Description = readCommandList.ElementAt(i).getDescription() });
                    
                }
                lvUsers.ItemsSource = items;
                hasDisplayedCommands = true;
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
        }
    }

    //Object command to test
    public class Commands
    {
        public string Name { get; set; }

        public String Description { get; set; }
    }
}
