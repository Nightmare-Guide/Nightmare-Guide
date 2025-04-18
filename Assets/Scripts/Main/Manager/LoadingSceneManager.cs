using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene; // �ε��� ���� ���� �̸��� �����ϴ� static ���� -> static�̱� ������ ���� �ٲ� ���� ����
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

        // AsyncOperation : �ð��� �ɸ��� �۾��� ��׶��忡�� ������ ��, �� ���¸� Ȯ���ϰų� ������ �� �ִ� Ŭ����
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene); // ���� ���� ��׶��忡�� �ε� ���� (�񵿱�)
        op.allowSceneActivation = false; // �ε��� ������ �ٷ� ��ȯ���� �ʰ� ��ٸ�. (ex: �ε� �ִϸ��̼� �� �����ְ� �Ѿ �� ����)
        float timer = 0.0f;

        while (!op.isDone) // �� �����Ӹ��� op.progress ���� Ȯ���ϸ鼭 �ð� ���� -> progress 0.9 : �� ��ȯ �غ� �Ϸ�, 1 : �� ��ȯ �Ϸ�
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
