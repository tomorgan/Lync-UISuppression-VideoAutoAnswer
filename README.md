Video AutoAnswer using Microsoft Lync in UI Suppression Mode
==================================

####*Lync wrapper and WPF control to allow you to easily create UI Suppression mode applications which auto-answer incoming video calls.*
---

##Current Functionality
* UISuppresion Mode WPF application
* Presence status exposed for single user
* Auto-answer video call sent to specific user, two-way video sharing
* Place call to single user, two-way video sharing
* Uses the [Lync 2010 Client SDK](http://www.microsoft.com/en-us/download/details.aspx?id=18898) (which you will need).

##To Do
* There's a couple of issues we know about (listed in the Issues section) which we're keen to fix ASAP.
* Add more features. Let us know what by adding an issue with a enhancement label.

##Things to know

* There is a WPF control - `LyncUISupressionWrapper.Controls.VideoWindow` -  which displays either incoming or outgoing video.
* The LyncUISupressionWrapper.LyncModel object is a singleton, so call it with `LyncModel.Instance;`.
* You must call `.SignIn` as one of the first things you do, otherwise nothing will work!
* When debugging, if you halt your application mid-call/conversation you will leave the underlying Lync process (communicator.exe) in a running state. You will need to kill that process before running again, otherwise you'll get errors that "{Client is in state 'SignedIn', expected 'Uninitialized'}".


###To enable UISuppression Mode
	1. Exit Lync client (and ensure communicator.exe isn't running)
	2. Add registry key HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Communicator\UISuppressionMode as a REG_DWORD with value of 1.
	
###To disable UISuppression Mode
	1. Ensure communicator.exe isn't running.
	2. Set UISuppressionMode key to value of 0.
	3. Restart Lync client.
	
--
from the Modality Product Development Team