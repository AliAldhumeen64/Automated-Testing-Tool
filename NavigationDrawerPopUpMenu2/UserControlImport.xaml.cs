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
        public static UInt32 syncKey;

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


        public UserControlImport()
        {
          
        InitializeComponent();
            OpenFileClicked();


            if (NavigationDrawerPopUpMenu2.UserControlImport.hasReadFile && !hasDisplayedCommands)
            {
                //this literally grabs the list of commands from the import page
                List<Command> readCommandList = NavigationDrawerPopUpMenu2.UserControlImport.commandList;

                List<UserControlHome.Commands> items = new List<UserControlHome.Commands>();

                for (int i = 0; i < readCommandList.Count; i++)
                {
                    //if a command doesnt have a reply type, it isnt a command
                    if (!(readCommandList.ElementAt(i).getReplyName().Equals("None")))
                        items.Add(new UserControlHome.Commands() { Name = readCommandList.ElementAt(i).getPayloadName(), Id = readCommandList.ElementAt(i).getDescription() });

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
            Debug.WriteLine("The Sync key is: ");
            Debug.WriteLine(syncKey);
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
        public static UInt32 getSyncKey(DocX docx)
        {
            UInt32 syncKey = 0;
            var paraList = docx.Paragraphs;
            Xceed.Words.NET.Paragraph currentP;
            string currentString;
            string syncKeyStr="0"; //this default value should never be returned

            for (int i = 0; i < paraList.Count; i++)
            {
                currentP = paraList.ElementAt(i);
                if (currentP.Text.Contains("Sync Key"))
                {
                    currentString = paraList.ElementAt(i + 1).Text;
                    //this cuts the text down to the sync key value and some text we don't want after it
                    //we have to do this because the sync key has a variable length;
                    currentString.Trim();
                    syncKeyStr = currentString.Split(' ').ElementAt(0);
                    break;
                }
                
            }
            syncKey = Convert.ToUInt32(syncKeyStr, 2);
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
            UInt32 commandReplyValue, commandPayloadType;

            //this is all default values for the first command in the list, which is a command that does nothing
            commandPayloadType = 0;
            commandPayloadName = "no message payload";
            commandIsCommand = false;
            List<Offset> firstCommandOffsets = null;
            commandReplyName = "None";
            commandReplyValue = 0;
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
                        commandReplyValue = Convert.ToUInt32(paraList.ElementAt(i + 7).Text);
                    }
                    else
                    {
                        commandIsCommand = false;
                        commandPayloadName = paraList.ElementAt(i + 1).Text + " reply";
                        commandReplyName = "None";
                        commandReplyValue = 0;
                    }
                    commandPayloadType = Convert.ToUInt32(paraList.ElementAt(i + 3).Text);

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

                    Command nextCommand = new Command(commandPayloadType, commandPayloadName, commandIsCommand,
                        commandOffsets, commandReplyName, commandReplyValue, commandDescription);
                    commandList.Add(nextCommand);
                }
            }

            return commandList;

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
