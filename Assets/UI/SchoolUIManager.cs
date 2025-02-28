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
    public GameObject[] uiObjects;
    public GameObject aimUI;

    [Header("# Object")]
    public GameObject cellPhone;
    public bool getCellPhone = false;
    [SerializeField] Camera playerCamera;

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

        // �׽�Ʈ�� -> �κ��丮 �޴��� ��ȣ�ۿ�
        if (Input.GetKeyDown(KeyCode.Keypad7) && getCellPhone)
        {
            OpenCellPhoneItem();
        }
    }
    
    // ���� ���� �Լ�
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // ���콺 Ŀ�� �߾ӿ� ����

        Debug.Log("UI First Setup");
    }

    // UI ���� �Լ�
    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        Debug.Log("Open PhoneUI");

    }

    // UI �ݱ� �Լ�
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

    // ���� �Ͻ� ���� �Լ�
    public void PauseGame()
    {
        // �Ͻ� ���� UI Ȱ��ȭ
        uiObjects[0].SetActive(true);

        // �÷��̾� ������ ����
        StopPlayerController();

        //�÷��̾� ��Ʈ�� OFF
        PlayerController.instance.Close_PlayerController();

        Debug.Log("Pause Game");
    }

    // ������Ʈ ��ȣ�ۿ� �� �÷��̾� ������ ���� �Լ�
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

    // Ŀ�� ���� �Լ�
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

    
    public void SetUIOpacity(Image img, bool up, float time, float waitTime)
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

    // �κ��丮 �޴��� ��ư �Լ�
    public void OpenCellPhoneItem()
    {
        // �޴��� �����ǰ� ����
        Vector3[] cellPhoneTransform = playerCamera.GetComponent<RayCast_Aim>().GetCameraInfo();

        cellPhone.GetComponent<Transform>().position = cellPhoneTransform[0];
        cellPhone.GetComponent<Transform>().rotation = Quaternion.Euler(cellPhoneTransform[1]);

        cellPhone.SetActive(true);

        OpenUI(uiObjects[1]); // null ���� UI ������Ʈ Ȱ��ȭ


        // ��������� �� ���°� �ƴ϶�� �ʱ�ȭ
        if (!cellPhone.GetComponent<CellPhone>().unLocked)
        {
            cellPhone.GetComponent<CellPhone>().SetFirst();
        }
    }
}
