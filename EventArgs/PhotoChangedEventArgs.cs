using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace LyncUISupressionWrapper
{
  public  class PhotoChangedEventArgs : EventArgs
    {
          public BitmapImage Photo{ get; private set; }

        private PhotoChangedEventArgs() { }
        internal PhotoChangedEventArgs(BitmapImage pValue)
        {
            Photo = pValue;
        }

    }
}
