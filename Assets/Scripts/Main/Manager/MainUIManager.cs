using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static CommonUIManager;
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
    PlayableAsset playableAsset;

    [Header("# SaveData")]
    public List<String> inventoryDatas;


    private void Awake()
    {
        FirstSetUP();
    }

    private void Start()
    {
        if (CommonUIManager.instance != null)
            commonUIManager = CommonUIManager.instance;
        if (TimeLineManager.instance != null)
            timeLineManager = TimeLineManager.instance;

        if (commonUIManager != null)
        {
            commonUIManager.uiManager = this;
            optionUI = commonUIManager.optionUI;
            uiObjects.Add(optionUI);

            if (cellPhoneObjs != null)
            {
                commonUIManager.stevenPhone.cellPhoneObj = cellPhoneObjs;
            }

            commonUIManager.stevenPhone.cellPhoneUI = uiObjects[2];
            //Debug.Log(commonUIManager.phoneInfos.cellPhoneUI.name);

            if (commonUIManager.stevenPhone.hasPhone && cellPhoneObjs != null)
            {
                cellPhoneObjs.SetActive(false);
            }
        }

        if(!ProgressManager.Instance.progressData.timelineWatchedList.Find(e => e.key == timeLineManager.playableAssets[0].name).value)
        {
            // Ÿ�Ӷ��� ����
            StartTimeLine(timeLineManager.playableAssets[0]);
        }
        else
        {
            playableAsset = null;   
        }
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (AreAllObjectsDisabled(uiObjects) && commonUIManager != null)               // -> �׽�Ʈ ������ ��Ȱ��ȭ
            //{
            //    // �Ͻ����� UI Ȱ��ȭ
            //    PauseGame(uiObjects[0]);
            //}
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

                // TimeLine �� ���� ���̸� �ٽ� ���
                if (playableDirector != null && playableDirector.state == PlayState.Paused && playableDirector.playableAsset != null)
                {
                    playableDirector.Play();
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
        if (Input.GetKeyDown(KeyCode.I) && commonUIManager.stevenPhone.hasPhone)
        {
            if (AreAllObjectsDisabled(uiObjects))
            {

                if (commonUIManager.stevenPhone.cellPhoneUI == null)
                {
                    Debug.LogError("cellPhoneUI is NULL in Update at time: " + Time.time);
                }
                else
                {
                    Debug.Log(commonUIManager.stevenPhone.cellPhoneUI.name);
                    OpenCellPhoneItem(commonUIManager.stevenPhone);
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
        if (commonUIManager != null)
        {
            commonUIManager.uiManager = null;
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
    public void OpenCellPhoneItem(PhoneInfos cellPhone)
    {
        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        InGameOpenUI(uiObjects[2]);

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
}
