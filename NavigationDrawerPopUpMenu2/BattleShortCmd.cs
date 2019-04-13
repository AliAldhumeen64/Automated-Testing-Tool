using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationDrawerPopUpMenu2
{
    ////This is a super temporary class, was some garbage christian wrote, using it for reference
    //public class BattleShortCmd
    //{
    //    private readonly byte[] m_Bytes = new byte[28];

    //    public BattleShortCmd()
    //    {
    //        byte[] valAsBytes = BitConverter.GetBytes(0x12345678);
    //        Array.Copy(valAsBytes, 0, m_Bytes, 0, sizeof(uint));
    //    }

    //    public uint SyncKey { get { return 0x12345678; } }

    //    public uint MessageID
    //    {
    //        get { return 0x12345678; }
    //        set
    //        {
    //            byte[] valAsBytes = BitConverter.GetBytes(value);
    //            Array.Copy(valAsBytes, 0, m_Bytes, 4, sizeof(uint));
    //        }
    //    }

    //    public byte[] getCMD()
    //    {
    //        return m_Bytes;
    //    }
    //}

    public class BaseMessage
    {
        protected const int WORD_SIZE_BYTES = 4;
        private readonly byte[] m_Header;
        protected static uint messageIdCounter = 1;

        public BaseMessage(Command commandToSend)
        {
            const uint HEADER_SIZE_WORDS = 6;
            m_Header = new byte[HEADER_SIZE_WORDS * WORD_SIZE_BYTES];
            //MessageStartIndicator = 0x12345678;
            MessageStartIndicator = UserControlImport.syncKey;
            MessageId = messageIdCounter;
            messageIdCounter++;
            PayloadType = commandToSend.getPayloadType();
            TimeStampLsb = 0;
            TimeStampMsb = 0;
            PayloadSize = Convert.ToUInt32(GetPayloadBytes(commandToSend).Length);
        }

        //public BaseMessage(byte[] replyBytes, Command replyCommand)
        //{

        //}

        public uint MessageStartIndicator
        {
            get
            {
                return BitConverter.ToUInt32(m_Header, 0);
            }

            private set
            {
                byte[] dataAsBytes = BitConverter.GetBytes(value);
                CopyBytes(dataAsBytes, m_Header, 0);
            }
        }
        public uint MessageId
        {
            get
            {
                return BitConverter.ToUInt32(m_Header, 1 * WORD_SIZE_BYTES);
            }
            set
            {
                byte[] dataAsBytes = BitConverter.GetBytes(value);
                CopyBytes(dataAsBytes, m_Header, 1 * WORD_SIZE_BYTES);

            }
        }

        //public enum PayloadType_E
        //{
        //    Payload_None = 0,
        //    Payload_System_State_Cmd = 1,
        //    Payload_Beam_Cmd = 2,
        //    Payload_NullBeam_Cmd = 3,
        //    Payload_Battleshort_Cmd = 14
        //}

        public UInt32 PayloadType
        {
            get
            {
                uint value = BitConverter.ToUInt32(m_Header, 2 * WORD_SIZE_BYTES);

                // get the bits that make up the payload aspect of this word
                value = (value >> 16);

                // mask out any garbage left over
                return (UInt32)(value & 0xFF);
            }

            protected set
            {
                // get the whole word value
                uint existingValue = BitConverter.ToUInt32(m_Header, 2 * WORD_SIZE_BYTES);

                // shift the value 16 bits to the left and bitwise OR it with the existing value
                uint newValue = (((uint)value) << 16);

                // bitwise or the new value into the existing value (as to not overwrite what may be set
                // as the payload type)
                existingValue |= newValue;

                // get this new value as a byte array
                byte[] dataAsBytes = BitConverter.GetBytes((uint)existingValue);

                // copy it back into our main array
                CopyBytes(dataAsBytes, m_Header, 2 * WORD_SIZE_BYTES);
            }
        }

        public uint PayloadSize
        {
            get
            {
                return BitConverter.ToUInt32(m_Header, 3 * WORD_SIZE_BYTES);
            }
            protected set
            {
                byte[] valAsBytes = BitConverter.GetBytes(value);
                CopyBytes(valAsBytes, m_Header, 3 * WORD_SIZE_BYTES);
            }
        }

        public uint TimeStampMsb
        {
            get
            {
                return BitConverter.ToUInt32(m_Header, 4 * WORD_SIZE_BYTES);
            }
            set
            {
                byte[] valAsBytes = BitConverter.GetBytes(value);
                CopyBytes(valAsBytes, m_Header, 4 * WORD_SIZE_BYTES);
            }
        }

        public uint TimeStampLsb
        {
            get
            {
                return BitConverter.ToUInt32(m_Header, 5 * WORD_SIZE_BYTES);
            }
            set
            {
                byte[] valAsBytes = BitConverter.GetBytes(value);
                CopyBytes(valAsBytes, m_Header, 5 * WORD_SIZE_BYTES);
            }
        }

        public byte[] GetByteArray(Command commandToConvert)
        {
            var fullArray = new byte[m_Header.Length + PayloadSize];
            CopyBytes(m_Header, fullArray, 0);
            byte[] payloadBytes = GetPayloadBytes(commandToConvert);

            const int PAYLOAD_OFFSET = 6 * WORD_SIZE_BYTES;
            CopyBytes(payloadBytes, fullArray, PAYLOAD_OFFSET);
            return fullArray;
        }

        protected byte[] GetPayloadBytes(Command commandToConvert)
        {
            List<Offset> commandOffsets= commandToConvert.getOffsetList();
            byte[] payloadBytes = new byte[commandOffsets.Count * WORD_SIZE_BYTES];
            string units;
            int offsetNumber;

            for (int i =0; i < commandOffsets.Count; i++)
            {
                byte[] currentOffset;
                units = commandOffsets.ElementAt(i).getUnits();
                units = units.Trim();
                units = units.ToLower();
                if (units.CompareTo("float") == 0)
                {
                    currentOffset = BitConverter.GetBytes(commandOffsets.ElementAt(i).getMessageFloat());
                }
                else
                {
                    currentOffset = BitConverter.GetBytes(commandOffsets.ElementAt(i).getMessage());
                }
                offsetNumber = Convert.ToInt32(commandOffsets.ElementAt(i).getOffsetValue());
                CopyBytes(currentOffset, payloadBytes, offsetNumber);

                //
                //this is ALL for the replies only, none of the commands to be sent have a range as their offset
                //this is only here currently because I thought we'd need it here, turns out we dont
                //
                // int offsetNumberL, offsetNumberR, dashIndex;
                // string offsetNumberString;
                //offsetNumberString = commandOffsets.ElementAt(i).getOffsetValue();
                //if(offsetNumberString.Contains('-') == true)
                //{
                //    dashIndex = offsetNumberString.IndexOf('-');
                //    offsetNumberL = Convert.ToInt32(offsetNumberString.Substring(0,dashIndex));
                //    offsetNumberR = Convert.ToInt32(offsetNumberString.Substring(dashIndex + 1));
                //}
                //else
                //{
                //    offsetNumberL = Convert.ToInt32(offsetNumberString);
                //    offsetNumberR = Convert.ToInt32(offsetNumberString);
                //}
                //for(int j =offsetNumberR; j >= offsetNumberL; j--) { 
                //}


            }

            return payloadBytes;
        }

        /// <summary>
        /// Copies a byte array into another byte array.
        /// </summary>
        /// <param name="fromArr">The bytes to be copied into the data array.</param>
        /// <param name="destArr">The array the bytes shall be copied to.</param>
        /// <param name="offset">Where in the destination array the bytes shall be copied to.</param>
        protected static void CopyBytes(byte[] fromArr, byte[] destArr, int offsetInDest)
        {
            for (int i = offsetInDest; i < offsetInDest + fromArr.Length; ++i)
            {
                destArr[i] = fromArr[i - offsetInDest];
            }
        }






    }


}
