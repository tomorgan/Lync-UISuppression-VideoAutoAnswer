using System;
using Microsoft.Lync.Model;

namespace LyncUISupressionWrapper
{
    public class PresenceInformationEventArgs : EventArgs
    {
        public ContactAvailability Presence { get; private set; }

        private PresenceInformationEventArgs() { }
        internal PresenceInformationEventArgs(ContactAvailability pState)
        {
            Presence = pState;
        }
    }

}

