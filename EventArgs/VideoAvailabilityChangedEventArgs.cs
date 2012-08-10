/* Copyright (C) 2012 Modality Systems - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the Microsoft Public License, a copy of which 
 * can be seen at: http://www.microsoft.com/en-us/openness/licenses.aspx
 * 
 * http://www.LyncAutoAnswer.com
*/

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