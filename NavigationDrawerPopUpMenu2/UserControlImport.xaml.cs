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

namespace NavigationDrawerPopUpMenu2
{
    
    public partial class UserControlImport : UserControl
    {
        private bool hasDisplayedCommands = false;
        //this will literally have the sync key value that is read from the file if the "hasReadFile" bool value is true
        public static int syncKey;

        //this will literally have the command list that is read from the file if the "hasReadFile" bool value is true
        public static List<Command> commandList;

        //this will be true if a document was read by the parser, false otherwise
        public static bool hasReadFile = false;

        //This function will outright return what message in the commandlist is the reply to the given command
        public static Command getCommandReply(Command thisCommand)
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                if (commandList.ElementAt(i).getPayloadType() == thisCommand.getReplyValue())
                    return commandList.ElementAt(i);
            }
            //this should only get returned if this is called before the list of commands are read.
            return commandList.ElementAt(0);
        }

        //these will be needed for sending a message but wont be saved here, or will it?
        //protected int nextMessageIdentifier = 1; //increase this value by 1 everytime a message is sent.
        //protected int payloadSize;
        //protected int timeStampMSB = 0;
        //protected int timeStampLSB = 0;



        public UserControlImport()
        {
          
        InitializeComponent();
            OpenFileClicked();

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

        public void OpenFileClicked()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                Stream fileName = openFileDialog.OpenFile();
                parse(fileName);
            }
        }

        public static void parse(Stream fileName)
        {
            DocX docx = DocX.Load(fileName);
            commandList = new List<Command>();
            

            var paraList = docx.Paragraphs;

            syncKey = getSyncKey(docx);
            

            commandList = getCommands(docx);
            hasReadFile = true;
            //
            //USED FOR DEBUGGING
            //
            //List<Offset> currentOffsets;
            //Debug.WriteLine("The Sync key is: ");
            //Debug.WriteLine(syncKey);
            //Debug.WriteLine(" ");
            //Debug.WriteLine(commandList.ElementAt(0).getPayloadName());
            //Debug.WriteLine(commandList.ElementAt(0).getPayloadType());
            //Debug.WriteLine(commandList.ElementAt(0).getIsCommand());
            //Debug.WriteLine(commandList.ElementAt(0).getReplyName());
            //Debug.WriteLine(commandList.ElementAt(0).getReplyValue());
            //Debug.WriteLine(" ");

            //for (int i = 1; i < commandList.Count; i++)
            //{
            //    Debug.WriteLine(commandList.ElementAt(i).getPayloadName());
            //    Debug.WriteLine(commandList.ElementAt(i).getPayloadType());
            //    Debug.WriteLine(commandList.ElementAt(i).getIsCommand());
            //    Debug.WriteLine(commandList.ElementAt(i).getReplyName());
            //    Debug.WriteLine(commandList.ElementAt(i).getReplyValue());
            //    Debug.WriteLine("Offset List: ");
            //    currentOffsets = commandList.ElementAt(i).getOffsetList();
            //    for (int j = 0; j < currentOffsets.Count; j++)
            //    {
            //        Debug.WriteLine("Offset: " + currentOffsets.ElementAt(j).getOffsetValue());
            //        Debug.WriteLine("Mask: " + currentOffsets.ElementAt(j).getMask());
            //        Debug.WriteLine("Type: " + currentOffsets.ElementAt(j).getType());
            //        Debug.WriteLine("Units: " + currentOffsets.ElementAt(j).getUnits());
            //        Debug.WriteLine("Description: " + currentOffsets.ElementAt(j).getDescription());
            //    }
            //    Debug.WriteLine(" ");
            //}

            //use for debugging, outputs the whole document
            //string text;
            //for (int j = 0; j < paraList.Count; j++)
            //{
            //    text = paraList.ElementAt(j).Text;
            //    Debug.WriteLine(text);
            //    Debug.WriteLine(j);
            //}
        }

        //This function takes in the DocX object that has opened the desired file, then returns the sync key

        //TO-DO change the code of how this function works to work for ANY IDD through use of docX.Paragraphs in the specific cell with the sync key, currently only works with this one
        public static int getSyncKey(DocX docx)
        {
            int syncKey = 0;
            string text = docx.Text;
            var textList = docx.FindAll("Sync Key");
            var foundIndex = textList.ElementAt(0).ToString();
            int startIndex = int.Parse(foundIndex) + 38;
            int endIndex = startIndex + 32;
            string stringKey = text.Substring(startIndex, 29);
            syncKey = Convert.ToInt32(stringKey, 2);
            return syncKey;
        }

        //returns the list of commands AND replies from the file
        public static List<Command> getCommands(DocX docx)
        {
            List<Command> commandList = new List<Command>();
            var paraList = docx.Paragraphs;

            Xceed.Words.NET.Paragraph currentP;
            string commandPayloadName, commandReplyName, commandDescription;
            bool commandIsCommand;
            Int32 commandReplyValue, commandPayloadType;

            commandPayloadType = 0;
            commandPayloadName = "no message payload";
            commandIsCommand = false;
            List<Offset> firstCommandOffsets = null;
            commandReplyName = "None";
            commandReplyValue = -1;
            commandDescription = "None";
            Command firstCommand = new Command(commandPayloadType, commandPayloadName, commandIsCommand, firstCommandOffsets, commandReplyName, commandReplyValue, commandDescription);
            commandList.Add(firstCommand);

            for (int i = 0; i < paraList.Count; i++)
            {
                currentP = paraList.ElementAt(i);
                if (paraList.ElementAt(i).Text.Equals("Payload Name"))
                {
                    if (paraList.ElementAt(i - 1).Text.Equals("command"))
                    {
                        commandIsCommand = true;
                        commandPayloadName = paraList.ElementAt(i + 1).Text + " command";
                        commandReplyName = paraList.ElementAt(i + 5).Text;
                        commandReplyValue = int.Parse(paraList.ElementAt(i + 7).Text);
                    }
                    else
                    {
                        commandIsCommand = false;
                        commandPayloadName = paraList.ElementAt(i + 1).Text + " reply";
                        commandReplyName = "None";
                        commandReplyValue = -1;
                    }
                    commandPayloadType = Int32.Parse(paraList.ElementAt(i + 3).Text);

                    //should never actually be null
                    Xceed.Words.NET.Table testTable = null;
                    commandDescription = "";
                    for (int j = i + 8; !(paraList.ElementAt(j).Equals("Offset")); j++)
                    {
                        commandDescription = commandDescription + paraList.ElementAt(j).Text;
                        if (paraList.ElementAt(j).FollowingTable != null)
                        {
                            testTable = paraList.ElementAt(j).FollowingTable;
                            break;
                        }
                    }
                    List<Offset> commandOffsets = getOffsetCommands(testTable);

                    Command nextCommand = new Command(commandPayloadType, commandPayloadName, commandIsCommand, commandOffsets, commandReplyName, commandReplyValue, commandDescription);
                    commandList.Add(nextCommand);
                }
            }

            return commandList;

        }

        //Object command to test
        public class Commands
        {
            public string Name { get; set; }

            public String Description { get; set; }
        }

        public static List<Offset> getOffsetCommands(Xceed.Words.NET.Table dataTable)
        {
            List<Offset> offsetList = new List<Offset>();

            string offsetOffset, offsetMask, offsetType, offsetUnits, offsetDescription;

            var rowList = dataTable.Rows;
            //the first row just says "offset", "mask","type","units", etc. skip it.
            for (int i = 1; i < rowList.Count; i++)
            {
                var cellList = rowList.ElementAt(i).Cells;

                //get the offset values
                offsetOffset = cellList.ElementAt(0).Paragraphs.ElementAt(0).Text;

                //get the mask
                offsetMask = "";
                for (int j = 0; j < cellList.ElementAt(1).Paragraphs.Count; j++)
                {
                    offsetMask = offsetMask + "\n" + cellList.ElementAt(1).Paragraphs.ElementAt(j).Text;
                }

                //get the type
                offsetType = cellList.ElementAt(2).Paragraphs.ElementAt(0).Text;

                //get the units
                offsetUnits = cellList.ElementAt(3).Paragraphs.ElementAt(0).Text;

                //get the description
                offsetDescription = "";
                for (int k = 0; k < cellList.ElementAt(4).Paragraphs.Count; k++)
                {
                    offsetDescription = offsetDescription + "\n" + cellList.ElementAt(4).Paragraphs.ElementAt(k).Text;
                }

                //add to the list
                Offset nextOffset = new Offset(offsetOffset, offsetMask, offsetType, offsetUnits, offsetDescription);
                offsetList.Add(nextOffset);
            }

            return offsetList;
        }
    }
}
