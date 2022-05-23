using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using OpenCVForUnityExample;

public class WebCamPhotoCamera : MonoBehaviour
{
    public RawImage rawImage;

    static WebCamTexture backCam;

    private MaskRCNNExample _maskRcnnExample;

    void Start()
    {
        if (backCam == null)
            backCam = new WebCamTexture();

        GetComponent<Renderer>().material.mainTexture = backCam;

        _maskRcnnExample = FindObjectOfType<MaskRCNNExample>();

        if (!backCam.isPlaying)
            backCam.Play();
    }

    public void RecordPhoto()
    {
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()  
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();

        byte[] bytes = photo.EncodeToJPG();
        File.WriteAllBytes(Application.dataPath+@"/StreamingAssets/dnn/004545.jpg", bytes);
        rawImage.texture = photo;
    }
}