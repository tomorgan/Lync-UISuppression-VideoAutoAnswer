using System;
using System.Collections.Generic;
using LyncUISupressionWrapper.Controls.Enums;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Internal;

namespace LyncUISupressionWrapper
{
    internal static class Lync
    {
        #region Fields

        private static LyncClient _client;

        private static Conversation _conversation = null;

        private static bool _incomingVideoStreamStarted = false;

        private static string _outgoingSipUri;

        //Win32 constants:                  WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
        private const long lEnableWindowStyles = 0x40000000L | 0x02000000L | 0x04000000L;
        //Win32 constants:                   WS_POPUP| WS_CAPTION | WS_SIZEBOX
        private const long lDisableWindowStyles = 0x80000000 | 0x00C00000 | 0x00040000L;
        private const int OATRUE = -1;
        private static readonly List<ChannelState> _outgoingChannelStates = new List<ChannelState>() { ChannelState.Send, ChannelState.SendReceive };
        private static readonly List<ChannelState> _incomingChannelStates = new List<ChannelState>() { ChannelState.Receive, ChannelState.SendReceive };

        #endregion

        #region Events

        internal static event EventHandler SignedIn = delegate { };
        private static void OnSignedIn()
        {
            SignedIn(null, EventArgs.Empty);
        }

        internal static event EventHandler SignInFailed = delegate { };
        private static void OnSignInFailed()
        {
            SignInFailed(null, EventArgs.Empty);
        }

        internal static event EventHandler CallAccepted = delegate { };
        private static void OnCallAccepted()
        {
            CallAccepted(null, EventArgs.Empty);
        }

        internal static event EventHandler<VideoAvailabilityChangedEventArgs> VideoAvailabilityChanged = delegate { };
        private static void OnVideoAvailabilityChanged(LyncVideoWindow videoWindow, VideoDirection direction, bool isAvailable)
        {
            VideoAvailabilityChanged(null, new VideoAvailabilityChangedEventArgs(videoWindow, direction, isAvailable));
        }

        internal static event EventHandler CallEnded = delegate { };
        private static void OnCallEnded()
        {
            CallEnded(null, EventArgs.Empty);
        }

        internal static event EventHandler CallDeclined = delegate { };
        private static void OnCallDeclined()
        {
            CallDeclined(null, EventArgs.Empty);
        }

        #endregion

        #region Constructors

        static Lync()
        {
        }

        #endregion

        #region Public Methods

        public static ContactManager LyncContactManager { get; private set; }


        public static void Initialize()
        {
            try
            {
                _client = LyncClient.GetClient();
                LyncContactManager = _client.ContactManager;
            }
            catch (ClientNotFoundException clne)
            {
                throw new Exception("Ensure Lync client is installed and is configured to run in Full UI suppression mode", clne);
            }

            if (!_client.InSuppressedMode)
                throw new ApplicationException("Ensure Lync client is configured to run in Full UI suppression mode");

            if (_client.State != ClientState.Uninitialized)
                throw new ApplicationException(string.Format("Client is in state '{0}', expected '{1}'", _client.State, ClientState.Uninitialized));

            _client.EndInitialize(_client.BeginInitialize(null, null));

            _client.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;
            _client.ConversationManager.ConversationRemoved += ConversationManager_ConversationRemoved;
        }

        static void ConversationManager_ConversationAdded(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {
            var conversation = e.Conversation;



            // Note - only handles 1 conversation at time
            if (_conversation != null)
                conversation.End();


            Console.WriteLine(conversation.Modalities[ModalityTypes.AudioVideo].State);

            // TODO - Check state
            //if (!_conversation.Modalities.ContainsKey(ModalityTypes.AudioVideo) || _conversation.Modalities[ModalityTypes.AudioVideo].State != ModalityState.Notified) ;

            // TODO - Locking?
            _conversation = conversation;

            var avModality = (AVModality)_conversation.Modalities[ModalityTypes.AudioVideo];

            if (avModality.State == ModalityState.Notified)
            {
                //incoming call

                avModality.ModalityStateChanged += avModality_ModalityStateChanged;
                avModality.Accept();

                OnCallAccepted();

            }
            else
            {
                //outgoing call

                conversation.ParticipantAdded += conversation_ParticipantAdded;
                conversation.StateChanged += conversation_StateChanged;

                if (conversation.CanInvoke(ConversationAction.AddParticipant))
                {
                    var contact = _client.ContactManager.GetContactByUri(_outgoingSipUri);
                    conversation.AddParticipant(contact);
                }

            }
        }

        static void conversation_StateChanged(object sender, ConversationStateChangedEventArgs e)
        {
            if (e.NewState == ConversationState.Terminated)
            {
                OnCallDeclined();
            }
        }



        static void conversation_ParticipantAdded(object source, ParticipantCollectionChangedEventArgs data)
        {
            if (data.Participant.IsSelf != true)
            {
                if (((Conversation)source).Modalities[ModalityTypes.AudioVideo].CanInvoke(ModalityAction.Connect))
                {
                    object[] asyncState = { ((Conversation)source).Modalities[ModalityTypes.AudioVideo], "CONNECT" };

                    ((Conversation)source).Modalities[ModalityTypes.AudioVideo].ModalityStateChanged += avModality_ModalityStateChanged;
                    ((Conversation)source).Modalities[ModalityTypes.AudioVideo].BeginConnect(EndConnect, asyncState);
                }
            }
        }

        private static void EndConnect(IAsyncResult ar)
        {
            Object[] asyncState = (Object[])ar.AsyncState;

            if (ar.IsCompleted == true)
            {
                ((AVModality)asyncState[0]).EndConnect(ar);
            }

        }



        static void ConversationManager_ConversationRemoved(object sender, ConversationManagerEventArgs e)
        {
            // TODO - this will probably kill the existing conversation, if a new one comes in - change it!
            _conversation = null;
            OnCallEnded();
        }

        static void avModality_ModalityStateChanged(object sender, ModalityStateChangedEventArgs e)
        {
            if (e.NewState == ModalityState.Connected)
            {
                var videoChannel = ((AVModality)_conversation.Modalities[ModalityTypes.AudioVideo]).VideoChannel;


                videoChannel.StateChanged += VideoChannel_StateChanged;

                //TODO: race condition needs sorting out
                System.Threading.Thread.Sleep(1000 * 5);
                if (videoChannel.CanInvoke(ChannelAction.Start))
                    videoChannel.BeginStart(videoChannelEndStart, videoChannel);
            }
        }

        private static void videoChannelEndStart(IAsyncResult result)
        {
            try
            {
                VideoChannel channel = (VideoChannel)result.AsyncState;
                channel.EndStart(result);
                RaiseVideoAvailable(channel.CaptureVideoWindow, VideoDirection.Outgoing);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void VideoChannel_StateChanged(object sender, ChannelStateChangedEventArgs e)
        {
            var videoChannel = (VideoChannel)sender;

            if (_incomingChannelStates.Contains(e.NewState) && !_incomingChannelStates.Contains(e.OldState))    // Incoming newly available
            {
                RaiseVideoAvailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }
            else if (!_incomingChannelStates.Contains(e.NewState) && _incomingChannelStates.Contains(e.OldState))    // Incoming newly unavailable
            {
                RaiseVideoUnavailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }

            if (e.NewState == ChannelState.Send)
            {
                // If outgoing is newly available, then raise the Available event
                if (!_outgoingChannelStates.Contains(e.OldState))
                    RaiseVideoAvailable(videoChannel.CaptureVideoWindow, VideoDirection.Outgoing);

                // If incoming was previously available, then raise the Unavailable event
                if (_incomingVideoStreamStarted && _incomingChannelStates.Contains(e.OldState))
                    RaiseVideoUnavailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }
            else if (e.NewState == ChannelState.Receive)
            {
                // If outgoing was previously available, then raise the Unavailable event
                if (_outgoingChannelStates.Contains(e.OldState))
                    RaiseVideoUnavailable(videoChannel.CaptureVideoWindow, VideoDirection.Outgoing);

                // If incoming is newly available, then raise the Available event
                if (_incomingVideoStreamStarted && !_incomingChannelStates.Contains(e.OldState))
                    RaiseVideoAvailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }
            else if (e.NewState == ChannelState.SendReceive)
            {
                // If outgoing is newly available, then raise the Available event
                if (!_outgoingChannelStates.Contains(e.OldState))
                    RaiseVideoAvailable(videoChannel.CaptureVideoWindow, VideoDirection.Outgoing);

                // If incoming is newly available, then raise the Available event
                if (_incomingVideoStreamStarted && !_incomingChannelStates.Contains(e.OldState))
                    RaiseVideoAvailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }
            else
            {
                // If outgoing was previously available, then raise the Unavailable event
                if (_outgoingChannelStates.Contains(e.OldState))
                    RaiseVideoUnavailable(videoChannel.CaptureVideoWindow, VideoDirection.Outgoing);

                // If incoming was previously available, then raise the Unavailable event
                if (_incomingVideoStreamStarted && _incomingChannelStates.Contains(e.OldState))
                    RaiseVideoUnavailable(videoChannel.RenderVideoWindow, VideoDirection.Incoming);
            }

        }

        public static void SignIn(string userAtHost, string domainAndUsername, string password)
        {
            try
            {
                var uri = userAtHost;
                if (userAtHost.StartsWith("sip:"))
                    uri = userAtHost.Remove(0, 4);

                _client.BeginSignIn(uri, domainAndUsername, password, SignInCallback, _client);
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }

        public static void Dispose()
        {
            if (_client.State == ClientState.SignedIn)
            {
                _client.BeginSignOut(EndSignout, _client);
            }
            else
            {
                _client.BeginShutdown(EndShutdown, _client);
            }
        }

        public static void PlaceCall(string sipUri)
        {
            _outgoingSipUri = sipUri;
            var conversation = _client.ConversationManager.AddConversation();
        }



        #endregion

        #region Signin

        private static void SignInCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.IsCompleted)
                {
                    var client = ar.AsyncState as LyncClient;
                    client.EndSignIn(ar);

                    OnSignedIn();
                }
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }

        #endregion

        #region Private methods

        private static void RaiseVideoAvailable(VideoWindow videoWindow, VideoDirection direction)
        {
            // Gets the current window style and modifies it
            long currentStyle = videoWindow.WindowStyle;
            currentStyle = currentStyle & ~lDisableWindowStyles;
            currentStyle = currentStyle | lEnableWindowStyles;
            videoWindow.WindowStyle = (int)currentStyle;

            videoWindow.Height = 100;
            videoWindow.Width = 100;

            _incomingVideoStreamStarted = true;
            OnVideoAvailabilityChanged(new LyncVideoWindow(videoWindow), direction, true); // Allow any listeners to attach the video window, then make visible
            videoWindow.Visible = OATRUE;
        }

        private static void RaiseVideoUnavailable(VideoWindow videoWindow, VideoDirection direction)
        {
            _incomingVideoStreamStarted = false;
            OnVideoAvailabilityChanged(new LyncVideoWindow(videoWindow), direction, false);
        }

        private static void EndShutdown(IAsyncResult result)
        {
            try
            {
                var client = (LyncClient)result.AsyncState;
                client.EndShutdown(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void EndSignout(IAsyncResult result)
        {
            try
            {
                var client = (LyncClient)result.AsyncState;
                client.EndSignOut(result);
                _client.BeginShutdown(EndShutdown, _client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}