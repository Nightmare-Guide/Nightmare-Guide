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
using System.IO;
using static CommonUIManager;

public class SchoolUIManager : UIUtility
{
    [Header("# School Object")]
    public GameObject[] cellPhoneObjs;
    public List<PhoneInfos> phoneInfos; // ���� �޴��� ������ ��� list
    public List<VerticalLayoutGroup> textBoxLayouts;

    [Header("# School Inventory")]
    public List<Sprite> itemImgs; // �κ��丮�� �� �̹�����
    public List<Item> inventory; // �÷��̾� �κ��丮 ������
    public List<Item> items; // �ΰ��� ������ ������
    public List<ItemSlot> inventorySlots; // ���� UI Slot ��

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��


    private void Awake()
    {
        FirstSetUP();

        phoneInfos = new List<PhoneInfos>();
        items = new List<Item>();
        inventory = new List<Item>();
    }

    private void Start()
    {
        if (CommonUIManager.instance != null)
            commonUIManager = CommonUIManager.instance;
        if (TimeLineManager.instance != null)
            timeLineManager = TimeLineManager.instance;

        if (commonUIManager != null)
        {
            optionUI = commonUIManager.optionUI;
            uiObjects.Add(optionUI);
        }

        InitItemDatas(); // ������ ������ �ʱ�ȭ
        GetProgressData(); // ����� ������ ��������


        if (commonUIManager != null)
        {
            commonUIManager.uiManager = this;
        }

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


        // Tab Ű -> �κ��丮
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
                InGameOpenUI(uiObjects[4]);
            }
            else if (uiObjects[4].activeInHierarchy)
            {
                InGameCloseUI(uiObjects[0]);
                InGameCloseUI(uiObjects[4]);
            }
        }
    }

    private void OnDisable()
    {
        commonUIManager.uiManager = null;
    }
    public void CloseUI()
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
    // ���� ���� �Լ�
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // ���콺 Ŀ�� �߾ӿ� ����
    }

    // �κ��丮 �޴��� ��ư �Լ�
    public void OpenCellPhoneItem(PhoneInfos cellPhone, GameObject uiObj)
    {
        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        InGameOpenUI(uiObj);

        CellPhone cpLogic = cellPhone.cellPhoneUI.GetComponent<CellPhone>();


        // ��������� �� ���°� �ƴ϶�� �ʱ�ȭ
        if (!cellPhone.isUnlocked)
        {
            cpLogic.SetFirst();
        }
        else
        {
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
    public void GetItem(GameObject obj)
    {
        // obj �̸��� �����ϴ� items �� �����͸� inventory �� �߰�
        inventory.Add(items.Find(info => obj.gameObject.name.Contains(info.name))); // info -> items List �� ���
        ProgressManager.Instance.progressData.schoolInventoryDatas.Add(items.Find(info => obj.gameObject.name.Contains(info.name)).name);

        // �κ��丮 ����
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].itemData = inventory[i];
        }
    }

    void InitItemDatas()
    {
        // �޴��� ������ �Է�
        phoneInfos.Add(new PhoneInfos { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new PhoneInfos { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });

        // ������ ������ �Է�
        items.Add(new Item { name = "Locker Key", itemImg = itemImgs[0], uiObj = null, schoolUIManager = this });
        items.Add(new Item { name = "Ethan CellPhone", itemImg = itemImgs[1], uiObj = uiObjects[2], schoolUIManager = this });
        items.Add(new Item { name = "David CellPhone", itemImg = itemImgs[2], uiObj = uiObjects[3], schoolUIManager = this });
    }

    void GetProgressData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // �޴���
            phoneInfos[0].hasPhone = ProgressManager.Instance.progressData.phoneDatas[1].hasPhone;
            phoneInfos[0].isUnlocked = ProgressManager.Instance.progressData.phoneDatas[1].isUnlocked;
            phoneInfos[1].hasPhone = ProgressManager.Instance.progressData.phoneDatas[2].hasPhone;
            phoneInfos[1].isUnlocked = ProgressManager.Instance.progressData.phoneDatas[2].isUnlocked;

            if (phoneInfos[0].hasPhone) { cellPhoneObjs[0].SetActive(false); }
            if (phoneInfos[1].hasPhone) { cellPhoneObjs[1].SetActive(false); }

            inventory = new List<Item>();

            // ������
            foreach (string itemName in ProgressManager.Instance.progressData.schoolInventoryDatas)
            {
                inventory.Add(items.Find(info => info.name.Contains(itemName)));
            }

            // �κ��丮 ����
            for (int i = 0; i < inventory.Count; i++)
            {
                inventorySlots[i].itemData = inventory[i];
            }
        }
        else
        {
            //Debug.Log("������ �������� �ʽ��ϴ�.");
        }
    }
}
