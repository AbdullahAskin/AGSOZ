using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoDataManager : MonoBehaviour
{
    [SerializeField] private string clientId;
    [SerializeField] private int perPage;
    [SerializeField] private List<VideoPlayer> videoPlayers;

    public IEnumerator GetVideoDatas(string word)
    {
        var currentApiUrl = "https://api.giphy.com/v1/gifs/search?api_key=" + clientId + "&limit=" + perPage + "&q=" +
                            word;
        var webRequest = UnityWebRequest.Get(currentApiUrl);
        yield return webRequest.SendWebRequest();
        print(currentApiUrl);

        var originalPattern = "\"original\"[^}]*";
        var originalRg = new Regex(originalPattern);
        var matchedAuthors = originalRg.Matches(webRequest.downloadHandler.text);

        for (var index = 0; index < matchedAuthors.Count; index++)
        {
            if (index == videoPlayers.Count) break;
            var videoPattern = "(?<=mp4.:.)[^\"]*";
            var videoRg = new Regex(videoPattern);
            var videoUrl = videoRg.Match(matchedAuthors[index].Value).Value;
            LoadVideo(videoPlayers[index], videoUrl);
        }
    }

    private void LoadVideo(VideoPlayer videoPlayer, string videoUrl)
    {
        videoPlayer.url = videoUrl;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.Prepare();
    }
}