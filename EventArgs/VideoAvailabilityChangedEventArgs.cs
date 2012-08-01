using System;
using LyncUISupressionWrapper.Controls.Enums;

namespace LyncUISupressionWrapper
{
    public class VideoAvailabilityChangedEventArgs : EventArgs
    {
        public VideoDirection Direction { get; private set; }
        public bool IsAvailable { get; private set; }
        public LyncVideoWindow VideoWindow { get; private set; }        

        private VideoAvailabilityChangedEventArgs() { }
        internal VideoAvailabilityChangedEventArgs(LyncVideoWindow videoWindow, VideoDirection direction, bool isAvailable)
        {
            VideoWindow = videoWindow;
            Direction = direction;
            IsAvailable = isAvailable;
        }
    }
}