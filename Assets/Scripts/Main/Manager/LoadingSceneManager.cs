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
    public static string nextScene; // 로드할 다음 씬의 이름을 저장하는 static 변수 -> static이기 때문에 씬이 바뀌어도 값이 유지

    public VideoPlayer videoPlayer;
    public RawImage videoImg;

    public string videoFileName = "Loading.mp4"; // StreamingAssets 안의 파일 이름

    private void Start()
    {
        Cursor.visible = false; // 커서 안보이게 하기
        videoImg.gameObject.SetActive(false);
        CommonUIManager.instance.interactionUI.SetActive(false);

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

        // AsyncOperation : 시간이 걸리는 작업을 백그라운드에서 진행할 때, 그 상태를 확인하거나 제어할 수 있는 클래스
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // 다음 씬을 백그라운드에서 로딩 시작 (비동기)
        op.allowSceneActivation = false; // 로딩이 끝나도 바로 전환되지 않고 기다림. (ex: 로딩 애니메이션 다 보여주고 넘어갈 때 유용)

        while (!op.isDone) // 매 프레임마다 op.progress 값을 확인하면서 시간 누적 -> progress 0.9 : 씬 전환 준비 완료, 1 : 씬 전환 완료
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
