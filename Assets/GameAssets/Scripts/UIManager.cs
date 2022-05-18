using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<WordUIElement> wordUIElements;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TMP_InputField inputField;
    private AudioSource _audioSource;
    private List<WordFeature> _currentWordFeature;
    private DataManager _dataManager;

    private void Awake()
    {
        _audioSource = FindObjectOfType<AudioSource>();
        _dataManager = FindObjectOfType<DataManager>();
    }

    public void OnSearchButton()
    {
        var word = inputField.text;
        StartCoroutine(_dataManager.GetData(word));
    }

    public void OnPlayClipButton()
    {
        var audioFeature = _currentWordFeature.Find(x => x.type == WordFeatureType.audio);
        StartCoroutine(_dataManager.PlayClip(_audioSource, audioFeature.data));
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