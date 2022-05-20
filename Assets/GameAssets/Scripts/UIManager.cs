using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<WordUIElement> wordUIElements;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TMP_InputField inputField;
    private AudioSource _audioSource;
    private List<WordFeature> _currentWordFeature;
    private DictionaryDataManager _dictionaryDataManager;
    private PhotoDataManager _photoDataManager;
    private VideoDataManager _videoDataManager;

    private void Awake()
    {
        _audioSource = FindObjectOfType<AudioSource>();
        _dictionaryDataManager = FindObjectOfType<DictionaryDataManager>();
        _photoDataManager = FindObjectOfType<PhotoDataManager>();
        _videoDataManager = FindObjectOfType<VideoDataManager>();
    }

    public void OnSearchButton()
    {
        var word = inputField.text;
        StartCoroutine(_dictionaryDataManager.GetDictionaryDatas(word));
        StartCoroutine(_photoDataManager.GetPhotoDatas(word));
        StartCoroutine(_videoDataManager.GetVideoDatas(word));
    }

    public void OnPlayClipButton()
    {
        var audioFeature = _currentWordFeature.Find(x => x.type == WordFeatureType.audio);
        StartCoroutine(_dictionaryDataManager.PlayClip(_audioSource, audioFeature.data));
    }

    public void SyncWordFeature(IEnumerable<WordFeature> wordFeatures)
    {
        _currentWordFeature = new List<WordFeature>(wordFeatures);
        var titleFeature = _currentWordFeature.Find(x => x.type == WordFeatureType.word);
        titleText.text = "<style=Title>" + titleFeature.data + "</style>";
        foreach (var wordUIElement in wordUIElements)
        {
            var wordFeature = _currentWordFeature.Find(x => x.type == wordUIElement.type);
            wordUIElement.textElement.text = wordUIElement.textStart + wordFeature.data;
        }
    }
}

[Serializable]
public class WordUIElement
{
    public TextMeshProUGUI textElement;
    public string textStart;
    public WordFeatureType type;
}