using System;
using SpeechRecognitionSystem;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

internal class SpeechRecognizer : MonoBehaviour
{
    public string LanguageModelDirPath = "SpeechRecognitionSystem/model/english_small";
    private bool _copyRequested;
    private bool _init;
    private int _lastSample;
    private IMicrophone _microphone;

    private SpeechRecognitionSystem.SpeechRecognizer _sr;
    private UIManager _uiManager;

    private void Awake()
    {
        _sr = new SpeechRecognitionSystem.SpeechRecognizer();
        _uiManager = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
            OnInitResult(Application.streamingAssetsPath + "/" + LanguageModelDirPath);
    }

    private void OnDestroy()
    {
        _init = false;
        _copyRequested = false;
        _sr.Dispose();
        _sr = null;
    }

    public void RecognizeControl()
    {
        if (Application.platform == RuntimePlatform.Android)
            if (!_copyRequested && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                CopyAssets2ExternalStorage(LanguageModelDirPath);
                _copyRequested = true;
            }

        if (_init && _microphone != null && _microphone.IsRecording()) Recognize();
    }

    public void OnMicrophoneReady(IMicrophone microphone)
    {
        if (Application.platform == RuntimePlatform.Android)
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        _microphone = microphone;
    }

    private void OnInitResult(string modelDirPath)
    {
        if (modelDirPath != string.Empty)
        {
            _init = _sr.Init(modelDirPath);
        }
    }

    private void CopyAssets2ExternalStorage(string modelDirPath)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var recognizerActivity = new AndroidJavaObject("com.sss.unity_asset_manager.MainActivity", currentActivity);
            recognizerActivity.CallStatic("setReceiverObjectName", gameObject.name);
            recognizerActivity.CallStatic("setLogReceiverMethodName", "onReceiveLogMess");
            recognizerActivity.CallStatic("setOnCopyingCompleteMethod", "onInitResult");
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                recognizerActivity.Call("tryCopyStreamingAssets2ExternalStorage", modelDirPath);
            }
        }
    }

    private void Recognize()
    {
        var pos = _microphone.GetRecordPosition();
        var diff = pos - _lastSample;
        if (diff > 0)
        {
            var samples = new float[diff * _microphone.GetAudioClip().channels];
            var ac = _microphone.GetAudioClip();
            if (ac != null)
            {
                _microphone.GetAudioClip().GetData(samples, _lastSample);
                if (_sr.AppendAudioData(samples) == 0)
                {
                    var result = _sr.GetPartialResult();
                    if (result.partial != string.Empty)
                        _uiManager.SetInputField(result.partial);
                }
                else
                {
                    var result = _sr.GetResult();
                    if (result.text != string.Empty)
                        _uiManager.SetInputField(result.text);
                }
            }
        }

        _lastSample = pos;
    }

    [Serializable]
    public class MessageEvent : UnityEvent<string>
    {
    }
}