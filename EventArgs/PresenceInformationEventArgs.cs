/* Copyright (C) 2012 Modality Systems - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the Microsoft Public License, a copy of which 
 * can be seen at: http://www.microsoft.com/en-us/openness/licenses.aspx
 * 
 * http://www.LyncAutoAnswer.com
*/

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

