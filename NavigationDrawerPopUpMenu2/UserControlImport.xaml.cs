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
        public UserControlImport()
        {
            //parse();
           
            InitializeComponent();
            OpenFileClicked();
        }

        public void OpenFileClicked()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                Console.WriteLine("File was chosen");
            }
        }

        public static void parse()
        {
            DocX docx = DocX.Load(@"C:\temp\ICD_SeniorProject.docx");
            List<Command> commandList = new List<Command>();
            List<Offset> currentOffsets;

            //var coreProperties = docx.CoreProperties;
            //int count = coreProperties.Count;
            //for(int i = 0; i < count; i++)
            //{
            //    Console.WriteLine(coreProperties.ElementAt(i));
            //}

            var paraList = docx.Paragraphs;

            int syncKey = getSyncKey(docx);
            Debug.WriteLine("The Sync key is: ");
            Debug.WriteLine(syncKey);
            Debug.WriteLine(" ");

            commandList = getCommands(docx);
            Debug.WriteLine(commandList.ElementAt(0).getPayloadName());
            Debug.WriteLine(commandList.ElementAt(0).getPayloadType());
            Debug.WriteLine(commandList.ElementAt(0).getIsCommand());
            Debug.WriteLine(" ");

            for (int i = 1; i < commandList.Count; i++)
            {
                Debug.WriteLine(commandList.ElementAt(i).getPayloadName());
                Debug.WriteLine(commandList.ElementAt(i).getPayloadType());
                Debug.WriteLine(commandList.ElementAt(i).getIsCommand());
                Debug.WriteLine("Offset List: ");
                currentOffsets = commandList.ElementAt(i).getOffsetList();
                for (int j = 0; j < currentOffsets.Count; j++)
                {
                    Debug.WriteLine("Offset: " + currentOffsets.ElementAt(j).getOffsetValue());
                    Debug.WriteLine("Mask: " + currentOffsets.ElementAt(j).getMask());
                    Debug.WriteLine("Type: " + currentOffsets.ElementAt(j).getType());
                    Debug.WriteLine("Units: " + currentOffsets.ElementAt(j).getUnits());
                    Debug.WriteLine("Description: " + currentOffsets.ElementAt(j).getDescription());
                }
                Debug.WriteLine(" ");
            }

            //use for debugging, outputs the whole document
            //string text;
            //for (int j = 0; j < paraList.Count; j++)
            //{
            //    text = paraList.ElementAt(j).Text;
            //    Debug.WriteLine(text);
            //    Debug.WriteLine(j);
            //}
        }

        private static int getSyncKey(DocX docx)
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

        private static List<Command> getCommands(DocX docx)
        {
            List<Command> commandList = new List<Command>();
            var paraList = docx.Paragraphs;

            Xceed.Words.NET.Paragraph currentP;
            string commandPayloadType, commandPayloadName;
            bool commandIsCommand;

            commandPayloadType = "00000000";
            commandPayloadName = "no message payload";
            commandIsCommand = false;
            List<Offset> firstCommandOffsets = null;
            Command firstCommand = new Command(commandPayloadType, commandPayloadName, commandIsCommand, firstCommandOffsets);
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
                    }
                    else
                    {
                        commandIsCommand = false;
                        commandPayloadName = paraList.ElementAt(i + 1).Text + " reply";
                    }
                    commandPayloadType = paraList.ElementAt(i + 3).Text;

                    //should never actually be null
                    Xceed.Words.NET.Table testTable = null;
                    for (int j = i + 7; !(paraList.ElementAt(j).Equals("Offset")); j++)
                    {
                        if (paraList.ElementAt(j).FollowingTable != null)
                        {
                            testTable = paraList.ElementAt(j).FollowingTable;
                            break;
                        }
                    }
                    List<Offset> commandOffsets = getOffsetCommands(testTable);

                    Command nextCommand = new Command(commandPayloadType, commandPayloadName, commandIsCommand, commandOffsets);
                    commandList.Add(nextCommand);
                }
            }

            return commandList;
        }

        private static List<Offset> getOffsetCommands(Xceed.Words.NET.Table dataTable)
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
