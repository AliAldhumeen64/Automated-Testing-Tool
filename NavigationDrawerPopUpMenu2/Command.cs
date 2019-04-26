using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace NavigationDrawerPopUpMenu2
{
    public class Command
    {
        protected UInt32 payloadType;
        protected string payloadName;
        protected bool isCommand;
        protected List<Offset> offsetList;
        protected string replyName;
        protected UInt32 replyValue;
        protected string description;

        public Command()
        {
            payloadType = 0;
            payloadName = "";
            isCommand = false;
            offsetList = null;
            replyName = "";
            replyValue = 0;
            description = "";
        }

        public Command(UInt32 newPayloadType, string newPayloadName, bool newIsCommand, List<Offset> newOffsetList, string newReplyName, UInt32 newReplyValue, string newDescription)
        {
            payloadType = newPayloadType;
            payloadName = newPayloadName;
            isCommand = newIsCommand;
            offsetList = newOffsetList;
            replyName = newReplyName;
            replyValue = newReplyValue;
            description = newDescription;
        }

        public UInt32 getPayloadType()
        {
            return payloadType;
        }
        public void setPayloadType(UInt32 newPayloadType)
        {
            payloadType = newPayloadType;
        }
        public string getPayloadName()
        {
            return payloadName;
        }
        public void setPayloadName(string newPayloadName)
        {
            payloadName = newPayloadName;
        }
        public bool getIsCommand()
        {
            return isCommand;
        }
        public void setIsCommand(bool newIsCommand)
        {
            isCommand = newIsCommand;
        }
        public List<Offset> getOffsetList()
        {
            return offsetList;
        }
        public void setOffsetList(List<Offset> newOffsetList)
        {
            while(offsetList.Count > 0)
            {
                offsetList.RemoveAt(0);
            }
            offsetList = newOffsetList;
        }
        public void setReplyName(string newReplyName)
        {
            replyName = newReplyName;
        }
        public string getReplyName()
        {
            return replyName;
        }
        public void setReplyValue(UInt32 newReplyValue)
        {
            replyValue = newReplyValue;
        }
        public UInt32 getReplyValue()
        {
            return replyValue;
        }
        public void setDescription(string newDescription)
        {
            description = newDescription;
        }
        public string getDescription()
        {
            return description;
        }

    }
}
