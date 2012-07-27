using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

