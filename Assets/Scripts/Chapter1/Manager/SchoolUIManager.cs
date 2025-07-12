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
using static ProgressManager;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public class SchoolUIManager : UIUtility
{
    [Header("# School Object")]
    public GameObject[] cellPhoneObjs;
    public List<PhoneInfos> phoneInfos; // 각각 휴대폰 정보를 담는 list
    public List<VerticalLayoutGroup> textBoxLayouts;
    [SerializeField] public Enemy schoolEnemy;
    [SerializeField] private Enemy backroomEnemy;
    [SerializeField] private Enemy lastEnemy;
    [SerializeField] private List<GameObject> timeLineEnemys;
    public GameObject playerObj;
    public Transform[] playerRespawnPoints;
    public Transform[] enemyRespawnPoints;
    public Transform[] timelineEnemyPoints;
    [SerializeField] Collider ehtanLocker;
    [SerializeField] GameObject fakeWall;
    [SerializeField] List<GameObject> schoolLights;
    public GameObject flashlightWall;
    public GameObject enemyFirstMeetWall;
    public List<GameObject> activeObjs;
    public List<GameObject> schoolMaps;
    public float monsterTimer = 0f;
    public float monsterWaitTime = 60f;
    public bool enterLounge = false;
    public bool useLockerKey = false;

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
        if (ProgressManager.Instance != null)
            progressManager = ProgressManager.Instance;
        if (SoundManager.instance != null)
            soundManager = SoundManager.instance;

        soundManager.PlayBGM(soundManager.windSound);

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
            if (AreAllObjectsDisabled(uiObjects) && commonUIManager != null)
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
                        if (PlayerController.instance.stat == PlayerController.PlayerState.Hide || PlayerController.instance.stat == PlayerController.PlayerState.Moving) // 락커 안 or 이동 애니메이션
                        {
                            CloseUI(uiObj);
                            Time.timeScale = 1;
                            CursorLocked();
                        }
                        else
                        {
                            InGameCloseUI(uiObj);
                        }
                    }
                }

                // TimeLine 이 정지 중이면 다시 재생
                if (playableDirector != null && playableDirector.state == PlayState.Paused && playableDirector.playableAsset != null)
                {
                    playableDirector.Play();

                    // 플레이어 움직임 멈춤
                    PlayerController.instance.Close_PlayerController();
                    Camera_Rt.instance.Close_Camera();
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
        progressManager.progressData.schoolInventoryDatas.Add(items.Find(info => obj.gameObject.name.Contains(info.name)).name);

        // 인벤토리 정리
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].itemData = inventory[i];
        }

        if (obj.name.Contains("Locker"))
        {
            GetLockerKey();
        }
        else if (obj.name.Contains("Janitor's office Key"))
        {
            GetOfficeKey();
        }

        // 사운드
        soundManager.GetItemSound();
    }

    // 아이템 사용 함수
    public void UseItem(Item item)
    {
        inventory.Remove(item);
        progressManager.progressData.schoolInventoryDatas.Remove(item.name);

        // 인벤토리 정리
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots[i].itemData = inventory[i];
        }

        if(inventory.Count == 0)
        {
            inventorySlots[0].itemData = null;
        }
    }

    public bool CheckItem(string name)
    {
        return inventory.Exists(info => name.Contains(info.name));
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

        progressManager.CompletedAction(ActionType.EnteredSchool);
    }

    void GetProgressData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // 휴대폰
            phoneInfos[0].hasPhone = progressManager.progressData.phoneDatas[1].hasPhone; // Ethan
            phoneInfos[0].isUnlocked = progressManager.progressData.phoneDatas[1].isUnlocked;
            phoneInfos[1].hasPhone = progressManager.progressData.phoneDatas[2].hasPhone; // David
            phoneInfos[1].isUnlocked = progressManager.progressData.phoneDatas[2].isUnlocked;

            // 데이터에 맞게 오브젝트 활성화/비활성화
            if (phoneInfos[0].hasPhone) { cellPhoneObjs[0].SetActive(false); } // 에단 휴대폰
            if (phoneInfos[1].hasPhone) { cellPhoneObjs[1].SetActive(false); } // 데이비드 휴대폰
            CheckObjData(ActionType.FirstMeetEthan, ehtanLocker);
            CheckObjData(ActionType.GetFlashlight, activeObjs[0]); // 손전등
            fakeWall.SetActive(!progressManager.IsActionCompleted(ActionType.FirstMeetEthan) && !progressManager.IsActionCompleted(ActionType.GetFlashlight));
            CheckObjData(ActionType.GetJanitorsOfficeKey, activeObjs[1]); // 관리실 열쇠
            CheckObjData(ActionType.GetLockerKey, activeObjs[2]); // 락커 열쇠
            activeObjs[3].SetActive(progressManager.IsActionCompleted(ActionType.GetLockerKey)); // 휴게실 Red Lright
            activeObjs[4].SetActive(progressManager.IsActionCompleted(ActionType.GetLockerKey)); // 휴게실 추격 Trigger
            activeObjs[8].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan)); // Portal Room 입장 Trigger
            activeObjs[9].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan));  // Backroom 입장 Trigger
            activeObjs[12].GetComponent<Door>().enabled = progressManager.IsActionCompleted(ActionType.ClearBackRoom) && !progressManager.IsActionCompleted(ActionType.EnteredEthanHouse); // Ethan House 입장 문
            activeObjs[13].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom));  // Backroom 퇴장 Trigger
            activeObjs[14].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom));  // Ethan House 입장 Trigger
            activeObjs[15].SetActive(!phoneInfos[1].hasPhone); // Locker Room 입장 벽

            bool isFirstMeetEthan = progressManager.IsActionCompleted(ActionType.FirstMeetEthan);
            bool isGetLockerKey = progressManager.IsActionCompleted(ActionType.GetLockerKey);
            ehtanLocker.enabled = isFirstMeetEthan == isGetLockerKey;

            // Backroom Door 기능 데이비드 휴대폰 획득 및 백룸 입장 여부로 활성화/비활성화
            activeObjs[10].GetComponent<Door>().enabled = phoneInfos[1].hasPhone && !progressManager.IsActionCompleted(ActionType.EnteredBackRoom);

            // 데이터에 맞게 맵 활성화/비활성화
            schoolMaps[0].SetActive(!progressManager.IsActionCompleted(ActionType.EnterPortalRoom) && !progressManager.IsActionCompleted(ActionType.LeaveEthan)); // 기본(First) 학교
            schoolMaps[1].SetActive(!progressManager.IsActionCompleted(ActionType.EnterPortalRoom) && !progressManager.IsActionCompleted(ActionType.LeaveEthan)); // 기본(First) 휴게실
            schoolMaps[2].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan) && !progressManager.IsActionCompleted(ActionType.OutOfBackRoom)); // Portal Room
            schoolMaps[3].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan) && !progressManager.IsActionCompleted(ActionType.EnteredEthanHouse)); // Backroom
            schoolMaps[4].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom)); // Ethan House
            // schoolMaps[5].SetActive(phoneInfos[1].hasPhone); // Locker Room 및 마지막 추격 맵

            // Backroom 을 클리어 했으나 탈출하고 않고 종료 후 재접 시, 문 개방
            if (progressManager.IsActionCompleted(ActionType.ClearBackRoom) && !progressManager.IsActionCompleted(ActionType.OutOfBackRoom))
            {
                if (!activeObjs[11].GetComponent<Door>().doorState) { activeObjs[11].GetComponent<Door>().Select_Door(); }
            }

            // Backroom Enemy
            backroomEnemy.gameObject.SetActive(progressManager.IsActionCompleted(ActionType.EnteredBackRoom) && !progressManager.IsActionCompleted(ActionType.ClearBackRoom));

            // 포스트 프로세싱, Fog 설정
            GetPostFogData();

            // 인벤토리
            inventory = new List<Item>();

            // 아이템
            foreach (string itemName in progressManager.progressData.schoolInventoryDatas)
            {
                inventory.Add(items.Find(info => info.name.Contains(itemName)));
            }

            // 인벤토리 정리
            for (int i = 0; i < inventory.Count; i++)
            {
                inventorySlots[i].itemData = inventory[i];
            }

            // 학교 맵 추격 중 게임 종료 후 다시 접속 시
            if (progressManager.IsActionCompleted(ActionType.GetLockerKey) && !progressManager.IsActionCompleted(ActionType.GetOutOfLocker))
            {
                RespawnDuringSchoolChase();
            }
            // 백룸 안에서 종료 후 다시 접속 시
            else if(progressManager.IsActionCompleted(ActionType.EnteredBackRoom) && !progressManager.IsActionCompleted(ActionType.ClearBackRoom))
            {
                RespawnDuringBackroom();
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
        if (CommonUIManager.instance != null)
        {
            CommonUIManager.instance.Blink(false);

            yield return new WaitForSeconds(CommonUIManager.instance.blinkDuration);

            PlayerMainCamera playerCamera = PlayerMainCamera.camera_single;

            soundManager.sfxSource.Stop();
            playerCamera.jumpscareObj.SetActive(false);
            playerCamera.InitCameraRotation();
            playerCamera.jumpscareObj.transform.localPosition = new Vector3(0, -1.77f, 1);
            playerCamera.jumpscareObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

            int respawnPointIndex = 0;

            // Action Type 에 따라서 리스폰 위치 설정
            switch (actionType)
            {
                case ProgressManager.ActionType.FirstMeetMonster:
                    respawnPointIndex = 0;
                    RespawnDuringSchoolChase();
                    break;
                case ProgressManager.ActionType.EnteredBackRoom:
                    respawnPointIndex = 1;
                    RespawnDuringBackroom();
                    break;
                case ProgressManager.ActionType.SolvedLockerRoom:
                    respawnPointIndex = 2;
                    lastEnemy.InitEnemy(enemyRespawnPoints[respawnPointIndex]);
                    break;
            }

            InitPlayer(playerRespawnPoints[respawnPointIndex]);

            yield return null;

            CommonUIManager.instance.Blink(true);

            yield return new WaitForSeconds(CommonUIManager.instance.blinkDuration);

            if(actionType == ProgressManager.ActionType.EnteredBackRoom)
            {
                backroomEnemy.gameObject.SetActive(true);
            }
        }
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

    void RespawnDuringSchoolChase()
    {
        Debug.Log("RespawnDuringSchoolChase");
        enemyFirstMeetWall.SetActive(true);
        schoolEnemy.InitEnemy(enemyRespawnPoints[0]);
        InitPlayer(playerRespawnPoints[0]);
        InitTimelineEnemy(timeLineEnemys[0].transform, timelineEnemyPoints[0]);
        InitTimelineEnemy(timeLineEnemys[1].transform, timelineEnemyPoints[1]);
        activeObjs[4].SetActive(true); // Lounge Trigger
        activeObjs[5].GetComponent<Door>().enabled = true;
        monsterTimer = 0;
        activeObjs[7].GetComponent<Collider>().enabled = true;
        enterLounge = false;
    }

    void RespawnDuringBackroom()
    {
        Debug.Log("RespawnDuringBackroom");
        backroomEnemy.InitEnemy(enemyRespawnPoints[1]);
        InitPlayer(playerRespawnPoints[1]);
        Maze_Mgr.instance.Btn_Clear();
    }

    void InitTimelineEnemy(Transform timelineEnemy, Transform respawnTransform)
    {
        timelineEnemy.position = respawnTransform.position;
        timelineEnemy.rotation = respawnTransform.rotation;
    }

    public void FirstMeetEthan(bool getFlashlight)
    {
        CSVRoad_Story.instance.OnSelectChapter("1_0_1"); // 테스트 때문에 1_0_0 에서 변경
        activeObjs[7].GetComponent<AudioSource>().Stop();
        activeObjs[7].GetComponent<AudioSource>().enabled = false;
        fakeWall.SetActive(false);
        flashlightWall.SetActive(!getFlashlight);
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();
        commonUIManager.isTalkingWithNPC = true;
        progressManager.CompletedAction(ActionType.FirstMeetEthan);
    }

    public void GetLockerKey()
    {
        // 조명 다 끄기
        foreach(GameObject light in schoolLights)
        {
            light.SetActive(false);
        }

        // trigger 벽 활성화
        enemyFirstMeetWall.SetActive(true);

        ehtanLocker.enabled = true;

        commonUIManager.ApplyFog(commonUIManager.fogSettings[0]);
        Camera_Rt.instance.ApplyPostProcessing("Nightmare");

        activeObjs[3].SetActive(true);
        activeObjs[4].SetActive(true);

        // 사운드
        soundManager.LockerFallSound();

        progressManager.CompletedAction(ActionType.GetLockerKey);
    }

    public void GetOfficeKey()
    {
        progressManager.CompletedAction(ActionType.GetJanitorsOfficeKey);
    }

    public void FirstMeetEnemy()
    {
        enemyFirstMeetWall.SetActive(false);
        timeLineEnemys[0].SetActive(false);
        activeObjs[6].GetComponent<Collider>().enabled = false;
        Camera_Rt.instance.postProecessingBehaviour.gameObject.GetComponent<RayCast_Aim>().flashlight.SetActive(true);
        StartTimeLine(TimeLineManager.instance.playableAssets[1]);
    }

    // 몬스터와 첫 추격 후 휴게실 문을 닫았을 때 실행
    public void CloseLoungeDoor()
    {
        StartCoroutine(StartMonsterTimer());
    }

    private IEnumerator StartMonsterTimer()
    {
        monsterTimer = 0f;

        yield return new WaitForSeconds(2f);

        schoolEnemy.transform.position = timelineEnemyPoints[1].position;
        schoolEnemy.transform.rotation = timelineEnemyPoints[1].rotation;

        schoolEnemy.GetComponent<AudioSource>().Stop();
        schoolEnemy.GetComponent<AudioSource>().clip = null;
        schoolEnemy.GetComponent<AudioSource>().clip = soundManager.doorBangSound;
        schoolEnemy.GetComponent<AudioSource>().Play();

        // waitTime 시간 동안 기다리기
        while (monsterTimer < monsterWaitTime)
        {
            if (progressManager.progressData.hideInLocker)
            {
                Debug.Log("hideInLocker");
                yield break; // 코루틴 즉시 종료
            }

            monsterTimer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("Finish Timer");
        MonsterWaitTimeOver(activeObjs[5].GetComponent<Door>());
    }

    public void MonsterWaitTimeOver(Door door)
    {
        schoolEnemy.transform.position = timelineEnemyPoints[1].position;
        schoolEnemy.transform.rotation = timelineEnemyPoints[1].rotation;

        door.Select_Door(); // 문 열기
        door.gameObject.GetComponent<Door>().enabled = false;

        schoolEnemy.GetComponent<AudioSource>().Stop();
        schoolEnemy.GetComponent<AudioSource>().clip = null;
        schoolEnemy.GetComponent<AudioSource>().clip = soundManager.enemyRunSound;
        schoolEnemy.GetComponent<AudioSource>().Play();
    }

    public void UseLockerKey()
    {
        CSVRoad_Story.instance.OnSelectChapter("1_0_4");
        commonUIManager.isTalkingWithNPC = true;
        activeObjs[6].GetComponent<Collider>().enabled = true; // 휴게실 락커 문
        useLockerKey = true;
        monsterTimer -= 10f;
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();
        progressManager.CompletedAction(ActionType.UseLockerKey);
        soundManager.KeyFailSound();
    }

    public void StartLoungeTimeLine()
    {
        progressManager.progressData.hideInLocker = true;
        schoolEnemy.gameObject.SetActive(false);
        activeObjs[4].SetActive(false); // Lounge Trigger
        activeObjs[5].GetComponent<Door>().enabled = false; // Lounge Door
        activeObjs[6].GetComponent<Collider>().enabled = false; // Lounge Locker Door

        StartTimeLine(TimeLineManager.instance.playableAssets[2]);
    }

    public void FinishLoungeTimeLine()
    {
        activeObjs[5].tag = "SpecialDoor"; // Lounge Door
        activeObjs[7].GetComponent<Collider>().enabled = true; // Ethan Locker

        progressManager.CompletedAction(ActionType.GetOutOfLocker);
    }

    public void GetOutOfLoungeLocker()
    {
        CSVRoad_Story.instance.OnSelectChapter("1_0_6");
        commonUIManager.isTalkingWithNPC = true;
        activeObjs[5].GetComponent<Door>().enabled = true; // 휴게실 문
        schoolMaps[0].SetActive(false); // First School
        schoolMaps[2].SetActive(true); // Portal Room
        schoolMaps[3].SetActive(true); // Backroom
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();
        progressManager.CompletedAction(ActionType.LeaveEthan);
    }

    public void EnterPortalRoom()
    {
        if (!ForceCloseDoor(activeObjs[5].GetComponent<Door>())) // Lounge Door -> 문 닫기 및 Trigger 비활성화
            return;

        progressManager.CompletedAction(ProgressManager.ActionType.EnterPortalRoom);

        activeObjs[8].SetActive(false); // Portal Room 입장 Trigger
        activeObjs[9].SetActive(true);  // Backroom 입장 Trigger
    }

    public void EnterBackroom()
    {
        if (!ForceCloseDoor(activeObjs[10].GetComponent<Door>())) // Backroom Door -> 문 닫기 및 Trigger 비활성화
            return;

        progressManager.CompletedAction(ActionType.EnteredBackRoom);

        activeObjs[9].SetActive(false); // Backroom 입장 Trigger
        backroomEnemy.gameObject.SetActive(true); // Backroom Enemy 활성화
    }

    public void ClearBackroom()
    {
        activeObjs[13].SetActive(true); // Backroom Clear Trigger
        activeObjs[14].SetActive(true); // Ethan House Enter Trigger

        backroomEnemy.gameObject.SetActive(false); // 클리어 즉시 몬스터 비활성화

        schoolMaps[4].SetActive(true); // Ethan House

        progressManager.CompletedAction(ActionType.ClearBackRoom);
    }

    public void OutOfBackroom()
    {
        if (!ForceCloseDoor(activeObjs[11].GetComponent<Door>())) // 백룸 탈출 문 -> 문 닫기 및 Trigger 비활성화
            return;

        activeObjs[13].SetActive(false); // Backroom Out Trigger

        // 맵 비활성화
        schoolMaps[1].SetActive(false); // 학교 휴게실
        schoolMaps[2].SetActive(false); // 포탈 방

        progressManager.CompletedAction(ActionType.OutOfBackRoom);
    }

    public void EnterEthanHouse()
    {
        if (!ForceCloseDoor(activeObjs[12].GetComponent<Door>())) // Ethan House 문 -> 문 닫기 및 Trigger 비활성화
            return;

        progressManager.CompletedAction(ActionType.EnteredEthanHouse);

        activeObjs[14].SetActive(false); // Ethan House 입장 Trigger
    }

    public void GetDavidCellPhone()
    {
        ProgressManager.Instance.progressData.phoneDatas[2].hasPhone = true;
        ProgressManager.Instance.CompletedAction(ActionType.GetDavidCellPhone);

        activeObjs[10].gameObject.GetComponent<Door>().enabled = true; // Barkroom 입장 문
    }

    public void GetEthanCellPhone()
    {
        ProgressManager.Instance.progressData.phoneDatas[1].hasPhone = true;

        activeObjs[15].SetActive(false); // 락커룸 가는 fake wall 비활성화
        schoolMaps[5].SetActive(true); // 락커룸 및 마지막 추격 맵

        soundManager.WallMoveSound();
    }

    public void FinishSchoolScene()
    {
        commonUIManager.MoveScene("NightHospital");
    }
}
