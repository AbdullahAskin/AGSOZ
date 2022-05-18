using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.Text;
using System;

namespace Weelco.SpeechControl {
    
    public class SpeechControl : SpeechControlSettings {

        public delegate void RecordStart();
        public delegate void RecordComplete();
        public delegate void TranslateStart(string languageCode);
        public delegate void TranslateComplete(string result);

        public RecordStart OnRecordStart;
        public RecordComplete OnRecordComplete;
        public TranslateStart OnTranslateStart;
        public TranslateComplete OnTranslateComplete;

        private AudioClip _recordClip;
        private string _deviceName;

        private bool _isRecording;
        private bool _hasPeak;

        // Use this for initialization
        void Start() {
            string[] devices = Microphone.devices;
            if (devices.Length > 0) {
                _deviceName = devices[0];
            }
            else {
                Debug.LogError("[" + this + "] Couldn't found microphone devices");
            }
        }

        // Update is called once per frame
        void Update() {
            if (_isRecording) {
                bool peak = _hasPeak;
                float level = getInputLevel();
                VoiceLevel = (int)(level * 100);
                if (!_hasPeak) {
                    _hasPeak = (level >= VoiceBound);
                }
                if (!peak.Equals(_hasPeak)) {
                    recordProcess();
                }
            }
        }

        void OnDestroy() {
            switch (Status) {
                case StatusEnum.RECORDING:
                    stopMicrophone();
                    break;
                case StatusEnum.TRANSLATING:
                    StopAllCoroutines();
                    break;
            }
        }

        private void updateStatus(StatusEnum status) {
            if (this.Status != status) {
                this.Status = status;
            }
        }

        #region RECORD
        public void StartRecord() {
            startMicrophone();
            if (OnRecordStart != null) {
                OnRecordStart();
            }
        }

        public void StopRecord() {
            stopMicrophone();            
            if (OnRecordComplete != null) {
                OnRecordComplete();
            }
        }

        public void PlayLastRecord() {
            if (_recordClip != null) {
                AudioSource audio = gameObject.GetComponent<AudioSource>();
                if (audio == null) {
                    audio = gameObject.AddComponent<AudioSource>();
                }
                audio.clip = _recordClip;
                audio.Play();
            }
        }

        private void startMicrophone() {
            if (!string.IsNullOrEmpty(_deviceName)) {
                _isRecording = true;
                _recordClip = Microphone.Start(_deviceName, false, MaxRecordTime, 16000);                
                recordStart();
            }
        }

        private void stopMicrophone() {
            VoiceLevel = 0;
            if (_isRecording) {
                Microphone.End(_deviceName);
                _isRecording = false;
                recordStop();
            }
        }

        private void recordStart() {
            updateStatus(StatusEnum.RECORDING);
            recordProcess();
            Invoke("recordComplete", MaxRecordTime);
        }

        private void recordProcess() {
            string invokeName = "recordReset";
            if (IsInvoking(invokeName)) {
                CancelInvoke(invokeName);
            }
            Invoke(invokeName, WaitSpeechTime);
            _hasPeak = false;
        }

        private void recordReset() {
            StopRecord();
        }

        private void recordComplete() {
            StopRecord();
        }

        private void recordStop() {
            updateStatus(StatusEnum.WAITING);
            CancelInvoke();
            recognize();
        }
        
        private float getInputLevel() {
            float inputLevel = 0;
            float[] waveData = new float[SAMPLES];
            int micPosition = Microphone.GetPosition(null) - (SAMPLES + 1);
            if (micPosition < 0) return 0;
            _recordClip.GetData(waveData, micPosition);
            for (int i = 0; i < SAMPLES; i++) {
                float wavePeak = waveData[i] * waveData[i];
                if (inputLevel < wavePeak) {
                    inputLevel = wavePeak;
                }
            }
            return Mathf.Sqrt(inputLevel);
        }
        #endregion

        #region RECOGNITON
        private void recognize() {
            if (trimSilence)
                _recordClip = SavWav.TrimSilence(_recordClip, 0.046f);

            if (_recordClip != null) {
                byte[] buffer = SavWav.Save(_recordClip);
                if (buffer != null) {
                    StartCoroutine(translate(buffer));
                }
            } else {
                if (OnTranslateComplete != null) {
                    OnTranslateComplete("");
                }
            }
        }

        private IEnumerator translate(byte[] buffer) {

            string language = getLanguage();

            JsonData requestJSON = new JsonData();
            JsonData requestConfig = new JsonData();
            JsonData requestAudio = new JsonData();

            requestConfig["encoding"] = "LINEAR16";
            requestConfig["sampleRate"] = "16000";
            requestConfig["languageCode"] = language;
            if (maxAlternatives >= 1)
                requestConfig["maxAlternatives"] = maxAlternatives;
            
            requestAudio["content"] = Convert.ToBase64String(buffer);

            requestJSON["config"] = requestConfig;
            requestJSON["audio"] = requestAudio;

            string url = URL + API_KEY;
            //Debug.Log(requestJSON.ToJson());
            byte[] bytes = Encoding.UTF8.GetBytes(requestJSON.ToJson());            
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            if (OnTranslateStart != null) {
                OnTranslateStart(language);
            }
            updateStatus(StatusEnum.TRANSLATING);
            WWW www = new WWW(url, bytes, headers);
            while (!www.isDone) {
                yield return null;
            }

            if (!string.IsNullOrEmpty(www.error)) {
                Debug.LogError("[" + this + "] Translate error: " + www.error);
            }

            if (OnTranslateComplete != null) {
                OnTranslateComplete(www.text);
            }

            updateStatus(StatusEnum.WAITING);
        }
        #endregion
    }
}