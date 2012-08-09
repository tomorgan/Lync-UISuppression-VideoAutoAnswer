Video AutoAnswer using Microsoft Lync in UI Suppression Mode
==================================

####*Lync wrapper and WPF control to allow you to easily create UI Suppression mode applications which auto-answer incoming video calls.*
---

##Current Functionality
* UISuppresion Mode WPF application
* Presence,Personal Note, Name, Photo exposed for the "watched" user
* Auto-answer video call, two-way video sharing
* Place call to "watched" user, two-way video sharing
* Uses the [Lync 2010 Client SDK](http://www.microsoft.com/en-us/download/details.aspx?id=18898) (which you will need).

##Quick HeadStart
Get going quickly with a sample project. This wrapper is being used over at [Scott Hanselman](http://www.hanselman.com/blog/)'s [LyncAutoAnswer](http://shanselman.github.com/LyncAutoAnswer/) project.

##Things to know
* There is a WPF control - `LyncUISupressionWrapper.Controls.VideoWindow` -  which displays either incoming or outgoing video.
* The LyncUISupressionWrapper.LyncModel object is a singleton, so call it with `LyncModel.Instance;`.
* You must call `.SignIn` as one of the first things you do, otherwise nothing will work!
* When debugging, if you halt your application mid-call/conversation you will leave the underlying Lync process (communicator.exe) in a running state. You will need to kill that process before running again, otherwise you'll get errors that "{Client is in state 'SignedIn', expected 'Uninitialized'}". Exiting the application normally is fine.
* You need to be in [UI Suppression Mode](http://msdn.microsoft.com/en-us/library/hh345230.aspx) for this to work. UI Suppression is set in the registry - keys are in the [Downloads](https://github.com/tomorgan/Lync-UISuppression-VideoAutoAnswer/downloads) section.

##Authors
This project has been written by the [Modality Systems](http://www.modalitysystems.com) Product Development Team, who blog about building interesting and innovative solutions on the Microsoft Lync platform at [ModalityDev](http://www.modalitydev.co.uk).