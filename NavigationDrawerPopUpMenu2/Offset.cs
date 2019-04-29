using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace NavigationDrawerPopUpMenu2
{
    public class Offset
    {
        //this holds what number offset is to be expected for the command
        //the main functionality missing from the project right now is for offsets with a range of values, which only show up in replies
        //in order to account for this, the offset value would need to be split using the string split command over a '-' character.
        //this would give the values for the start and end offsets for a command.
        public string offsetValue { get; set; } 

        public string mask { get; set; }
        private string type; 
        private string units;
        public string description { get; set; }

        //this says how many inputs need to be read in for this command
        private int parameterCount;
        //this is the message that is converted to a byte array when sending the message for this offset of this command
        private string message;

        public Offset()
        {
            offsetValue = "";
            mask = "";
            type = "";
            units = "";
            description = "";
            parameterCount = 0;
            message = "0";
        }

        public Offset(string newOffsetValue, string newMask, string newType, string newUnits, string newDescription)
        {
            offsetValue = newOffsetValue;
            mask = newMask;
            type = newType;
            units = newUnits;
            description = newDescription;
            parameterCount = 0;
            message = "0";
        }

        public string getOffsetValue()
        {
            return offsetValue;
        }

        public void setOffsetValue(string newOffsetValue)
        {
            offsetValue = newOffsetValue;
        }

        public string getMask()
        {
            return mask;
        }
        public void setMask(string newMask)
        {
            mask = newMask;
        }
        public string getType()
        {
            return type;
        }
        public void setType(string newType)
        {
            type = newType;
        }
        public string getUnits()
        {
            return units;
        }
        public void setUnits(string newUnits)
        {
            units = newUnits;
        }
        public string getDescription()
        {
            return description;
        }
        public void setDescription(string newDescription)
        {
            description = newDescription;
        }

        //
        //This function sets the actual message for an offset
        //pretty much needed
        //the parameters are always Letters where the first one is 'A', then 'B', then 'C', etc.
        //This function does 2 loops through the mask, one to find how many parameters there are as well as when they start and end
        //and a second to properly set the message
        //there could easily be bugs hiding here, should be working as intended though

        public void setMessage(string[] messageValues)
        {
            string newMessage = "";
            parameterCount = 0;

            List<char> seenLetters = new List<char>();
            List<int> seenLettersStartIndexes = new List<int>();
            List<int> seenLettersEndIndexes = new List<int>();
            char currentLetter;
            bool isNewLetter;
            int charsleft;

            //getting rid of all the useless whitespace in the mask
            //a bit of a mess
            string trueMask = mask.Trim(' ');
            trueMask = trueMask.Trim('\n');
            string[] truemaskarray = trueMask.Split(' ');
            trueMask = "";
            for(int n = 0; n < truemaskarray.Length; n++)
            {
                trueMask += truemaskarray[n].Trim(' ');
            }

            //look through whole mask for parameters
            for (int i = 0; i < trueMask.Length; i++)
            {
                currentLetter = trueMask.ElementAt(i);
                //ignore Xs and whitespace in the mask
                if ((currentLetter != ' ') && (currentLetter != '\n'))
                {
                    if (!(currentLetter.Equals('X')))
                    {
                        isNewLetter = true;
                        //search through seenLetters if the currentLetter being looked at has shown up before, it isnt a new parameter and the count shouldnt be incremented
                        for (int j = 0; j < seenLetters.Count; j++)
                        {
                            if ((seenLetters.ElementAt(j).Equals(currentLetter)))
                            {
                                isNewLetter = false;
                                seenLettersEndIndexes[j]++;
                            }

                        }

                        //if it is a new letter, add it to the list and increase the parameter count
                        if (isNewLetter)
                        {
                            seenLetters.Add(currentLetter);
                            seenLettersStartIndexes.Add(i);
                            seenLettersEndIndexes.Add(i);
                            parameterCount++;
                        }
                    }
                }

            }


            //search through now that we know exactly where the first and last index for each input is
            for (int i = 0; i < trueMask.Length; i++)
            {
                currentLetter = trueMask.ElementAt(i);
                //ignore Xs in the mask
                if((currentLetter != ' ') && (currentLetter != '\n'))
                {
                    if (!(currentLetter.Equals('X')))
                    {
                        isNewLetter = true;
                        //search through seenLetters if the currentLetter being looked at has shown up before, it isnt a new parameter and the count shouldnt be incremented
                        for (int j = 0; j < seenLetters.Count; j++)
                        {
                            charsleft = seenLettersEndIndexes[j] - seenLettersStartIndexes[j];
                            if ((seenLetters.ElementAt(j).Equals(currentLetter)))
                            {
                                isNewLetter = false;
                                if ((messageValues[j].Length-1 < charsleft))
                                {
                                    newMessage = newMessage + "0";
                                    seenLettersStartIndexes[j]++;
                                }
                                else if((messageValues[j].Length-1 == charsleft)) //user entered input properly
                                {
                                    newMessage = newMessage + messageValues[j].Substring(0,1);
                                    messageValues[j] = messageValues[j].Substring(1);
                                    seenLettersStartIndexes[j]++;
                                }
                                else
                                {
                                    //reduce length of input by 1, remove a character entered at the front
                                    messageValues[j] = messageValues[j].Substring(1);
                                    UserControlConsole.dc.ConsoleInput = ("ERROR: Input for offset# " + offsetValue + " too long, shortening the offset.");
                                    UserControlConsole.dc.RunCommand();
                                }
                                
                            }

                        }

                        //if it is a new letter, add it to the list and increase the parameter count
                        //this should never happen
                        if (isNewLetter)
                        {
                            string text = messageValues[parameterCount];
                            newMessage = newMessage + text.Substring(0, 1);
                            seenLetters.Add(currentLetter);
                            seenLettersStartIndexes.Add(i);
                            seenLettersEndIndexes.Add(i);
                            parameterCount++;
                        }
                    }
                    else
                    {
                        newMessage = newMessage + '0';
                    }
                }
                
            }

            message = newMessage;
        }

        public void setMessage(string newMessage)
        {
            message = newMessage;
        }

        public UInt32 getMessage()
        {
            UInt32 trueMessage;

            trueMessage = Convert.ToUInt32(message, 2);

            return trueMessage;
        }

        public float getMessageFloat()
        {
            float trueMessage;

            trueMessage = float.Parse(Convert.ToInt32(message, 2).ToString());

            return trueMessage;
        }

        
            
    }
}

