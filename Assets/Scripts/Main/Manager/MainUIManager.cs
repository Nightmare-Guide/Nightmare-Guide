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
    public List<Sprite> itemImgs; // 인벤토리에 들어갈 이미지들
    public List<Item> inventory; // 플레이어 인벤토리 데이터
    public List<Item> items; // 인게임 아이템 데이터
    public List<ItemSlot> inventorySlots; // 실제 UI Slot 들

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

        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects) && CommonUIManager.instance != null)
            {
                // 일시정지 UI 활성화
                PauseGame(uiObjects[0]);
            }
            else
            {
                // 모든 UI 닫기
                foreach (GameObject uiObj in uiObjects)
                {
                    if (uiObj.activeInHierarchy)
                    {
                        InGameCloseUI(uiObj);
                    }
                }
            }
        }

        // Tab 키 -> 인벤토리
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                InGameOpenUI(uiObjects[0]); // blur 배경 활성화
                InGameOpenUI(uiObjects[3]);
            }
            else if (uiObjects[3].activeInHierarchy)
            {
                InGameCloseUI(uiObjects[0]);
                InGameCloseUI(uiObjects[3]);
            }
        }

        // I 키 -> 휴대폰
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

    // 시작 세팅 함수
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // 마우스 커서 중앙에 고정
    }

    // 휴대폰 열기 함수 -> I 키
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone)
    {
        InGameOpenUI(uiObjects[0]); // blur 배경 활성화
        InGameOpenUI(uiObjects[2]);

        Debug.Log(cellPhone.name);
        Debug.Log(cellPhone.cellPhoneUI.name);
        CellPhone cpLogic = cellPhone.cellPhoneUI.GetComponent<CellPhone>();


        // 잠금해제를 한 상태가 아니라면 초기화
        if (!cellPhone.isUnlocked)
        {
            cpLogic.SetFirst();
        }
        else
        {
            // Lock Screen 비활성화, App Screen UI 활성화
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
