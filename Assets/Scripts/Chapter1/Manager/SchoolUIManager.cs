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
    public List<Item> inventory; // �÷��̾� �κ��丮 ������
    public List<Item> items; // �ΰ��� ������ ������
    public List<ItemSlot> inventorySlots; // ���� UI Slot ��

    [Header("# SaveData")]
    public List<SavePhoneData> phoneDatas;
    public List<String> inventoryDatas;

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��


    private void Awake()
    {
        FirstSetUP();

        phoneInfos = new List<CharacterPhoneInfo>();
        phoneDatas = new List<SavePhoneData>();
        items = new List<Item>();
        inventory = new List<Item>();
    }

    private void Start()
    {
        optionUI = CommonUIManager.instance.optionUI;
        uiObjects.Add(optionUI);

        // �޴��� ������ �Է�
        phoneInfos.Add(new CharacterPhoneInfo { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new CharacterPhoneInfo { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });

        // ������ ������ �Է�
        items.Add(new Item { name = "Locker Key", itemImg = itemImgs[0], uiObj = null, schoolUIManager = this});
        items.Add(new Item { name = "Ethan CellPhone", itemImg = itemImgs[1], uiObj = uiObjects[2], schoolUIManager = this });
        items.Add(new Item { name = "David CellPhone", itemImg = itemImgs[2], uiObj = uiObjects[3], schoolUIManager = this });

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
        CommonUIManager.instance.schoolUIManager = null;
    }

    private void OnApplicationQuit()
    {
        phoneDatas.Add(new SavePhoneData { name = phoneInfos[0].name, hasPhone = phoneInfos[0].hasPhone, isUnlocked = phoneInfos[0].isUnlocked });
        phoneDatas.Add(new SavePhoneData { name = phoneInfos[1].name, hasPhone = phoneInfos[2].hasPhone, isUnlocked = phoneInfos[3].isUnlocked });

        foreach (Item item in inventory)
        {
            inventoryDatas.Add(item.name);
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
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone, GameObject uiObj)
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

        // �κ��丮 ����
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].itemData = inventory[i];
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

    public class Item
    {
        public string name;
        public Sprite itemImg;
        public GameObject uiObj;
        public SchoolUIManager schoolUIManager;
    }
    [System.Serializable]
    public class SavePhoneData
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
    }
}
