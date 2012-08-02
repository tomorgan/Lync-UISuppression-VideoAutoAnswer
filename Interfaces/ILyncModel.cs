using System;
using System.Windows.Media.Imaging;

namespace LyncUISupressionWrapper
{
    public interface ILyncModel : IDisposable
    {
        event EventHandler<StateChangedEventArgs> StateChanged;
        event EventHandler<VideoAvailabilityChangedEventArgs> VideoAvailabilityChanged;
        event EventHandler<PresenceInformationEventArgs> PresenceChanged;

        ApplicationState State { get; }

        void SignIn(string userAtHost, string domainAndUsername, string password);
        void SubscribeForPresenceChange(string sipUri);
        void StartCall(string sipUri);
        string GetContactDisplayName(string sipUri);
        BitmapImage GetContactPhoto(string sipUri);
    }
}