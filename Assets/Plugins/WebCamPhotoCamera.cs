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

    IEnumerator TakePhoto()  // Start this Coroutine on some button click
    {

        // NOTE - you almost certainly have to do this here:

        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear,
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToJPG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        System.IO.File.WriteAllBytes(@"C:\Users\Admin\Documents\GitHub\AGSOZ\Assets\StreamingAssets\dnn\" + "004545.jpg", bytes);
        rawImage.texture = photo;
    }
}