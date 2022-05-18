using UnityEngine;
using UnityEngine.UI;

namespace Weelco.SpeechControl {
    
    public class Example : MonoBehaviour {

        public Text LogText;
        public Button RecordButton;
        public Button StopButton;
        public Button PlayButton;

        private SpeechControl _speechControl;           
        
        void Start() {

            _speechControl = GetComponent<SpeechControl>();

            if (_speechControl != null) {
                _speechControl.OnRecordStart += onRecordStart;
                _speechControl.OnRecordComplete += onRecordComplete;
                _speechControl.OnTranslateStart += onTranslateStart;
                _speechControl.OnTranslateComplete += onTranslateComplete;
            } else {
                Debug.LogError("Couldn't find SpeechControl Component");
            }
            
            if (RecordButton != null) {
                RecordButton.onClick.AddListener(onRecordButtonClick);
            }
            
            if (StopButton != null) {
                StopButton.onClick.AddListener(onStopButtonClick);
                StopButton.interactable = false;
            }

            if (PlayButton != null) {
                PlayButton.onClick.AddListener(onPlayButtonClick);
                PlayButton.interactable = false;
            }
                                        
            if (LogText != null) {
                LogText.text = string.Empty;
            }           
        }
        
        void OnDestroy() {
            if (_speechControl != null) {
                _speechControl.OnRecordStart -= onRecordStart;
                _speechControl.OnRecordComplete -= onRecordComplete;
                _speechControl.OnTranslateStart -= onTranslateStart;
                _speechControl.OnTranslateComplete -= onTranslateComplete;
            }

            if (RecordButton != null) {
                RecordButton.onClick.RemoveAllListeners();
            }

            if (StopButton != null) {
                StopButton.onClick.RemoveAllListeners();
            }

            if (PlayButton != null) {
                PlayButton.onClick.RemoveAllListeners();
            }
        }

        private void onRecordButtonClick() {            
            _speechControl.StartRecord();
        }

        private void onStopButtonClick() {
            _speechControl.StopRecord();
        }

        private void onPlayButtonClick() {
            _speechControl.PlayLastRecord();
        }
                
        public void onRecordStart() {
            RecordButton.interactable = false;
            StopButton.interactable = true;
            PlayButton.interactable = false;
            LogText.text = "Record start" + "\n";
        }

        public void onRecordComplete() {
            RecordButton.interactable = true;
            StopButton.interactable = false;
            PlayButton.interactable = true;
            LogText.text += "Record complete" + "\n";
        }

        public void onTranslateStart(string languageCode) {
            LogText.text += "Translate start, language: " + languageCode + "\n";
        }

        private void onTranslateComplete(string response) {
            LogText.text += "Translate complete, response: " + response;
            ResponseData result = new ResponseData(response);

            LogText.text += "\nParsed response:\n";
            string transcript;
            float confidence;
            for (int i = 0; i < result.Data.Count; i++) {
                transcript = result.Data[i].transcript;
                confidence = result.Data[i].confidence;
                if (i == 0) {
                    LogText.text += "Best coincidence: " + result.Data[i].transcript;
                    if (!float.IsNaN(confidence)) {
                        LogText.text += " [confidence: " + confidence + "]";
                    }
                    if (result.Data.Count > 1) {
                        LogText.text += "\nOther transcripts: ";
                    }
                } else {
                    string comma = (i < result.Data.Count - 1) ? ", " : string.Empty;
                    LogText.text += transcript + comma;
                }
            }            
        }        
    }
}
