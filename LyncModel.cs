using System;
using System.Collections.Generic;
using Microsoft.Lync.Model;
using System.IO;
using System.Windows.Media.Imaging;


namespace LyncUISupressionWrapper
{
    public class LyncModel : ILyncModel
    {
        #region Fields

        Contact _contact;

        #endregion

        #region Events

        public event EventHandler<StateChangedEventArgs> StateChanged = delegate { };
        private void OnStateChanged(ApplicationState pState)
        {
            StateChanged(this, new StateChangedEventArgs(pState));
        }

        public event EventHandler<VideoAvailabilityChangedEventArgs> VideoAvailabilityChanged = delegate { };
        private void OnVideoAvailabilityChanged(VideoAvailabilityChangedEventArgs e)
        {
            VideoAvailabilityChanged(null, e);
        }

        public event EventHandler<PresenceInformationEventArgs> PresenceChanged = delegate { };
        private void OnPresenceChanged(ContactAvailability presence)
        {
            PresenceChanged(null, new PresenceInformationEventArgs(presence));
        }

        public event EventHandler<StringValueInformationEventArgs> ActivityChanged = delegate { };
        private void OnActivityChanged(string activity)
        {
            ActivityChanged(null, new StringValueInformationEventArgs(activity));
        }

        public event EventHandler<StringValueInformationEventArgs> DisplayNameChanged = delegate { };
        private void OnDisplayNameChanged(string name)
        {
            DisplayNameChanged(null, new StringValueInformationEventArgs(name));
        }

        public event EventHandler<PhotoChangedEventArgs> PhotoChanged = delegate { };
        private void OnPhotoChanged(BitmapImage image)
        {
            PhotoChanged(null, new PhotoChangedEventArgs(image));
        }


        #endregion

        #region Properties

        private ApplicationState _state = ApplicationState.SigningIn;
        public ApplicationState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                OnStateChanged(_state);
            }
        }

        private static ILyncModel _instance = null;
        public static ILyncModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LyncModel();

                return _instance;
            }
        }

        #endregion

        #region Constructor

        private LyncModel()
        {
            // TODO - Catch instances where Lync signs in/out (e.g. network connectivity issues)
        }

        #endregion

        #region Public Methods

        public void SignIn(string userAtHost, string domainAndUsername, string password)
        {
            Lync.SignedIn += Lync_SignedIn;
            Lync.SignInFailed += Lync_SignInFailed;
            Lync.CallAccepted += Lync_CallAccepted;
            Lync.VideoAvailabilityChanged += Lync_VideoAvailabilityChanged;
            Lync.CallEnded += Lync_CallEnded;
            Lync.CallDeclined += Lync_CallDeclined;

            Lync.Initialize();

            Lync.SignIn(userAtHost, domainAndUsername, password);
        }

        public void SubscribeForInformationUpdates(string sipUri)
        {
            _contact = Lync.LyncContactManager.GetContactByUri(sipUri);
            _contact.ContactInformationChanged += contact_ContactInformationChanged;
            
            var subscription = Lync.LyncContactManager.CreateSubscription();
            subscription.AddContact(_contact);

            List<ContactInformationType> informationItems = new List<ContactInformationType>();
            informationItems.Add(ContactInformationType.Availability);
            informationItems.Add(ContactInformationType.PersonalNote);
            informationItems.Add(ContactInformationType.DisplayName);
            informationItems.Add(ContactInformationType.Photo);

            subscription.Subscribe(ContactSubscriptionRefreshRate.High, informationItems);
        }

       
        public void StartCall(string sipUri)
        {
            Lync.PlaceCall(sipUri);
            State = ApplicationState.CallInProgress;
        }

        public void Dispose()
        {
            Lync.Dispose();
            _instance = null;
        }

        #endregion

        #region Private Methods

        private ContactAvailability GetPresence()
        {
            return (ContactAvailability)_contact.GetContactInformation(ContactInformationType.Availability);
        }

        private string GetPersonalNote()
        {
            return (string)_contact.GetContactInformation(ContactInformationType.PersonalNote);
        }

        private string GetDisplayName()
        {
            return (string)_contact.GetContactInformation(ContactInformationType.DisplayName);
        }

        private BitmapImage GetPhoto()
        {
            Stream photoStream;

            try
            {
                photoStream = _contact.GetContactInformation(ContactInformationType.Photo) as Stream;
            }
            catch (NotReadyException)
            {
                //TODO: is it really true that Lync returns from GetContactInformation before it is ready to show the photo stream?
                System.Threading.Thread.Sleep(1000);
                photoStream = _contact.GetContactInformation(ContactInformationType.Photo) as Stream;
            }
            catch (ItemNotFoundException)
            {
                //no picture available
                return null;
            }

            
            BitmapImage userImageBitMap = new BitmapImage();
                    
                    if (photoStream != null)
                    {
                        userImageBitMap.BeginInit();
                        userImageBitMap.StreamSource = photoStream;
                        userImageBitMap.EndInit();
                        userImageBitMap.Freeze();
                        return userImageBitMap;
                    }
                    return null;
                
        }

        #endregion

        #region Lync Event Handlers

        void Lync_SignedIn(object sender, EventArgs e)
        {
            State = ApplicationState.NoCall;
        }

        private void Lync_SignInFailed(object sender, EventArgs e)
        {
            State = ApplicationState.SignInFailed;
        }

        void Lync_CallAccepted(object sender, EventArgs e)
        {
            State = ApplicationState.CallInProgress;
        }

        void Lync_VideoAvailabilityChanged(object sender, VideoAvailabilityChangedEventArgs e)
        {
            OnVideoAvailabilityChanged(e);
        }

        void Lync_CallEnded(object sender, EventArgs e)
        {
            State = ApplicationState.NoCall;
        }

        void Lync_CallDeclined(object sender, EventArgs e)
        {
            State = ApplicationState.NoCall;
        }


        void contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            if (e.ChangedContactInformation.Contains(ContactInformationType.Availability))
            {
                OnPresenceChanged(GetPresence());
            }
            if (e.ChangedContactInformation.Contains(ContactInformationType.PersonalNote))
            {
                OnActivityChanged(GetPersonalNote());
            }
            if (e.ChangedContactInformation.Contains(ContactInformationType.DisplayName))
            {
                OnDisplayNameChanged(GetDisplayName());
            }
            if (e.ChangedContactInformation.Contains(ContactInformationType.Photo))
            {
                OnPhotoChanged(GetPhoto());
            }
        }




        #endregion




    }

}
