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

    public string videoFileName = "Loading.mp4"; // StreamingAssets ���� ���� �̸�

    private void Start()
    {
        CommonUIManager.instance.Blink(true);

        Cursor.visible = false; // Ŀ�� �Ⱥ��̰� �ϱ�
        videoImg.gameObject.SetActive(false);
        text.gameObject.SetActive(false);

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
        text.gameObject.SetActive(true);

        StartCoroutine(PrintDialogue());
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CommonUIManager.instance != null && !CommonUIManager.instance.optionUI.activeInHierarchy)
            {
                // �Ͻ� ���� UI Ȱ��ȭ
                CommonUIManager.instance.optionUI.SetActive(true);
                blur.SetActive(true);
                Cursor.visible = true; // Ŀ�� ���̰� �ϱ�
                Time.timeScale = 0;

            }
            else if (CommonUIManager.instance != null && CommonUIManager.instance.optionUI.activeInHierarchy)
            {
                // �Ͻ� ���� UI �� Ȱ��ȭ
                CommonUIManager.instance.optionUI.SetActive(false);
                blur.SetActive(false);
                Cursor.visible = false; // Ŀ�� �� ���̰� �ϱ�
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
