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
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine.Localization.SmartFormat.Utilities;

public class SchoolUIManager : UIUtility
{
    [Header("# School Object")]
    public GameObject[] cellPhoneObjs;
    public List<CharacterPhoneInfo> phoneInfos; // ���� �޴��� ������ ��� list
    public List<VerticalLayoutGroup> textBoxLayouts;

    [Header("# School Inventory")]
    public List<Sprite> itemImgs; // �κ��丮�� �� �̹�����
    public List<String> inventory; // �κ��丮 ������
    public List<Image> inventorySlots; // ���� UI Slot ��
    public Dictionary<string, int> schoolItems; // name, img index -> ������ �̸��̶� key ���� ���ƾ� ��.

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��


    private void Awake()
    {
        FirstSetUP();

        phoneInfos = new List<CharacterPhoneInfo>();
        inventory = new List<String>();

        // Dictionary �ʱ�ȭ
        schoolItems = new Dictionary<string, int>();
    }

    private void OnEnable()
    {
        CommonUIManager.instance.schoolUIManager = this;
    }

    private void Start()
    {
        phoneInfos.Add(new CharacterPhoneInfo { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new CharacterPhoneInfo { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });
        phoneInfos.Add(new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[2], cellPhoneUI = uiObjects[4] });

        // Item Dictionary �� ������ �Է�
        schoolItems.Add("Locker Key", 0);
        schoolItems.Add("Ethan CellPhone", 1);
        schoolItems.Add("David CellPhone", 2);

        optionUI = CommonUIManager.instance.optionUI;
        uiObjects.Add(optionUI);

        CommonUIManager.instance.schoolUIManager = this;
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                // �Ͻ����� UI Ȱ��ȭ
                PauseGame(uiObjects[0]);
            }
            else
            {
                // ��� UI �ݱ�
                foreach (GameObject uiObj in uiObjects)
                {
                    if (uiObj.activeInHierarchy)
                    {
                        InGameCloseUI(uiObj);
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

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
                InGameOpenUI(uiObjects[5]);
            }
        }
    }

    private void OnDisable()
    {
        CommonUIManager.instance.schoolUIManager = null;
    }

    // ���� ���� �Լ�
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // ���콺 Ŀ�� �߾ӿ� ����

        Debug.Log("UI First Setup");
    }

    // �κ��丮 �޴��� ��ư �Լ�
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone, int index)
    {
        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        InGameOpenUI(uiObjects[index]);

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

    // ������ ȹ�� �Լ�
    public void AddItem(GameObject obj)
    {
        inventory.Add(obj.name);

        // �κ��丮 ����
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].gameObject.SetActive(true);
            inventorySlots[i].sprite = itemImgs[schoolItems[inventory[i]]];
        }
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
