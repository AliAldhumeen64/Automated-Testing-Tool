using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationDrawerPopUpMenu2
{
    public class Header
    {
        private static UInt32 counter = 1;
        private UInt32 synckey;
        private UInt32 messageIdentifier; //what number message this is in the connection
        private UInt32 payloadType;
        private UInt32 payloadSize;
        private UInt32 timestampMSB = 0;
        private UInt32 timestampLSB = 0;
        private Byte[] fullHeader;

        //this probably shouldnt be called
        public Header()
        {
            synckey = 0;
            messageIdentifier = counter;
            counter++;
            payloadType = 0;
            payloadSize = 0;
        }

        public Header(UInt32 newSyncKey, UInt32 newPayloadType, UInt32 newPayloadSize)
        {

        }
    }
}
