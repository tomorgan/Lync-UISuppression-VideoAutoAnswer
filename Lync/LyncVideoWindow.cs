using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation.AudioVideo;

namespace LyncUISupressionWrapper
{
    public class LyncVideoWindow
    {
        private VideoWindow LyncObject;

        public LyncVideoWindow(VideoWindow lyncVideoObject)
        {
            LyncObject = lyncVideoObject;
        }

        public VideoWindow Video
        {
            get
            {
                return LyncObject;
            }
        }

        public void SetWindowPosition(int left, int top, int width, int height)
        {
            try
            {
                LyncObject.SetWindowPosition(left, top, width, height);
            }
            catch (LyncClientException lcex)
            {
                if (lcex.InternalCode != 0x80040209) return;   // Just swallow the exception that is caused by accessing the window after the conversation is inactive
                throw;
            }
        }

        public int Owner
        {
            get
            {
                return LyncObject.Owner;
            }
            set
            {
                LyncObject.Owner = value;
            }
        }

    }
}
