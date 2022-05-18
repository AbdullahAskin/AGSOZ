using UnityEngine;

namespace Weelco.SpeechControl {
    
    public class SpeechControlSettings : MonoBehaviour {
        
        // Record consts
        protected const int SAMPLES = 128;
        // Recognition consts
        protected const string URL = "https://speech.googleapis.com/v1beta1/speech:syncrecognize/?key=";

        public string API_KEY = "Paste your Google Cloud Speech API key here, for more info press [?] button";        
        public int VoiceLevel = 0;
        public int WaitSpeechTime = 3;
        public int MaxRecordTime = 10;
        public float VoiceBound = 0.35f;
        public bool UseLanguageCode = false;
        public StatusEnum Status = StatusEnum.WAITING;
        public LanguageEnum LanguageSel = LanguageEnum.English;
        public string LanguageStr = "en-US";        
        public bool trimSilence = false;
        public int maxAlternatives = 10;               

        protected string getLanguage() {
            if (UseLanguageCode) {
                return LanguageStr;                
            } else {
                switch (LanguageSel) {
                    case LanguageEnum.Czech:
                        return "cs-CZ";
                    case LanguageEnum.French:
                        return "fr-FR";
                    case LanguageEnum.German:
                        return "de-DE";
                    case LanguageEnum.Italian:
                        return "it-IT";
                    case LanguageEnum.Russian:
                        return "ru-RU";
                    case LanguageEnum.Spanish:
                        return "es-ES";
                    case LanguageEnum.English:
                    default:
                        return "en-US";
                }
            }
        }
    }

    public enum StatusEnum {
        WAITING,
        RECORDING,
        TRANSLATING
    }

    public enum LanguageEnum {
        English,
        Russian,
        German,
        French,
        Czech,
        Spanish,
        Italian
    }
}
