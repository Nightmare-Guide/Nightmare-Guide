using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NarrationManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImg;

    public string videoFileName = "Loading.mp4"; // StreamingAssets ���� ���� �̸�

    private void Start()
    {
        videoImg.gameObject.SetActive(false);

        // Loading ���� url ã��
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        #if UNITY_EDITOR || UNITY_STANDALONE
                videoPlayer.url = "file://" + filePath;
        #elif UNITY_ANDROID
                videoPlayer.url = filePath; // Android�� file:// ���� ���
        #elif UNITY_IOS
                videoPlayer.url = "file://" + filePath;
        #endif

        videoPlayer.Play();

        videoImg.gameObject.SetActive(true);

        // StartCoroutine(LoadScene());
    }
}
