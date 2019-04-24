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
        public string offsetValue { get; set; }
        private string mask;
        private string type;
        private string units;
        public string description { get; set; }
        private int parameterCount;
        private string message;

        public Offset()
        {
            offsetValue = "";
            mask = "";
            type = "";
            units = "";
            description = "";
            parameterCount = -1;
        }

        public Offset(string newOffsetValue, string newMask, string newType, string newUnits, string newDescription)
        {
            offsetValue = newOffsetValue;
            mask = newMask;
            type = newType;
            units = newUnits;
            description = newDescription;
            parameterCount = -1;
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
            //because the mask is changed, we have to find the parameter count again
            parameterCount = -1;
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

        public void setMessage(int[] messageValues)
        {
            //TO-DO TO-DO TO-DO TO-DO
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

        //
        //This function returns the number of parameters for an offset
        //pretty much needed
        //the parameters are always Letters where the first one is 'A', then 'B', then 'C', etc.
        //Use the int value this function to returns to know how many letters to worry about
        //
        public int getParameterCount()
        {
            //parameterCount is -1 if it hasnt been found before
            if (parameterCount == -1)
            {
                List<char> seenLetters = new List<char>();
                char currentLetter;
                bool isNewLetter;

                //look through whole mask for parameters
                for (int i = 0; i < mask.Length; i++)
                {
                    currentLetter = mask.ElementAt(i);
                    //ignore Xs in the mask
                    if (currentLetter != 'X')
                    {
                        isNewLetter = true;
                        //search through seenLetters if the currentLetter being looked at has shown up before, it isnt a new parameter and the count shouldnt be incremented
                        for (int j = 0; j < seenLetters.Count; j++)
                        {
                            if (seenLetters.ElementAt(j) == currentLetter)
                                isNewLetter = false;
                        }

                        //if it is a new letter, add it to the list and increase the parameter count
                        if (isNewLetter)
                        {
                            seenLetters.Add(currentLetter);
                            parameterCount++;
                        }
                    }
                }
            }

            return parameterCount;
        }
            
    }
}

