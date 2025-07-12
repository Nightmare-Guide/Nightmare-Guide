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
    public List<PhoneInfos> phoneInfos; // ���� �޴��� ������ ��� list
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

        InitItemDatas(); // ������ ������ �ʱ�ȭ
        GetProgressData(); // ����� ������ ��������
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects) && commonUIManager != null)
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
                        if (PlayerController.instance.stat == PlayerController.PlayerState.Hide || PlayerController.instance.stat == PlayerController.PlayerState.Moving) // ��Ŀ �� or �̵� �ִϸ��̼�
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

                // TimeLine �� ���� ���̸� �ٽ� ���
                if (playableDirector != null && playableDirector.state == PlayState.Paused && playableDirector.playableAsset != null)
                {
                    playableDirector.Play();

                    // �÷��̾� ������ ����
                    PlayerController.instance.Close_PlayerController();
                    Camera_Rt.instance.Close_Camera();
                }

                if (PlayerMainCamera.camera_single.jumpscareObj.activeInHierarchy)
                {
                    // �÷��̾� ������ ����
                    PlayerController.instance.Close_PlayerController();
                    Camera_Rt.instance.Close_Camera();
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
        progressManager.progressData.schoolInventoryDatas.Add(items.Find(info => obj.gameObject.name.Contains(info.name)).name);

        // �κ��丮 ����
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

        // ����
        soundManager.GetItemSound();
    }

    // ������ ��� �Լ�
    public void UseItem(Item item)
    {
        inventory.Remove(item);
        progressManager.progressData.schoolInventoryDatas.Remove(item.name);

        // �κ��丮 ����
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
        // �޴��� ������ �Է�
        phoneInfos.Add(new PhoneInfos { name = "Ethan", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[0], cellPhoneUI = uiObjects[2] });
        phoneInfos.Add(new PhoneInfos { name = "David", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs[1], cellPhoneUI = uiObjects[3] });

        // ������ ������ �Է�
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

            // �޴���
            phoneInfos[0].hasPhone = progressManager.progressData.phoneDatas[1].hasPhone; // Ethan
            phoneInfos[0].isUnlocked = progressManager.progressData.phoneDatas[1].isUnlocked;
            phoneInfos[1].hasPhone = progressManager.progressData.phoneDatas[2].hasPhone; // David
            phoneInfos[1].isUnlocked = progressManager.progressData.phoneDatas[2].isUnlocked;

            // �����Ϳ� �°� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
            if (phoneInfos[0].hasPhone) { cellPhoneObjs[0].SetActive(false); } // ���� �޴���
            if (phoneInfos[1].hasPhone) { cellPhoneObjs[1].SetActive(false); } // ���̺�� �޴���
            CheckObjData(ActionType.FirstMeetEthan, ehtanLocker);
            CheckObjData(ActionType.GetFlashlight, activeObjs[0]); // ������
            fakeWall.SetActive(!progressManager.IsActionCompleted(ActionType.FirstMeetEthan) && !progressManager.IsActionCompleted(ActionType.GetFlashlight));
            CheckObjData(ActionType.GetJanitorsOfficeKey, activeObjs[1]); // ������ ����
            CheckObjData(ActionType.GetLockerKey, activeObjs[2]); // ��Ŀ ����
            activeObjs[3].SetActive(progressManager.IsActionCompleted(ActionType.GetLockerKey)); // �ްԽ� Red Lright
            activeObjs[4].SetActive(progressManager.IsActionCompleted(ActionType.GetLockerKey)); // �ްԽ� �߰� Trigger
            activeObjs[8].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan)); // Portal Room ���� Trigger
            activeObjs[9].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan));  // Backroom ���� Trigger
            activeObjs[12].GetComponent<Door>().enabled = progressManager.IsActionCompleted(ActionType.ClearBackRoom) && !progressManager.IsActionCompleted(ActionType.EnteredEthanHouse); // Ethan House ���� ��
            activeObjs[13].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom));  // Backroom ���� Trigger
            activeObjs[14].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom));  // Ethan House ���� Trigger
            activeObjs[15].SetActive(!phoneInfos[1].hasPhone); // Locker Room ���� ��

            bool isFirstMeetEthan = progressManager.IsActionCompleted(ActionType.FirstMeetEthan);
            bool isGetLockerKey = progressManager.IsActionCompleted(ActionType.GetLockerKey);
            ehtanLocker.enabled = isFirstMeetEthan == isGetLockerKey;

            // Backroom Door ��� ���̺�� �޴��� ȹ�� �� ��� ���� ���η� Ȱ��ȭ/��Ȱ��ȭ
            activeObjs[10].GetComponent<Door>().enabled = phoneInfos[1].hasPhone && !progressManager.IsActionCompleted(ActionType.EnteredBackRoom);

            // �����Ϳ� �°� �� Ȱ��ȭ/��Ȱ��ȭ
            schoolMaps[0].SetActive(!progressManager.IsActionCompleted(ActionType.EnterPortalRoom) && !progressManager.IsActionCompleted(ActionType.LeaveEthan)); // �⺻(First) �б�
            schoolMaps[1].SetActive(!progressManager.IsActionCompleted(ActionType.EnterPortalRoom) && !progressManager.IsActionCompleted(ActionType.LeaveEthan)); // �⺻(First) �ްԽ�
            schoolMaps[2].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan) && !progressManager.IsActionCompleted(ActionType.OutOfBackRoom)); // Portal Room
            schoolMaps[3].SetActive(progressManager.IsActionCompleted(ActionType.LeaveEthan) && !progressManager.IsActionCompleted(ActionType.EnteredEthanHouse)); // Backroom
            schoolMaps[4].SetActive(progressManager.IsActionCompleted(ActionType.ClearBackRoom)); // Ethan House
            // schoolMaps[5].SetActive(phoneInfos[1].hasPhone); // Locker Room �� ������ �߰� ��

            // Backroom �� Ŭ���� ������ Ż���ϰ� �ʰ� ���� �� ���� ��, �� ����
            if (progressManager.IsActionCompleted(ActionType.ClearBackRoom) && !progressManager.IsActionCompleted(ActionType.OutOfBackRoom))
            {
                if (!activeObjs[11].GetComponent<Door>().doorState) { activeObjs[11].GetComponent<Door>().Select_Door(); }
            }

            // Backroom Enemy
            backroomEnemy.gameObject.SetActive(progressManager.IsActionCompleted(ActionType.EnteredBackRoom) && !progressManager.IsActionCompleted(ActionType.ClearBackRoom));

            // ����Ʈ ���μ���, Fog ����
            GetPostFogData();

            // �κ��丮
            inventory = new List<Item>();

            // ������
            foreach (string itemName in progressManager.progressData.schoolInventoryDatas)
            {
                inventory.Add(items.Find(info => info.name.Contains(itemName)));
            }

            // �κ��丮 ����
            for (int i = 0; i < inventory.Count; i++)
            {
                inventorySlots[i].itemData = inventory[i];
            }

            // �б� �� �߰� �� ���� ���� �� �ٽ� ���� ��
            if (progressManager.IsActionCompleted(ActionType.GetLockerKey) && !progressManager.IsActionCompleted(ActionType.GetOutOfLocker))
            {
                RespawnDuringSchoolChase();
            }
            // ��� �ȿ��� ���� �� �ٽ� ���� ��
            else if(progressManager.IsActionCompleted(ActionType.EnteredBackRoom) && !progressManager.IsActionCompleted(ActionType.ClearBackRoom))
            {
                RespawnDuringBackroom();
            }
        }
        else
        {
            //Debug.Log("������ �������� �ʽ��ϴ�.");
        }
    }

    // �������� �׾��� ���� �����ؾ��ϴ� �Լ�
    public IEnumerator RevivalPlayer(ProgressManager.ActionType actionType)
    {
        // Blink UI ����
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

            // Action Type �� ���� ������ ��ġ ����
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

        //ī�޶� ȸ�� Ȱ��ȭ
        Camera_Rt.instance.Open_Camera();

        //�÷��̾� ��Ʈ�� On
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
        CSVRoad_Story.instance.OnSelectChapter("1_0_1"); // �׽�Ʈ ������ 1_0_0 ���� ����
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
        // ���� �� ����
        foreach(GameObject light in schoolLights)
        {
            light.SetActive(false);
        }

        // trigger �� Ȱ��ȭ
        enemyFirstMeetWall.SetActive(true);

        ehtanLocker.enabled = true;

        commonUIManager.ApplyFog(commonUIManager.fogSettings[0]);
        Camera_Rt.instance.ApplyPostProcessing("Nightmare");

        activeObjs[3].SetActive(true);
        activeObjs[4].SetActive(true);

        // ����
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

    // ���Ϳ� ù �߰� �� �ްԽ� ���� �ݾ��� �� ����
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

        // waitTime �ð� ���� ��ٸ���
        while (monsterTimer < monsterWaitTime)
        {
            if (progressManager.progressData.hideInLocker)
            {
                Debug.Log("hideInLocker");
                yield break; // �ڷ�ƾ ��� ����
            }

            monsterTimer += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("Finish Timer");
        MonsterWaitTimeOver(activeObjs[5].GetComponent<Door>());
    }

    public void MonsterWaitTimeOver(Door door)
    {
        schoolEnemy.transform.position = timelineEnemyPoints[1].position;
        schoolEnemy.transform.rotation = timelineEnemyPoints[1].rotation;

        door.Select_Door(); // �� ����
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
        activeObjs[6].GetComponent<Collider>().enabled = true; // �ްԽ� ��Ŀ ��
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
        activeObjs[5].GetComponent<Door>().enabled = true; // �ްԽ� ��
        schoolMaps[0].SetActive(false); // First School
        schoolMaps[2].SetActive(true); // Portal Room
        schoolMaps[3].SetActive(true); // Backroom
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();
        progressManager.CompletedAction(ActionType.LeaveEthan);
    }

    public void EnterPortalRoom()
    {
        if (!ForceCloseDoor(activeObjs[5].GetComponent<Door>())) // Lounge Door -> �� �ݱ� �� Trigger ��Ȱ��ȭ
            return;

        progressManager.CompletedAction(ProgressManager.ActionType.EnterPortalRoom);

        activeObjs[8].SetActive(false); // Portal Room ���� Trigger
        activeObjs[9].SetActive(true);  // Backroom ���� Trigger
    }

    public void EnterBackroom()
    {
        if (!ForceCloseDoor(activeObjs[10].GetComponent<Door>())) // Backroom Door -> �� �ݱ� �� Trigger ��Ȱ��ȭ
            return;

        progressManager.CompletedAction(ActionType.EnteredBackRoom);

        activeObjs[9].SetActive(false); // Backroom ���� Trigger
        backroomEnemy.gameObject.SetActive(true); // Backroom Enemy Ȱ��ȭ
    }

    public void ClearBackroom()
    {
        activeObjs[13].SetActive(true); // Backroom Clear Trigger
        activeObjs[14].SetActive(true); // Ethan House Enter Trigger

        backroomEnemy.gameObject.SetActive(false); // Ŭ���� ��� ���� ��Ȱ��ȭ

        schoolMaps[4].SetActive(true); // Ethan House

        progressManager.CompletedAction(ActionType.ClearBackRoom);
    }

    public void OutOfBackroom()
    {
        if (!ForceCloseDoor(activeObjs[11].GetComponent<Door>())) // ��� Ż�� �� -> �� �ݱ� �� Trigger ��Ȱ��ȭ
            return;

        activeObjs[13].SetActive(false); // Backroom Out Trigger

        // �� ��Ȱ��ȭ
        schoolMaps[1].SetActive(false); // �б� �ްԽ�
        schoolMaps[2].SetActive(false); // ��Ż ��

        progressManager.CompletedAction(ActionType.OutOfBackRoom);
    }

    public void EnterEthanHouse()
    {
        if (!ForceCloseDoor(activeObjs[12].GetComponent<Door>())) // Ethan House �� -> �� �ݱ� �� Trigger ��Ȱ��ȭ
            return;

        progressManager.CompletedAction(ActionType.EnteredEthanHouse);

        activeObjs[14].SetActive(false); // Ethan House ���� Trigger
    }

    public void GetDavidCellPhone()
    {
        ProgressManager.Instance.progressData.phoneDatas[2].hasPhone = true;
        ProgressManager.Instance.CompletedAction(ActionType.GetDavidCellPhone);

        activeObjs[10].gameObject.GetComponent<Door>().enabled = true; // Barkroom ���� ��
    }

    public void GetEthanCellPhone()
    {
        ProgressManager.Instance.progressData.phoneDatas[1].hasPhone = true;

        activeObjs[15].SetActive(false); // ��Ŀ�� ���� fake wall ��Ȱ��ȭ
        schoolMaps[5].SetActive(true); // ��Ŀ�� �� ������ �߰� ��

        soundManager.WallMoveSound();
    }

    public void FinishSchoolScene()
    {
        commonUIManager.MoveScene("NightHospital");
    }
}
