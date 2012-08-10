/* Copyright (C) 2012 Modality Systems - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the Microsoft Public License, a copy of which 
 * can be seen at: http://www.microsoft.com/en-us/openness/licenses.aspx
 * 
 * http://www.LyncAutoAnswer.com
*/

using System;
using System.Windows.Media.Imaging;

namespace LyncUISupressionWrapper
{
    public interface ILyncModel 
    {
        event EventHandler<StateChangedEventArgs> StateChanged;
        event EventHandler<VideoAvailabilityChangedEventArgs> VideoAvailabilityChanged;
        event EventHandler<PresenceInformationEventArgs> PresenceChanged;
        event EventHandler<StringValueInformationEventArgs> ActivityChanged;
        event EventHandler<StringValueInformationEventArgs> DisplayNameChanged;
        event EventHandler<PhotoChangedEventArgs> PhotoChanged;

        ApplicationState State { get; }

        void SignIn(string userAtHost, string domainAndUsername, string password);
        void SignOut();
        void Shutdown();

        void SubscribeForInformationUpdates(string sipUri);
        void StartCall(string sipUri);
       
    }
}