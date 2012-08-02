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

        public void SubscribeForPresenceChange(string sipUri)
        {
            _contact = Lync.LyncContactManager.GetContactByUri(sipUri);
            _contact.ContactInformationChanged += contact_ContactInformationChanged;



            var subscription = Lync.LyncContactManager.CreateSubscription();
            subscription.AddContact(_contact);
            subscription.Subscribe(ContactSubscriptionRefreshRate.High, new List<ContactInformationType>() { ContactInformationType.Availability });
        }

        public string GetContactDisplayName(string sipUri)
        {
            _contact = Lync.LyncContactManager.GetContactByUri(sipUri);


            if (_contact != null)
            {

                while (_contact.ContactManager == null)
                {
                    System.Threading.Thread.Sleep(500);
                }

                string contact;
                try
                {
                    contact = _contact.GetContactInformation(ContactInformationType.DisplayName).ToString();
                    return contact;
                }
                catch (Microsoft.Lync.Model.NotReadyException)
                {
                    //TODO: this is horrible, but the data that is returned from GetContactInformation seems to be back-filled AFTER returning.
                    //Therefore, sometimes a NotReadyException is thrown when you attempt to use the data.
                    //try once more
                    System.Threading.Thread.Sleep(1000);
                    contact = _contact.GetContactInformation(ContactInformationType.DisplayName).ToString();
                    return contact;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public BitmapImage GetContactPhoto(string sipUri)
        {
            try
            {
                _contact = Lync.LyncContactManager.GetContactByUri(sipUri);
                if (_contact != null)
                {

                    while (_contact.ContactManager == null)
                    {
                        System.Threading.Thread.Sleep(500);
                    }


                    BitmapImage userImageBitMap = new BitmapImage();
                    System.IO.Stream photoStream;

                    try
                    {
                        photoStream = _contact.GetContactInformation(ContactInformationType.Photo) as Stream;
                    }
                    catch (Microsoft.Lync.Model.NotReadyException ex)
                    {
                        //TODO: this is horrible, but the data that is returned from GetContactInformation seems to be back-filled AFTER returning.
                        //Therefore, sometimes a NotReadyException is thrown when you attempt to use the data.
                        //try once more
                        System.Threading.Thread.Sleep(1000);
                        photoStream = _contact.GetContactInformation(ContactInformationType.Photo) as Stream;
                    }


                    if (photoStream != null)
                    {
                        userImageBitMap.BeginInit();
                        userImageBitMap.StreamSource = photoStream;
                        userImageBitMap.EndInit();
                        userImageBitMap.Freeze();
                        return userImageBitMap;
                    }
                    System.Diagnostics.Debug.WriteLine("photoStream is null");
                    return null;
                }
                System.Diagnostics.Debug.WriteLine("contact is null");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("exception thrown");
                return null;
            }

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
        }




        #endregion




    }

}
