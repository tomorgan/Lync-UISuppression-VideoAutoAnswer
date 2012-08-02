using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyncUISupressionWrapper
{
   public class StringValueInformationEventArgs : EventArgs
    {
        public string Value{ get; private set; }

        private StringValueInformationEventArgs() { }
        internal StringValueInformationEventArgs(string pValue)
        {
            Value = pValue;
        }
    }
}
