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

public class SchoolUIManager : UIUtility
{
    [Header("# School Object")]
    public GameObject[] cellPhoneObjs;
    public List<PhoneInfos> phoneInfos; // ���� �޴��� ������ ��� list
    public List<VerticalLayoutGroup> textBoxLayouts;
    [SerializeField] private Enemy schoolEnemy;
    [SerializeField] private Enemy backroomEnemy;
    [SerializeField] private Enemy lastEnemy;
    [SerializeField] private List<GameObject> timeLineEnemys;
    public GameObject playerObj;
    public Transform[] playerRespawnPoints;
    public Transform[] enemyRespawnPoints;
    [SerializeField] Collider ehtanLocker;
    [SerializeField] GameObject fakeWall;
    [SerializeField] List<GameObject> schoolLights;
    public GameObject flashlightWall;
    public GameObject enemyFirstMeetWall;
    public List<GameObject> activeObjs;

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

        // commonUIManager.ApplyFog(commonUIManager.fogSettings[1]); // �׽�Ʈ -> ������ ���� �� �ҷ������ ����
        // Camera_Rt.instance.ApplyPostProcessing("Warm"); // �׽�Ʈ -> ������ ���� �� �ҷ������ ����

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
        ProgressManager.Instance.progressData.schoolInventoryDatas.Add(items.Find(info => obj.gameObject.name.Contains(info.name)).name);

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
    }

    // ������ ��� �Լ�
    public void UseItem(Item item)
    {
        inventory.Remove(item);
        ProgressManager.Instance.progressData.schoolInventoryDatas.Remove(item.name);

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

            // �����Ϳ� �°� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
            if (phoneInfos[0].hasPhone) { cellPhoneObjs[0].SetActive(false); }
            if (phoneInfos[1].hasPhone) { cellPhoneObjs[1].SetActive(false); }
            CheckObjData(ActionType.FirstMeetEthan, ehtanLocker);
            CheckObjData(ActionType.GetFlashlight, activeObjs[0]);
            CheckObjData(ActionType.GetFlashlight, fakeWall);
            CheckObjData(ActionType.GetJanitorsOfficeKey, activeObjs[1]);
            CheckObjData(ActionType.GetLockerKey, activeObjs[2]);

            bool isFirstMeetEthan = ProgressManager.Instance.IsActionCompleted(ActionType.FirstMeetEthan);
            bool isGetLockerKey = ProgressManager.Instance.IsActionCompleted(ActionType.GetLockerKey);
            ehtanLocker.enabled = isFirstMeetEthan == isGetLockerKey;

            // ����Ʈ ���μ���, Fog ����
            GetPostFogData();


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

    // �������� �׾��� ���� �����ؾ��ϴ� �Լ�
    public IEnumerator RevivalPlayer(ProgressManager.ActionType actionType)
    {
        // Blink UI ����
        if (CommonUIManager.instance != null)
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

        // Action Type �� ���� ������ ��ġ ����
        switch (actionType)
        {
            case ProgressManager.ActionType.FirstMeetMonster:
                respawnPointIndex = 0;
                schoolEnemy.InitEnemy(enemyRespawnPoints[respawnPointIndex]);
                break;
            case ProgressManager.ActionType.EnteredBackRoom:
                respawnPointIndex = 1;
                backroomEnemy.InitEnemy(enemyRespawnPoints[respawnPointIndex]);
                break;
            case ProgressManager.ActionType.SolvedLockerRoom:
                respawnPointIndex = 2;
                lastEnemy.InitEnemy(enemyRespawnPoints[respawnPointIndex]);
                break;
        }

        InitPlayer(playerRespawnPoints[respawnPointIndex]);
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

    public void FirstMeetEthan(bool getFlashlight)
    {
        CSVRoad_Story.instance.OnSelectChapter("1_0_1"); // �׽�Ʈ ������ 1_0_0 ���� ����
        fakeWall.SetActive(false);
        flashlightWall.SetActive(!getFlashlight);
        StopPlayerController();
        commonUIManager.isTalkingWithNPC = true;
        ProgressManager.Instance.CompletedAction(ActionType.FirstMeetEthan);
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

        // ��! �ϴ� ���� �ʿ�

        ProgressManager.Instance.CompletedAction(ActionType.GetLockerKey);
    }

    public void GetOfficeKey()
    {
        ProgressManager.Instance.CompletedAction(ActionType.GetJanitorsOfficeKey);
    }

    public void FirstMeetEnemy()
    {
        enemyFirstMeetWall.SetActive(false);
        StartTimeLine(TimeLineManager.instance.playableAssets[1]);
        timeLineEnemys[0].SetActive(false);
    }

    public void FinishSchoolScene()
    {
        commonUIManager.MoveScene("NightHospital");
    }
}
