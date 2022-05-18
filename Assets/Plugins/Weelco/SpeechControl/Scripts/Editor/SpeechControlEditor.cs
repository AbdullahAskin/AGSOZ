using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Weelco.SpeechControl {

    [ExecuteInEditMode]
    [CustomEditor(typeof(SpeechControl))]
    public class SpeechControlEditor : Editor {

        private const string GOOGLE_AUTH_LINK = "https://cloud.google.com/speech/docs/common/auth";
        private const string GOOGLE_LANG_LINK = "https://cloud.google.com/speech/docs/languages";

        private const int MIN_RECORD_TIME = 3;
        private const int MAX_RECORD_TIME = 60;

        private const int MIN_WAIT_SPEECH_TIME = 1;
        private const int MAX_WAIT_SPEECH_TIME = 10;

        private const float MIN_VOICE_BOUND = 0.05f;
        private const float MAX_VOICE_BOUND = 1.0f;

        SerializedProperty voiceLevelProperty;

        void OnEnable() {
            voiceLevelProperty = serializedObject.FindProperty("VoiceLevel");
        }

        public override void OnInspectorGUI() {
            SpeechControlSettings myTarget = (SpeechControlSettings)target;

            if (target == null)
                return;
            
            GUI.changed = false;
            EditorGUIUtility.labelWidth = 200;

            GUILayout.Space(15);
            EditorGUILayout.LabelField("Recognition Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();            
            EditorGUILayout.LabelField("Google Cloud Speech API key", GUILayout.Width(195));            
            if (GUILayout.Button("[?]", GUI.skin.button, GUILayout.Width(28))) {
                Application.OpenURL(GOOGLE_AUTH_LINK);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            myTarget.API_KEY = EditorGUILayout.TextField(myTarget.API_KEY);
            GUILayout.Space(5);
            myTarget.maxAlternatives = EditorGUILayout.IntField("Max Alternatives", myTarget.maxAlternatives);
            myTarget.trimSilence = EditorGUILayout.Toggle(new GUIContent("Trim Silence", "Reduces recognition time, but decrease accuracy"), myTarget.trimSilence);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Language", EditorStyles.boldLabel);
            myTarget.UseLanguageCode = EditorGUILayout.Toggle("[Use language code?]", myTarget.UseLanguageCode);
            GUILayout.Space(5);
            if (myTarget.UseLanguageCode) {                
                GUILayout.BeginHorizontal();
                myTarget.LanguageStr = EditorGUILayout.TextField(myTarget.LanguageStr, GUILayout.Width(195), GUILayout.Height(18));
                if (GUILayout.Button("[?]", GUI.skin.button, GUILayout.Width(28))) {
                    Application.OpenURL(GOOGLE_LANG_LINK);
                }
                GUILayout.EndHorizontal();
            } else {
                myTarget.LanguageSel = (LanguageEnum)EditorGUILayout.EnumPopup(myTarget.LanguageSel, GUILayout.Height(18));                
            }            

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Speech Settings", EditorStyles.boldLabel);
            GUI.enabled = (myTarget.Status != StatusEnum.RECORDING);
            myTarget.MaxRecordTime = EditorGUILayout.IntSlider("Max record time [seconds]", myTarget.MaxRecordTime, MIN_RECORD_TIME, MAX_RECORD_TIME);
            myTarget.WaitSpeechTime = EditorGUILayout.IntSlider("Max silence time [seconds]", myTarget.WaitSpeechTime, MIN_WAIT_SPEECH_TIME, MAX_WAIT_SPEECH_TIME);                        
            GUI.enabled = true;
            myTarget.VoiceBound = EditorGUILayout.Slider("Voice detection level", myTarget.VoiceBound, MIN_VOICE_BOUND, MAX_VOICE_BOUND);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Dashboard", EditorStyles.boldLabel);            
            EditorGUILayout.LabelField("Status:", myTarget.Status.ToString());
            GUILayout.Space(5);
            if (!voiceLevelProperty.hasMultipleDifferentValues)
                ProgressBar(voiceLevelProperty.intValue / 100.0f, "Voice level");
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(10);

            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        void ProgressBar(float value, string label) {
            GUILayout.BeginHorizontal();
            Rect rect = GUILayoutUtility.GetRect(500, 18, "TextField");
            string text = label + ": " + value;
            EditorGUI.ProgressBar(rect, value, text);
            GUILayout.EndHorizontal();
        }
    }
}