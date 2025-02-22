using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;
    public GameObject[] uiObjects;
    public GameObject aimUI;

    // �޴��� ���ȭ��
    [Header ("#Lock")]
    public GameObject LockPhoneUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��

    private void Awake()
    {
        FirstSetUP();
    }

    private void Update()
    {
        // �޴��� UI �ð� �ǽð� ����
        if (LockPhoneUI.activeInHierarchy)
        {
            GetDate();
        }

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

    void GetDate()
    {
        // ���� ��¥�� �ð� ��������
        DateTime currentTime = DateTime.Now;

        // �̱� �������� ��¥ ������ (MM/dd/yyyy)
        string formattedDate = currentTime.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture);

        // �ð� ������ (������ 00��, 13�ÿ��� 24�ô� -12�� ǥ��, 1�ú��� 12�ô� �״�� ǥ��)
        int hour = currentTime.Hour;
        string formattedTime;

        if (hour == 0)
        {
            // ������ 00�÷� ǥ��
            formattedTime = "00:" + currentTime.ToString("mm");
        }
        else if (hour >= 13)
        {
            // 13�ÿ��� 24�ô� -12�� ��ȯ
            formattedTime = (hour - 12).ToString("00") + ":" + currentTime.ToString("mm");
        }
        else
        {
            // 1�ÿ��� 12�ô� �״�� ǥ��
            formattedTime = hour.ToString("00") + ":" + currentTime.ToString("mm");
        }

        // UI �ؽ�Ʈ�� ��¥�� �ð� ǥ��
        dateText.text = formattedDate;
        timeText.text = formattedTime;
    }

    void FirstSetUP()
    {
        blurBG.SetActive(false);
        LockPhoneUI.SetActive(false);

        // ���콺 Ŀ�� �߾ӿ� ����
        CursorLocked();

        Debug.Log("UI First Setup");
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

    public void OpenPhoneUI()
    {
        blurBG.SetActive(true);
        LockPhoneUI.SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        Debug.Log("Open PhoneUI");
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;  // Ŀ���� ���̰� �ϱ�
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ȭ�� �߾��� Ŭ���ϴ� ȿ���� �߻���Ŵ (Windows ����)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}
