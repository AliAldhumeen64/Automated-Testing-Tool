using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace NavigationDrawerPopUpMenu2
{
    class Offset
    {
        private string offsetValue;
        private string mask;
        private string type;
        private string units;
        private string description;

        public Offset()
        {
            offsetValue = "";
            mask = "";
            type = "";
            units = "";
            description = "";
        }

        public Offset(string newOffsetValue, string newMask, string newType, string newUnits, string newDescription)
        {
            offsetValue = newOffsetValue;
            mask = newMask;
            type = newType;
            units = newUnits;
            description = newDescription;
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

    }
}
