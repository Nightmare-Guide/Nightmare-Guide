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
    [SerializeField] Camera playerCamera;
    public GameObject[] cellPhoneObjs;
    public List<CharacterPhoneInfo> phoneInfos; // ���� �޴��� ������ ��� list

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��

    public static SchoolUIManager instance { get; private set; }


    private void Awake()
    {
        FirstSetUP();

        phoneInfos = new List<CharacterPhoneInfo>();

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

    private void Start()
    {
        phoneInfos.Add(new CharacterPhoneInfo { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new CharacterPhoneInfo { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });
        phoneInfos.Add(new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[2], cellPhoneUI = uiObjects[4] });
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
        if (Input.GetKeyDown(KeyCode.Keypad1) && phoneInfos[0].hasPhone)
        {
            OpenCellPhoneItem(phoneInfos[0], 2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) && phoneInfos[1].hasPhone)
        {
            OpenCellPhoneItem(phoneInfos[1], 3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) && phoneInfos[2].hasPhone)
        {
            OpenCellPhoneItem(phoneInfos[2], 4);
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
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone, int index)
    {
        OpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        OpenUI(uiObjects[index]);

        CellPhone cpLogic = cellPhone.cellPhoneUI.GetComponent<CellPhone>();


        // ��������� �� ���°� �ƴ϶�� �ʱ�ȭ
        if (!cellPhone.isUnlocked)
        {
            cpLogic.SetFirst();
        }
        else
        {
            Debug.Log($"name : {cellPhone.name} , isUnlocked : {cellPhone.isUnlocked}");

            // Lock Screen ��Ȱ��ȭ, App Screen UI Ȱ��ȭ
            cpLogic.LockPhoneUI.SetActive(false);
            cpLogic.appScreenUI.SetActive(true);

            foreach (Image img in cpLogic.appScreenImgs)
            {
                SetUIOpacity(img, true, 0f, 0f);
            }
            foreach (TextMeshProUGUI text in cpLogic.appScreenTexts)
            {
                SetUIOpacity(text, true, 0f, 0f);
            }
        }
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

    // �޴��� ���� Class
    public class CharacterPhoneInfo
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
        public GameObject cellPhoneObj;
        public GameObject cellPhoneUI;
    }
}
