UFFScreenRecorder Ver 1.0 Developed by Muhammad Usman Munir 

For further details and suggestions:
email: Muhammadusmanmunir@gmail.com
Cell: +923181510911 

NOTE:
To use this DLL, Binaries folder contents (FFMPEG.EXE, FFPROBE.EXE) must be copied to Executable folder of the project (where project's EXE files are located)

For developers:
//Currently, binaries are kept with .DLLs, to use these .DLLs, copy DLLs & FFMPEG binaries to your project's executable folder i.e "bin/debug" 

Usage:

To use this .dll, following steps should be followed.

            UFFScreenRecorder recorder = new UFFScreenRecorder();
            recorder.OnNotification += Recorder_OnNotification; //any notifications received from FFMPEG
            recorder.OnRecordingStarted += Recorder_OnRecordingStarted; //Recording Started 
            recorder.OnRecordingStopped += Recorder_OnRecordingStopped; //Recording Stopped

//To record any window, write down title of that window. 
recorder.Window_Title = "Window Title"; 
//OR
//Leave it blank if you want to record whole screen(s) i.e. 
recorder.Window_Title = String.Empty; //default setting
            
            recorder.Output_Filename = "VideoOutput.mp4"; //output filename
            recorder.Start(); //Start Screen Recording

//to stop video recording, use
            recorder.Stop();

Warning:

You MUST stop video recording before exiting, otherwise video file will be of no use (corrupted).

