using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SchoolUIManager;

public class MainUIManager : UIUtility
{
    public List<VerticalLayoutGroup> textBoxLayouts;

    public GameObject cellPhoneObjs;
    public CharacterPhoneInfo phoneInfos; // 휴대폰 정보를 담는 변수

    private void OnEnable()
    {
        CommonUIManager.instance.mainUIManager = this;
        optionUI = CommonUIManager.instance.optionUI;
    }

    private void Start()
    {
        CommonUIManager.instance.mainUIManager = this;
        optionUI = CommonUIManager.instance.optionUI;

        phoneInfos = new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs, cellPhoneUI = uiObjects[2] };
    }

    private void Update()
    {
        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects))
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
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (AreAllObjectsDisabled(uiObjects))
        //    {
        //        InGameOpenUI(uiObjects[0]); // blur 배경 활성화
        //        InGameOpenUI(uiObjects[3]);
        //    }
        //    else if (uiObjects[3].activeInHierarchy)
        //    {
        //        uiObjects[0].SetActive(false);
        //        uiObjects[3].SetActive(false);
        //    }
        //}

        // I 키 -> 휴대폰
        if (Input.GetKeyDown(KeyCode.I) && phoneInfos.hasPhone)
        {
            OpenCellPhoneItem(phoneInfos);
        }
    }

    private void OnDisable()
    {
        CommonUIManager.instance.mainUIManager = null;
    }

    // 휴대폰 열기 함수 -> I 키
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone)
    {
        InGameOpenUI(uiObjects[0]); // blur 배경 활성화
        InGameOpenUI(uiObjects[2]);

        CellPhone cpLogic = cellPhone.cellPhoneUI.GetComponent<CellPhone>();


        // 잠금해제를 한 상태가 아니라면 초기화
        if (!cellPhone.isUnlocked)
        {
            cpLogic.SetFirst();
        }
        else
        {
            Debug.Log($"name : {cellPhone.name} , isUnlocked : {cellPhone.isUnlocked}");

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

    // 휴대폰 정보 Class
    public class CharacterPhoneInfo
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
        public GameObject cellPhoneObj;
        public GameObject cellPhoneUI;
    }


}
