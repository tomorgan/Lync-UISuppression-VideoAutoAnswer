using System;
using System.Windows.Media.Imaging;

namespace LyncUISupressionWrapper
{
    public interface ILyncModel : IDisposable
    {
        event EventHandler<StateChangedEventArgs> StateChanged;
        event EventHandler<VideoAvailabilityChangedEventArgs> VideoAvailabilityChanged;
        event EventHandler<PresenceInformationEventArgs> PresenceChanged;
        event EventHandler<StringValueInformationEventArgs> ActivityChanged;
        event EventHandler<StringValueInformationEventArgs> DisplayNameChanged;
        event EventHandler<PhotoChangedEventArgs> PhotoChanged;

        ApplicationState State { get; }

        void SignIn(string userAtHost, string domainAndUsername, string password);
        void SubscribeForInformationUpdates(string sipUri);
        void StartCall(string sipUri);
       
    }
}