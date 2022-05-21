using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using SpeechRecognitionSystem;

internal class SpeechRecognizer : MonoBehaviour {
    public string LanguageModelDirPath = "SpeechRecognitionSystem/model/english_small";

    public void OnMicrophoneReady( IMicrophone microphone ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                Permission.RequestUserPermission( Permission.ExternalStorageWrite );
            }
        }
        _microphone = microphone;
    }

    [System.Serializable]
    public class MessageEvent : UnityEvent<string> { }

    public MessageEvent LogMessageReceived = new MessageEvent( );
    public MessageEvent PartialResultReceived = new MessageEvent( );
    public MessageEvent ResultReceived = new MessageEvent( );

    private void onInitResult( string modelDirPath ) {
        if ( modelDirPath != string.Empty ) {
             _init = _sr.Init( modelDirPath );
            LogMessageReceived?.Invoke( "Say something..." );
        }
        else {
            LogMessageReceived?.Invoke( "Error on copying streaming assets" );
        }
    }

    private void onReceiveLogMess( string message ) {
        LogMessageReceived?.Invoke( message );
    }

    private void Awake( ) {
        _sr = new SpeechRecognitionSystem.SpeechRecognizer( );
    }

    private void Start( ) {
        if ( Application.platform != RuntimePlatform.Android ) {
            onInitResult( Application.streamingAssetsPath + "/" + LanguageModelDirPath );
        }
    }

    private void FixedUpdate( ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            if ( !_copyRequested && Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                copyAssets2ExternalStorage( LanguageModelDirPath );
                _copyRequested = true;
            }
        }
        if ( _init && ( _microphone != null ) && _microphone.IsRecording( ) ) {
            recognize( );
        }
    }

    private void OnDestroy( ) {
        _init = false;
        _copyRequested = false;
        _sr.Dispose( );
        _sr = null;
    }
    private void copyAssets2ExternalStorage( string modelDirPath ) {
        if ( Application.platform == RuntimePlatform.Android ) {
            var javaUnityPlayer = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
            var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>( "currentActivity" );
            var recognizerActivity = new AndroidJavaObject( "com.sss.unity_asset_manager.MainActivity", currentActivity );
            recognizerActivity.CallStatic( "setReceiverObjectName", this.gameObject.name );
            recognizerActivity.CallStatic( "setLogReceiverMethodName", "onReceiveLogMess" );
            recognizerActivity.CallStatic( "setOnCopyingCompleteMethod", "onInitResult" );
            if ( Permission.HasUserAuthorizedPermission( Permission.ExternalStorageWrite ) ) {
                LogMessageReceived?.Invoke( "Please wait until the files of language model are copied..." );
                recognizerActivity.Call( "tryCopyStreamingAssets2ExternalStorage", modelDirPath );
            }
        }
    }
    private void recognize( ) {
        int pos = _microphone.GetRecordPosition( );
        int diff = pos - _lastSample;
        if ( diff > 0 ) {
            var samples = new float[ diff * _microphone.GetAudioClip( ).channels ];
            var ac = _microphone.GetAudioClip( );
            if ( ac != null ) {
                _microphone.GetAudioClip( ).GetData( samples, _lastSample );
                if ( _sr.AppendAudioData( samples ) == 0 ) {
                    var result = _sr.GetPartialResult( );
                    if ( result.partial != string.Empty )
                        PartialResultReceived?.Invoke( result.partial );
                } else {
                    var result = _sr.GetResult( );
                    if ( result.text != string.Empty )
                        ResultReceived?.Invoke( result.text );
                }
            }
        }
        _lastSample = pos;
    }

    private SpeechRecognitionSystem.SpeechRecognizer _sr = null;
    private IMicrophone _microphone = null;
    private bool _init = false;
    private int _lastSample = 0;
    private bool _copyRequested = false;
}
