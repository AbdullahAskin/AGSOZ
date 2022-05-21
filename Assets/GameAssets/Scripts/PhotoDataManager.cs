using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoDataManager : MonoBehaviour
{
    [SerializeField] private string clientId;
    [SerializeField] private int perPage;
    [SerializeField] private List<RawImage> rawImages;

    public IEnumerator GetPhotoDatas(string word)
    {
        var currentApiUrl = "https://api.unsplash.com/search/photos?query=" + word + "&client_id=" + clientId + "&per_page="+perPage;
        var webRequest = UnityWebRequest.Get(currentApiUrl);
        yield return webRequest.SendWebRequest();

        var pattern = "(?<=thumb.:.)[^\"]*(fit=max)[^\"]+";
        var rg = new Regex(pattern);
        var matchedAuthors = rg.Matches(webRequest.downloadHandler.text);

        for (var index = 0; index < matchedAuthors.Count; index++)
        {
            if (index == rawImages.Count) break;
            var rawUrl = matchedAuthors[index].Value;
            var url = rawUrl.Insert(rawUrl.Length - 5, "&");
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImages[index].texture = myTexture;
            rawImages[index].SetNativeSize();
        }
    }
}