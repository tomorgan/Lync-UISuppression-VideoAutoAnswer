/* Copyright (C) 2012 Modality Systems - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the Microsoft Public License, a copy of which 
 * can be seen at: http://www.microsoft.com/en-us/openness/licenses.aspx
 * 
 * http://www.LyncAutoAnswer.com
*/

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
