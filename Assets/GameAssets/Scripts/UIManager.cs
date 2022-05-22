using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<WordUIElement> wordUIElements;
    [SerializeField] private TextMeshProUGUI titleText;
    public TMP_InputField inputField;
    public float recordDuration;
    private AudioRecorder _audioRecorder;
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
        _audioRecorder = FindObjectOfType<AudioRecorder>();
    }

    public void OnRecordButton()
    {
        StartCoroutine(_audioRecorder.Record(recordDuration));
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

    public void SetInputField(string input)
    {
        inputField.text = input;
    }
}

[Serializable]
public class WordUIElement
{
    public TextMeshProUGUI textElement;
    public string textStart;
    public WordFeatureType type;
}