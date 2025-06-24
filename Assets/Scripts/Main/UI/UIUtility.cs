using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices;
using UnityStandardAssets.Characters.FirstPerson;
using Unity.VisualScripting;
using UnityEngine.Playables;

public class UIUtility : MonoBehaviour
{
    [Header("# UI")]
    public List<GameObject> uiObjects;
    public GameObject aimUI;
    public GameObject optionUI;
    public Image fadeInOutImg;

    [Header("# Object")]
    [SerializeField] Camera playerCamera;

    [Header("# TimeLine")]
    public PlayableDirector playableDirector;

    [Header("# Singleton")]
    public CommonUIManager commonUIManager;
    public TimeLineManager timeLineManager;

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��

    // �ΰ��� UI ���� �Լ� -> Player O
    public void InGameOpenUI(GameObject ui)
    {
        OpenUI(ui);

        // �÷��̾� ������ ����
        StopPlayerController();
    }

    // �ΰ��� UI �ݱ� �Լ� -> Player O
    public void InGameCloseUI(GameObject ui)
    {
        CloseUI(ui);

        Time.timeScale = 1;

        if (!CommonUIManager.instance.isTalkingWithNPC)
        {
            StartPlayerController();
        }
    }

    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);
    }

    // ���� �Ͻ� ���� �Լ�
    protected void PauseGame(GameObject blur)
    {
        // TimeLine �� ���� ���̸� ����
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            playableDirector.Pause();
        }

        // �÷��̾� ������ ����
        StopPlayerController();

        // �ð� ����
        Time.timeScale = 0;

        // �Ͻ� ���� UI Ȱ��ȭ
        blur.SetActive(true);
        optionUI.SetActive(true);
    }

    // ������Ʈ ��ȣ�ۿ� �� �÷��̾� ������ ���� �Լ�
    public void StopPlayerController()
    {
        //�÷��̾� ��Ʈ�� OFF
        if(PlayerController.instance != null)
        {
            PlayerController.instance.Close_PlayerController();
        }

        if(Camera_Rt.instance != null)
        {
            //ī�޶� ȸ�� ����
            Camera_Rt.instance.Close_Camera();
        }

        CursorUnLocked();
    }

    public void StartPlayerController()
    {
        if (PlayerController.instance != null)
        {
            //�÷��̾� ��Ʈ�� On
            PlayerController.instance.Open_PlayerController();
        }

        if (Camera_Rt.instance != null)
        {
            //ī�޶� ȸ�� Ȱ��ȭ
            Camera_Rt.instance.Open_Camera();
        }

        // ���콺 Ŀ�� �߾ӿ� ����
        CursorLocked();
    }

    // UI ������Ʈ ��� ��Ȱ��ȭ ������ �� Ȯ��
    protected bool AreAllObjectsDisabled(List<GameObject> uiObjs)
    {
        return uiObjs.All(obj => !obj.activeSelf);
    }

    // Ŀ�� ���� �Լ�
    public void CursorLocked()
    {
        // ���� UI Ȱ��ȭ
        aimUI.SetActive(true);

        // ���� ���� �� �⺻������ Locked ���� ����
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // ȭ�� �߾��� Ŭ���ϴ� ȿ���� �߻���Ŵ (Windows ����)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }

    public void CursorUnLocked()
    {
        // ���� UI ��Ȱ��ȭ
        aimUI.SetActive(false);

        //���콺 Ŀ�� Ȱ��ȭ
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // Ŀ���� ���̰� �ϱ�
    }


    public void SetUIOpacity(Image img, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(img, up, time, waitTime));
    }

    public void SetUIOpacity(RawImage img, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(img, up, time, waitTime));
    }

    public void SetUIOpacity(TextMeshProUGUI text, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(text, up, time, waitTime));
    }


    // Image ���� ���� �ڷ�ƾ
    private IEnumerator SetOpacity(Image img, bool up, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (up) { img.gameObject.SetActive(true); }

        float elapsed = 0f;

        float start = up ? 0f : 1f;
        float end = up ? 1f : 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            UnityEngine.Color color = img.color;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            img.color = color;
            yield return null;
        }

        // ���� �� ����
        UnityEngine.Color finalColor = img.color;
        finalColor.a = end;
        img.color = finalColor;

        if (!up) { img.gameObject.SetActive(false); }
    }

    private IEnumerator SetOpacity(RawImage img, bool up, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (up) { img.gameObject.SetActive(true); }

        float elapsed = 0f;

        float start = up ? 0f : 1f;
        float end = up ? 1f : 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            UnityEngine.Color color = img.color;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            img.color = color;
            yield return null;
        }

        // ���� �� ����
        UnityEngine.Color finalColor = img.color;
        finalColor.a = end;
        img.color = finalColor;

        if (!up) { img.gameObject.SetActive(false); }
    }

    // TextMeshProUGUI ���� ���� �ڷ�ƾ
    private IEnumerator SetOpacity(TextMeshProUGUI text, bool up, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (up) { text.gameObject.SetActive(true); }

        float elapsed = 0f;

        float start = up ? 0f : 1f;
        float end = up ? 1f : 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            UnityEngine.Color color = text.color;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            text.color = color;
            yield return null;
        }

        // ���� �� ����
        UnityEngine.Color finalColor = text.color;
        finalColor.a = end;
        text.color = finalColor;

        if (!up) { text.gameObject.SetActive(false); }
    }

    // �ε� �Ҽ��� ���� ���� �Լ�
    public bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }

    public bool ApproximatelyEqual(float a, float b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

    // VerticalLayoutGroup �ʱ�ȭ
    public void RebuildVerticalLayout(List<VerticalLayoutGroup> verticalLayoutGroup)
    {
        foreach(VerticalLayoutGroup group in verticalLayoutGroup)
        {
            LayoutRebuilder.MarkLayoutForRebuild(group.GetComponent<RectTransform>());
        }
    }


    // TimeLine
    public void FadeIn()
    {
        fadeInOutImg.gameObject.SetActive(true);
        SetUIOpacity(fadeInOutImg, false, 0.4f, 0f);
    }

    public void FadeOut()
    {
        SetUIOpacity(fadeInOutImg, true, 0.4f, 0f);
    }

    public void FinishedTimeLine()
    {
        playableDirector.playableAsset = null;

        // ���� UI Ȱ��ȭ
        aimUI.SetActive(true);
    }

    public void EnableCollider(Collider col)
    {
        col.enabled = true;
    }

    public void DisableCollider(Collider col)
    {
        col.enabled = false;
    }

    public IEnumerator EnableCollider(Collider col, float time)
    {
        yield return time;
        col.enabled = true;
    }


    public void StartTimeLine(PlayableAsset asset)
    {
        if (timeLineManager.playableAssets.Count > 0 && playableDirector != null)
        {
            // �̹� ����� �� ������ return
            //if (ProgressManager.Instance.progressData.timelineWatchedList.Find(e => e.key == asset.name).value)
            //    return;

            // Ÿ�Ӷ��� ����
            playableDirector.playableAsset = asset;
            playableDirector.Play();

            // ������ key ������ ã�Ƽ� ����
            // ProgressManager.Instance.progressData.timelineWatchedList.Find(e => e.key == asset.name).value = true;

            // ���� UI ��Ȱ��ȭ
            aimUI.SetActive(false);

            //���콺 Ŀ�� ��Ȱ��ȭ
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;  // Ŀ���� ���̰� �ϱ�
        }
    }


}
