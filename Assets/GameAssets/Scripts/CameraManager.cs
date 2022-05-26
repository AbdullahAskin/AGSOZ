using System.Collections;
using System.IO;
using DG.Tweening;
using OpenCVForUnityExample;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    private static WebCamTexture backCam;
    public RawImage cameraImage;
    [SerializeField] private float cameraFrozeTime;
    [SerializeField] private float photoOffset;
    [SerializeField] private Button shootPhotoButton;
    private Coroutine _currentCoroutine;

    private MaskRCNNExample _maskRcnnExample;

    private void Start()
    {
        if (backCam == null)
            backCam = new WebCamTexture();

        cameraImage.texture = backCam;

        _maskRcnnExample = FindObjectOfType<MaskRCNNExample>();

        if (!backCam.isPlaying)
            backCam.Play();
    }

    public void RecordPhoto()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(TakePhoto());

        shootPhotoButton.interactable = false;
        DOVirtual.DelayedCall(photoOffset, () => shootPhotoButton.interactable = true);
    }

    private IEnumerator TakePhoto()
    {
        yield return new WaitForEndOfFrame();

        var photo = new Texture2D(backCam.width, backCam.height);
        photo.SetPixels(backCam.GetPixels());
        photo.Apply();

        var bytes = photo.EncodeToJPG();
        File.WriteAllBytes(Application.dataPath + @"/StreamingAssets/dnn/004545.jpg", bytes);
        cameraImage.texture = photo;

        yield return new WaitForSeconds(cameraFrozeTime);
        cameraImage.texture = backCam;
    }
}