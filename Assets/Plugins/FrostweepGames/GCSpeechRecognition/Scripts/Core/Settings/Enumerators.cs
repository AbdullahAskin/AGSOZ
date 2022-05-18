﻿namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Enumerators
    {
        public enum AudioEncoding
        {
            ENCODING_UNSPECIFIED,
            LINEAR16,
            FLAC,
            MULAW,
            AMR,
            AMR_WB
        }

        public enum SpeechRecognitionType
        {
            SYNC,
            ASYNC,
            STREAMING
        }

        public enum LanguageCode
        {
            RU_RU,
            EN_GB,
            DE,
            FR,
            UK,
            EN_US,
            AF,
            AR_SA,
            EU,
            BE,
            BG,
            HR,
            CS,
            DA,
            HL,
            EN,
            ET,
            FO,
            FI,
            EL,
            HE,
            HU,
            IS,
            ID,
            IT,
            JA,
            KO,
            LV,
            LT,
            NO,
            PL,
            PT,
            RO,
            SK,
            SL,
            ES,
            SV,
            TH,
            TR,
            VI,
            //bad language code, so change to test
            //ZH_CHS,
            //ZH_CHT
            cmn_Hans_CN,
            cmn_Hant_TW,
            yue_Hant_HK

        }
    }
}