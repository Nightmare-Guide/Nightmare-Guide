using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices;
using UnityStandardAssets.Characters.FirstPerson;
using Unity.VisualScripting;

public class UIUtility : MonoBehaviour
{
    [Header("# UI")]
    public List<GameObject> uiObjects;
    public GameObject aimUI;
    public GameObject optionUI;

    [Header("# Object")]
    [SerializeField] Camera playerCamera;

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

        //ī�޶� ȸ�� Ȱ��ȭ
        Camera_Rt.instance.Open_Camera();

        //�÷��̾� ��Ʈ�� On
        PlayerController.instance.Open_PlayerController();

        // ���콺 Ŀ�� �߾ӿ� ����
        CursorLocked();
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
        // �Ͻ� ���� UI Ȱ��ȭ
        blur.SetActive(true);
        optionUI.SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        // �ð� ����
        Time.timeScale = 0;
    }

    // ������Ʈ ��ȣ�ۿ� �� �÷��̾� ������ ���� �Լ�
    protected void StopPlayerController()
    {
        //�÷��̾� ��Ʈ�� OFF
        PlayerController.instance.Close_PlayerController();

        //ī�޶� ȸ�� ����
        Camera_Rt.instance.Close_Camera();

        CursorUnLocked();
    }

    // UI ������Ʈ ��� ��Ȱ��ȭ ������ �� Ȯ��
    protected bool AreAllObjectsDisabled(List<GameObject> uiObjs)
    {
        return uiObjs.All(obj => !obj.activeSelf);
    }

    // Ŀ�� ���� �Լ�
    protected void CursorLocked()
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

    protected void CursorUnLocked()
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
}
