using System;
using System.Collections;
using DG.Tweening;
using SpeechRecognitionSystem;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class AudioRecorder : MonoBehaviour, IMicrophone
{
    private const int LENGTH_SEC = 2;
    private const int FREQ = 16000;
    public int MicrophoneIndex;

    public MicReadyEvent MicReady = new MicReadyEvent();
    private AudioClip _audioClip;

    private bool _firstLoad = true;
    private SpeechRecognizer _speechRecognizer;

    private void Awake()
    {
        _speechRecognizer = FindObjectOfType<SpeechRecognizer>();
        if (Application.platform == RuntimePlatform.Android)
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                Permission.RequestUserPermission(Permission.Microphone);
    }

    private void OnDestroy()
    {
        Microphone.End(Microphone.devices[MicrophoneIndex]);
        _firstLoad = true;
    }

    public int GetRecordPosition()
    {
        return Microphone.GetPosition(Microphone.devices[MicrophoneIndex]);
    }

    public AudioClip GetAudioClip()
    {
        return _audioClip;
    }

    public bool IsRecording()
    {
        return Microphone.IsRecording(Microphone.devices[MicrophoneIndex]);
    }

    public IEnumerator Record(float recordDuration)
    {
        while (_firstLoad)
        {
            var micAutorized = true;
            if (Application.platform == RuntimePlatform.Android)
                micAutorized = Permission.HasUserAuthorizedPermission(Permission.Microphone);
            if (!micAutorized) continue;
            _audioClip = Microphone.Start(Microphone.devices[MicrophoneIndex], true, LENGTH_SEC, FREQ);
            MicReady?.Invoke(this);
            _firstLoad = false;
            yield return new WaitForEndOfFrame();
        }
        DOVirtual.Float(0, recordDuration,  recordDuration, (x) => _speechRecognizer.RecognizeControl()).SetUpdate(UpdateType.Fixed);
        yield return new WaitForSeconds(recordDuration);

        Microphone.End(Microphone.devices[MicrophoneIndex]);
        _firstLoad = true;
    }

    [Serializable]
    public class MicReadyEvent : UnityEvent<IMicrophone>
    {
    }
}