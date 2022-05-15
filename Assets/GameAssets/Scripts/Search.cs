using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Search : MonoBehaviour
{
    // string pattern = @"\b[w]\w+";  
    [SerializeField] private TMP_InputField inputField;
    public List<WordFeature> wordFeatures = new List<WordFeature>();
    private readonly string API_URL = "https://api.dictionaryapi.dev/api/v2/entries/en/";
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(GetData());
    }

    public void OnSearchButton()
    {
        StartCoroutine(GetData());
    }

    private IEnumerator GetData()
    {
        // var word = inputField.text;
        var word = "hello";
        var webRequest = UnityWebRequest.Get(API_URL + word);
        yield return webRequest.SendWebRequest();

        foreach (var wordFeature in wordFeatures)
        {
            var pattern = "(?<=" + wordFeature.type + ".:.)[^\"]*";
            var rg = new Regex(pattern);
            wordFeature.data = rg.Match(webRequest.downloadHandler.text).Value;
        }
    }

    private IEnumerator LoadMusic(string url)
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
    public string type;
    public string data;
}