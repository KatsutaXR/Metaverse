using Photon.Voice.Unity;

public class RecorderController
{
    private Recorder _recorder;
    public Recorder Recorder => _recorder;

    public void SetRecorder(Recorder recorder)
    {
        _recorder = recorder;
    }

    public void SetMicActive()
    {
        if (_recorder == null) return;
        _recorder.TransmitEnabled = !_recorder.TransmitEnabled;
    }
}
