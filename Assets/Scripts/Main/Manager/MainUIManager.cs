using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static SchoolUIManager;

public class MainUIManager : UIUtility
{
    public List<VerticalLayoutGroup> textBoxLayouts;

    public GameObject cellPhoneObjs;

    [Header("# Main Inventory")]
    public List<Sprite> itemImgs; // �κ��丮�� �� �̹�����
    public List<Item> inventory; // �÷��̾� �κ��丮 ������
    public List<Item> items; // �ΰ��� ������ ������
    public List<ItemSlot> inventorySlots; // ���� UI Slot ��

    [Header("# Time Line")]

    PlayableDirector playerDirector;

    [Header("# SaveData")]
    public List<String> inventoryDatas;



    private void Awake()
    {
        FirstSetUP();
    }

    private void Start()
    {
        if (CommonUIManager.instance != null)
        {
            CommonUIManager.instance.mainUIManager = this;
            optionUI = CommonUIManager.instance.optionUI;
            uiObjects.Add(optionUI);

            if (cellPhoneObjs != null)
            {
                CommonUIManager.instance.phoneInfos.cellPhoneObj = cellPhoneObjs;
            }

            CommonUIManager.instance.phoneInfos.cellPhoneUI = uiObjects[2];
            //Debug.Log(CommonUIManager.instance.phoneInfos.cellPhoneUI.name);

            if (CommonUIManager.instance.phoneInfos.hasPhone&& cellPhoneObjs != null)
            {
                cellPhoneObjs.SetActive(false);

            }
          
        }
    }

    private void Update()
    {
       // Debug.Log(CommonUIManager.instance.phoneInfos.cellPhoneUI.name);

        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects) && CommonUIManager.instance != null)
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
                InGameOpenUI(uiObjects[3]);
            }
            else if (uiObjects[3].activeInHierarchy)
            {
                InGameCloseUI(uiObjects[0]);
                InGameCloseUI(uiObjects[3]);
            }
        }

        // I Ű -> �޴���
        if (Input.GetKeyDown(KeyCode.I) && CommonUIManager.instance.phoneInfos.hasPhone)
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                
                if (CommonUIManager.instance.phoneInfos.cellPhoneUI == null)
                {
                    Debug.LogError("cellPhoneUI is NULL in Update at time: " + Time.time);
                }
                else
                {
                    Debug.Log(CommonUIManager.instance.phoneInfos.cellPhoneUI.name);
                    OpenCellPhoneItem(CommonUIManager.instance.phoneInfos);
                }
            }
            else if (uiObjects[2].activeInHierarchy)
            {
                InGameCloseUI(uiObjects[0]);
                InGameCloseUI(uiObjects[2]);
            }
        }
    }

    private void OnDisable()
    {
        if (CommonUIManager.instance != null)
        {
            CommonUIManager.instance.mainUIManager = null;
        }
       
    }

    private void OnApplicationQuit()
    {
        if (inventory.Count <= 0 || inventory == null)
            return;

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

    // �޴��� ���� �Լ� -> I Ű
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone)
    {
        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        InGameOpenUI(uiObjects[2]);

        Debug.Log(cellPhone.name);
        Debug.Log(cellPhone.cellPhoneUI.name);
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

    public class Item
    {
        public string name;
        public Sprite itemImg;
        public GameObject uiObj;
        public SchoolUIManager schoolUIManager;
    }
}
