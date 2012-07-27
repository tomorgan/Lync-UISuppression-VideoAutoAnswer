using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LyncUISupressionWrapper
{
   public  class StateChangedEventArgs:EventArgs
    {
        public ApplicationState State { get; private set; }

        private StateChangedEventArgs() { }
        internal StateChangedEventArgs(ApplicationState pState)
        {
            State = pState;
        }
    }
}
