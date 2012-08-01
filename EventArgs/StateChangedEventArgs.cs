using System;

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
