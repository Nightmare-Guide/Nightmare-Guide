using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingSceneManager : UIUtility
{
    public static string nextScene; // �ε��� ���� ���� �̸��� �����ϴ� static ���� -> static�̱� ������ ���� �ٲ� ���� ����

    public VideoPlayer videoPlayer;
    public RawImage videoImg;

    public string videoFileName = "Loading.mp4"; // StreamingAssets ���� ���� �̸�

    private void Start()
    {
        Cursor.visible = false; // Ŀ�� �Ⱥ��̰� �ϱ�
        videoImg.gameObject.SetActive(false);
        CommonUIManager.instance.interactionUI.SetActive(false);

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

        StartCoroutine(LoadScene());
    }


    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }


    IEnumerator LoadScene()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // AsyncOperation : �ð��� �ɸ��� �۾��� ��׶��忡�� ������ ��, �� ���¸� Ȯ���ϰų� ������ �� �ִ� Ŭ����
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // ���� ���� ��׶��忡�� �ε� ���� (�񵿱�)
        op.allowSceneActivation = false; // �ε��� ������ �ٷ� ��ȯ���� �ʰ� ��ٸ�. (ex: �ε� �ִϸ��̼� �� �����ְ� �Ѿ �� ����)

        while (!op.isDone) // �� �����Ӹ��� op.progress ���� Ȯ���ϸ鼭 �ð� ���� -> progress 0.9 : �� ��ȯ �غ� �Ϸ�, 1 : �� ��ȯ �Ϸ�
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                Debug.Log("Preparing to switch scene");
            }
            else
            {
                Debug.Log("Finish to switch scene");

                yield return new WaitForSeconds(1f);

                SetUIOpacity(videoImg, true, 1f, 0f);

                yield return new WaitForSeconds(2.5f);

                SetUIOpacity(videoImg, false, 1f, 0f);

                yield return new WaitForSeconds(1f);

                op.allowSceneActivation = true;

                CommonUIManager.instance.Blink(true);
            }
        }
    }
}
