using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class VideoDataManager : MonoBehaviour
{
    [SerializeField] private string clientId;
    [SerializeField] private int perPage;
    
    public IEnumerator GetPhotoDatas(string word)
    {
        var currentApiUrl = "https://api.unsplash.com/search/photos?query=" + word + "&client_id=" + clientId + "&per_page="+perPage;
        var webRequest = UnityWebRequest.Get(currentApiUrl);
        yield return webRequest.SendWebRequest();

        var pattern = "(?<=thumb.:.)[^\"]*(fit=max)[^\"]+";
        var rg = new Regex(pattern);
        var videoUrl = rg.Match(webRequest.downloadHandler.text);
        print(webRequest.downloadHandler.text);
    }
}
