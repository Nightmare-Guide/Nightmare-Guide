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

public class SchoolUIManager : UIUtility
{
    [Header("# Object")]
    public GameObject[] cellPhoneObjs;
    public List<CharacterPhoneInfo> phoneInfos; // 각각 휴대폰 정보를 담는 list
    public List<VerticalLayoutGroup> textBoxLayouts;

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌


    private void Awake()
    {
        FirstSetUP();

        phoneInfos = new List<CharacterPhoneInfo>();
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

        optionUI = CommonUIManager.instance.optionUI;
        uiObjects.Add(optionUI);
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

        // 테스트용 -> 인벤토리 휴대폰 상호작용
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

    private void OnDisable()
    {
        CommonUIManager.instance.schoolUIManager = null;
    }

    // 시작 세팅 함수
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // 마우스 커서 중앙에 고정

        Debug.Log("UI First Setup");
    }

    // 인벤토리 휴대폰 버튼 함수
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone, int index)
    {
        InGameOpenUI(uiObjects[0]); // blur 배경 활성화
        InGameOpenUI(uiObjects[index]);

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
