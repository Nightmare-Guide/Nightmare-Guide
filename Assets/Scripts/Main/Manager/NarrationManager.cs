using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using static UnityEditor.Progress;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SocialPlatforms;

public class NarrationManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoImg;
    public TextMeshProUGUI text;
    public GameObject blur;

    public string videoFileName = "Loading.mp4"; // StreamingAssets 안의 파일 이름

    private void Start()
    {
        CommonUIManager.instance.Blink(true);

        Cursor.visible = false; // 커서 안보이게 하기
        videoImg.gameObject.SetActive(false);
        text.gameObject.SetActive(false);

        // Loading 영상 url 찾기
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        #if UNITY_EDITOR || UNITY_STANDALONE
                videoPlayer.url = "file://" + filePath;
        #elif UNITY_ANDROID
                videoPlayer.url = filePath; // Android는 file:// 없이 사용
        #elif UNITY_IOS
                videoPlayer.url = "file://" + filePath;
        #endif

        videoPlayer.Play();

        videoImg.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        StartCoroutine(PrintDialogue());
    }

    private void Update()
    {
        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CommonUIManager.instance != null && !CommonUIManager.instance.optionUI.activeInHierarchy)
            {
                // 일시 정지 UI 활성화
                CommonUIManager.instance.optionUI.SetActive(true);
                blur.SetActive(true);
                Cursor.visible = true; // 커서 보이게 하기
                Time.timeScale = 0;

            }
            else if (CommonUIManager.instance != null && CommonUIManager.instance.optionUI.activeInHierarchy)
            {
                // 일시 정지 UI 비 활성화
                CommonUIManager.instance.optionUI.SetActive(false);
                blur.SetActive(false);
                Cursor.visible = false; // 커서 안 보이게 하기
                Time.timeScale = 1;
            }
        }
    }

    IEnumerator PrintDialogue()
    {
        yield return new WaitForSeconds(CommonUIManager.instance.blinkDuration + 1.5f);

        if (CSVRoad_Story.instance != null)
        {
            CSVRoad_Story.instance.OnSelectChapter("0_0_0");
        }
        else
        {
            Debug.Log("Can not find CSVRoad_Story");
        }
    }
}
