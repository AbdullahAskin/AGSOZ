using LitJson;
using System.Collections.Generic;

namespace Weelco.SpeechControl {
    
    public class ResponseData {

        /*
       Response example:  
       {
         "results": [
           {
             "alternatives": [
               {
                 "transcript": string,
                 "confidence": number
               }
             ]
           },
           {
             "alternatives": [
               {
                 "transcript": string,
                 "confidence": number
               }
             ]
           },
           ...
         ]
       }
       */

        private List<TranscriptData> _transcripts;
        
        public ResponseData(string response) {
            _transcripts = new List<TranscriptData>();
            if (!response.Equals(string.Empty)) {
                JsonData jsonData = JsonMapper.ToObject(response);
                string data = string.Empty;
                if (jsonData.Keys.Contains("results")) {
                    JsonData results = jsonData["results"];
                    JsonData alternatives;
                    JsonData alternative;
                    string transcript;
                    float confidence;                    
                    for (int i = 0; i < results.Count; i++) {
                        alternatives = results[i]["alternatives"];
                        for (int j = 0; j < alternatives.Count; j++) {
                            alternative = alternatives[j];
                            transcript = alternative["transcript"].ToString();
                            if (alternative.Keys.Contains("confidence")) {
                                confidence = float.Parse(alternative["confidence"].ToString());
                            }
                            else {
                                confidence = float.NaN;
                            }
                            _transcripts.Add(new TranscriptData(transcript, confidence));
                        }
                    }
                }
            }
        }

        public List<TranscriptData> Data {
            get { return _transcripts; }
        }
    }

    public class TranscriptData {

        private string _transcript;
        private float _confidence;

        public TranscriptData(string transcripts, float confidence) {
            _transcript = transcripts;
            _confidence = confidence;
        }

        public string transcript {
            get { return _transcript; }
        }

        public float confidence {
            get { return _confidence; }
        }
    }
}