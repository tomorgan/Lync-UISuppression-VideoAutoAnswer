Lync-UISuppression-VideoAutoAnswer
==================================

Lync wrapper and WPF control to allow you to easily create UI Suppression mode applications which auto-answer incoming video calls.

To enable UISuppression Mode:
	1. Exit Lync client (and ensure communicator.exe isn't running)
	2. Add registry key HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Communicator\UISuppressionMode as a REG_DWORD with value of 1.
	
To disable UISuppression Mode:
	1. Ensure communicator.exe isn't running.
	2. Set UISuppressionMode key to value of 0.
	3. Restart Lync client.