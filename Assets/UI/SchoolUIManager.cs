using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;
using System.Drawing;

public class SchoolUIManager : MonoBehaviour
{
    [Header("# UI")]
    public GameObject blurBG;
    public GameObject[] uiObjects;
    public GameObject aimUI;

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��

    public static SchoolUIManager instance { get; private set; }


    private void Awake()
    {
        FirstSetUP();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled())
            {
                // �Ͻ����� UI Ȱ��ȭ
                PauseGame();
            }
            else
            {
                // ��� UI �ݱ�
                foreach (GameObject uiObj in uiObjects)
                {
                    if (uiObj.activeInHierarchy)
                    {
                        CloseUI(uiObj);
                    }
                }
            }
        }
    }

    void FirstSetUP()
    {
        blurBG.SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // ���콺 Ŀ�� �߾ӿ� ����

        Debug.Log("UI First Setup");
    }

    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        Debug.Log("Open PhoneUI");

    }
    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);

        //ī�޶� ȸ�� Ȱ��ȭ
        Camera_Rt.instance.Open_Camera();

        //�÷��̾� ��Ʈ�� On
        PlayerController.instance.Open_PlayerController();

        // ���콺 Ŀ�� �߾ӿ� ����
        CursorLocked();

        Debug.Log("Close UI : " + ui.name);
    }

    public void PauseGame()
    {
        // �Ͻ� ���� UI Ȱ��ȭ
        blurBG.SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        Debug.Log("Pause Game");
    }

    void StopPlayerController()
    {
        // ���� UI ��Ȱ��ȭ
        aimUI.SetActive(false);

        //ī�޶� ȸ�� ����
        Camera_Rt.instance.Close_Camera();

        //���콺 Ŀ�� Ȱ��ȭ
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // Ŀ���� ���̰� �ϱ�
    }

    // UI ������Ʈ ��� ��Ȱ��ȭ ������ �� Ȯ��
    bool AreAllObjectsDisabled()
    {
        Debug.Log("All UI Objects Disabled");

        return uiObjects.All(obj => !obj.activeSelf);
    }

    void CursorLocked()
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

    public void SetUIOpacity(Image img, bool up, float time)
    {
        StartCoroutine(SetOpacity(img, up, time));
    }

    // Image ���� ���� �ڷ�ƾ
    private IEnumerator SetOpacity(Image img, bool up, float time)
    {
        yield return new WaitForSeconds(0.2f);

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

        if(!up) { img.gameObject.SetActive(false);}
    }
}
