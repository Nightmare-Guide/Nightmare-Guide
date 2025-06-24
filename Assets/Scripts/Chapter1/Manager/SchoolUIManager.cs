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
using UnityEngine.Playables;

public class SchoolUIManager : UIUtility
{
    [Header("# School Object")]
    public GameObject[] cellPhoneObjs;
    public List<PhoneInfos> phoneInfos; // 각각 휴대폰 정보를 담는 list
    public List<VerticalLayoutGroup> textBoxLayouts;
    [SerializeField] private Enemy enemyObj;
    public GameObject playerObj;
    public Transform[] playerRespawnPoints;
    public Transform[] enemyRespawnPoints;

    [Header("# School Inventory")]
    public List<Sprite> itemImgs; // 인벤토리에 들어갈 이미지들
    public List<Item> inventory; // 플레이어 인벤토리 데이터
    public List<Item> items; // 인게임 아이템 데이터
    public List<ItemSlot> inventorySlots; // 실제 UI Slot 들

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌


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

        // 타임라인 실행 -> 테스트
        // StartTimeLine(timeLineManager.playableAssets[1]);

        if (commonUIManager != null)
        {
            optionUI = commonUIManager.optionUI;
            uiObjects.Add(optionUI);
            commonUIManager.uiManager = this;
        }

        InitItemDatas(); // 아이템 데이터 초기화
        GetProgressData(); // 저장된 데이터 가져오기
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

                // TimeLine 이 정지 중이면 다시 재생
                if (playableDirector != null && playableDirector.state == PlayState.Paused && playableDirector.playableAsset != null)
                {
                    playableDirector.Play();
                }

                if (PlayerMainCamera.camera_single.jumpscareObj.activeInHierarchy)
                {
                    // 플레이어 움직임 멈춤
                    PlayerController.instance.Close_PlayerController();
                    Camera_Rt.instance.Close_Camera();
                }
            }
        }


        // Tab 키 -> 인벤토리
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                InGameOpenUI(uiObjects[0]); // blur 배경 활성화
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
        // 모든 UI 닫기
        foreach (GameObject uiObj in uiObjects)
        {
            if (uiObj.activeInHierarchy)
            {
                InGameCloseUI(uiObj);
            }
        }
    }
    // 시작 세팅 함수
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // 마우스 커서 중앙에 고정
    }

    // 인벤토리 휴대폰 버튼 함수
    public void OpenCellPhoneItem(PhoneInfos cellPhone, GameObject uiObj)
    {
        InGameOpenUI(uiObjects[0]); // blur 배경 활성화
        InGameOpenUI(uiObj);

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

    // 아이템 획득 함수
    public void GetItem(GameObject obj)
    {
        // obj 이름을 포함하는 items 의 데이터를 inventory 에 추가
        inventory.Add(items.Find(info => obj.gameObject.name.Contains(info.name))); // info -> items List 의 요소
        // ProgressManager.Instance.progressData.schoolInventoryDatas.Add(items.Find(info => obj.gameObject.name.Contains(info.name)).name);

        // 인벤토리 정리
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].itemData = inventory[i];
        }
    }

    void InitItemDatas()
    {
        // 휴대폰 데이터 입력
        phoneInfos.Add(new PhoneInfos { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new PhoneInfos { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });

        // 아이템 데이터 입력
        items.Add(new Item { name = "Locker Key", itemImg = itemImgs[0], uiObj = null, schoolUIManager = this });
        items.Add(new Item { name = "Janitor's office Key", itemImg = itemImgs[1], uiObj = null, schoolUIManager = this });
        items.Add(new Item { name = "Ethan CellPhone", itemImg = itemImgs[2], uiObj = uiObjects[2], schoolUIManager = this });
        items.Add(new Item { name = "David CellPhone", itemImg = itemImgs[3], uiObj = uiObjects[3], schoolUIManager = this });
    }

    void GetProgressData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // 휴대폰
            phoneInfos[0].hasPhone = ProgressManager.Instance.progressData.phoneDatas[1].hasPhone;
            phoneInfos[0].isUnlocked = ProgressManager.Instance.progressData.phoneDatas[1].isUnlocked;
            phoneInfos[1].hasPhone = ProgressManager.Instance.progressData.phoneDatas[2].hasPhone;
            phoneInfos[1].isUnlocked = ProgressManager.Instance.progressData.phoneDatas[2].isUnlocked;

            if (phoneInfos[0].hasPhone) { cellPhoneObjs[0].SetActive(false); }
            if (phoneInfos[1].hasPhone) { cellPhoneObjs[1].SetActive(false); }

            inventory = new List<Item>();

            // 아이템
            foreach (string itemName in ProgressManager.Instance.progressData.schoolInventoryDatas)
            {
                inventory.Add(items.Find(info => info.name.Contains(itemName)));
            }

            // 인벤토리 정리
            for (int i = 0; i < inventory.Count; i++)
            {
                inventorySlots[i].itemData = inventory[i];
            }
        }
        else
        {
            //Debug.Log("파일이 존재하지 않습니다.");
        }
    }

    // 몬스터한테 죽었을 때에 실행해야하는 함수
    public IEnumerator RevivalPlayer(ProgressManager.ActionType actionType)
    {
        // Blink UI 실행
        if(CommonUIManager.instance != null)
        {
            CommonUIManager.instance.Blink(false);

            yield return new WaitForSeconds(CommonUIManager.instance.blinkDuration);

            PlayerMainCamera playerCamera = PlayerMainCamera.camera_single;

            playerCamera.jumpscareObj.SetActive(false);
            playerCamera.InitCameraRotation();
            playerCamera.jumpscareObj.transform.localPosition = new Vector3(0, -1.77f, 1);
            playerCamera.jumpscareObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

            CommonUIManager.instance.Blink(true);

            yield return new WaitForSeconds(CommonUIManager.instance.blinkDuration);
        }

        int respawnPointIndex = 0;

        // Action Type 에 따라서 리스폰 위치 설정
        switch (actionType)
        {
            case ProgressManager.ActionType.FirstMeetMonster:
                respawnPointIndex = 0;
                break;
            case ProgressManager.ActionType.GetDavidCellPhone:
                respawnPointIndex = 1;
                break;
            case ProgressManager.ActionType.EnteredBackRoom:
                respawnPointIndex = 2;
                break;
            case ProgressManager.ActionType.SolvedCabinetRoom:
                respawnPointIndex = 3;
                break;
        }

        enemyObj.InitEnemy(enemyRespawnPoints[respawnPointIndex]);
        InitPlayer(playerRespawnPoints[respawnPointIndex]);
    }

    void InitPlayer(Transform respawnTransform)
    {
        playerObj.transform.position = respawnTransform.position;
        playerObj.transform.rotation = respawnTransform.rotation;

        //카메라 회전 활성화
        Camera_Rt.instance.Open_Camera();

        //플레이어 컨트롤 On
        PlayerController.instance.Open_PlayerController();
    }
}
