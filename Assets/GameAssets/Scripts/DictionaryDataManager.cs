using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class DictionaryDataManager : MonoBehaviour
{
    public List<WordFeature> wordFeatures = new List<WordFeature>();
    private readonly string API_URL = "https://api.dictionaryapi.dev/api/v2/entries/en/";
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }

    public IEnumerator GetDictionaryDatas(string word)
    {
        var webRequest = UnityWebRequest.Get(API_URL + word);
        yield return webRequest.SendWebRequest();

        foreach (var wordFeature in wordFeatures)
        {
            var pattern = "(?<=" + wordFeature.type + ".:.)[^\"]" + wordFeature.limit;
            var rg = new Regex(pattern);    
            var data = rg.Match(webRequest.downloadHandler.text).Value;
            if (data.Length == 0) data = "Non found.";
            wordFeature.data = data;
        }
        _uiManager.SyncWordFeature(wordFeatures);
    }

    public IEnumerator PlayClip(AudioSource audioSource, string url)
    {
        var music = new WWW(url);
        yield return music;
        var pronunciation = music.GetAudioClipCompressed(true, AudioType.MPEG);
        audioSource.clip = pronunciation;
        audioSource.Play();
    }
}

[Serializable]
public class WordFeature
{
    public WordFeatureType type;
    public string data;
    public string limit;
}

public enum WordFeatureType
{
    word,
    audio,
    definition,
    example
}