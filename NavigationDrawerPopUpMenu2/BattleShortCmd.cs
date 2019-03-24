using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigationDrawerPopUpMenu2
{
    //This is a super temporary class, was some garbage christian wrote, using it for reference
    class BattleShortCmd
    {
        private readonly byte[] m_Bytes = new byte[28];

        public BattleShortCmd()
        {
            byte[] valAsBytes = BitConverter.GetBytes(0x12345678);
            Array.Copy(valAsBytes, 0, m_Bytes, 0, sizeof(uint));
        }

        public uint SyncKey { get { return 0x12345678; } }

        public uint MessageID
        {
            get { return 0x12345678; }
            set
            {
                byte[] valAsBytes = BitConverter.GetBytes(value);
                Array.Copy(valAsBytes, 0, m_Bytes, 4, sizeof(uint));
            }
        }

        public byte[] getCMD()
        {
            return m_Bytes;
        }
    }
}
