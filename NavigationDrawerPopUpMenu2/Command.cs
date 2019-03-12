using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace NavigationDrawerPopUpMenu2
{
    class Command
    {
        protected string payloadType;
        protected string payloadName;
        protected bool isCommand;
        protected List<Offset> offsetList;

        public Command()
        {
            payloadType = "";
            payloadName = "";
            isCommand = false;
            offsetList = null;
        }

        public Command(string newPayloadType, string newPayloadName, bool newIsCommand, List<Offset> newOffsetList)
        {
            payloadType = newPayloadType;
            payloadName = newPayloadName;
            isCommand = newIsCommand;
            offsetList = newOffsetList;
        }

        public string getPayloadType()
        {
            return payloadType;
        }
        public void setPayloadType(string newPayloadType)
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
            offsetList = newOffsetList;
        }
    }
}
