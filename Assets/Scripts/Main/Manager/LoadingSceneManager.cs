using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene; // 로드할 다음 씬의 이름을 저장하는 static 변수 -> static이기 때문에 씬이 바뀌어도 값이 유지
                                    // [SerializeField] Image progressBar;

    public VideoPlayer videoPlayer;

    private void Start()
    {

        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        // AsyncOperation : 시간이 걸리는 작업을 백그라운드에서 진행할 때, 그 상태를 확인하거나 제어할 수 있는 클래스
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // 다음 씬을 백그라운드에서 로딩 시작 (비동기)
        op.allowSceneActivation = false; // 로딩이 끝나도 바로 전환되지 않고 기다림. (ex: 로딩 애니메이션 다 보여주고 넘어갈 때 유용)
        float timer = 0.0f;

        while (!op.isDone) // 매 프레임마다 op.progress 값을 확인하면서 시간 누적 -> progress 0.9 : 씬 전환 준비 완료, 1 : 씬 전환 완료
        {
            yield return null;
            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                Debug.Log("Preparing to switch scene");
            }
            else
            {
                op.allowSceneActivation = true;
            }
        }
    }
}
