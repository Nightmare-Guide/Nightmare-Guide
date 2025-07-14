using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityStandardAssets.Characters.FirstPerson;
using static CommonUIManager;
using static ProgressManager;
using static SchoolUIManager;
using static UnityEditor.FilePathAttribute;


//���� ���൵ ����
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("���൵ ������")]
    public ProgressData progressData;
    public ProgressData defaultData;

    public enum ActionType
    {
        // ���� ������� �Է� (���� �б��� -> ���ӵǴ� ������ ���� ����)
        StartNewDay,
        FirstMeetMichael,
        FirstMeetJames,
        TalkWithEthanMom,
        EnteredSchool,
        FirstMeetEthan,
        GetFlashlight,
        GetJanitorsOfficeKey,
        EnteredControlRoom,
        GetLockerKey,
        FirstMeetMonster,
        UseLockerKey,
        GetOutOfLocker,
        LeaveEthan,
        EnterPortalRoom,
        GetDavidCellPhone,
        EnteredBackRoom,
        ClearBackRoom,
        OutOfBackRoom,
        EnteredEthanHouse,
        GetEthanCellPhone,
        ClearLockerRoom,
        StartFinalChase,
        FinishFinalChase,
        EnterLastLounge,
        TalkWarmlyToEthan,
        BackToHospital,
        FinishFirstWork,
        FirstMeetAlex,
        CompleteFirstDay
    }

    void Awake()
    {
        // �̱��� ó��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAllActions();
    }

    // Action ���尪 �ʱ�ȭ �Լ�
    void InitAllActions()
    {
        defaultData.actionStatuses = new List<ActionStatus>();

        foreach (ActionType type in Enum.GetValues(typeof(ActionType)))
        {
            defaultData.actionStatuses.Add(new ActionStatus { actionType = type, isCompleted = false });
        }
    }

    // Action �� ����Ǿ��� �� �ȵǾ��� �� Ȯ���ϴ� �Լ�
    public bool IsActionCompleted(ActionType type)
    {
        return progressData.actionStatuses.Find(a => a.actionType == type).isCompleted;
    }

    public void CompletedAction(ActionType type)
    {
        progressData.actionStatuses.Find(a => a.actionType == type).isCompleted = true;
    }

    /// <summary>
    /// PlayerPrefs���� ����� ���൵ �ҷ���
    /// </summary>
    public void LoadProgress()
    {
        if (progressData == null)
        {
            Debug.LogError("ProgressData�� ������� �ʾҽ��ϴ�!");
            return;
        }
        // GameDataManager���� ���൵ �����͸� �̹� progressData�� �ε������Ƿ�,
        // ���⼭�� �߰����� �ε� ������ �ʿ��ϴٸ� �����մϴ�.
        // ��: ���� ���� �ʱ� ���� ��
        Debug.Log("ProgressData �ε� �Ϸ� (GameDataManager���� ������ �ε�)");
    }



    public void UpdateProgress(string newScene, string newProgress)
    {
        if (progressData != null)
        {
            progressData.scene = newScene;
            progressData.storyProgress = newProgress;
            // Debug.Log("���൵ ������Ʈ : " + newScene +" :: "+newProgress);
        }
    }

    /// <summary>
    /// ���൵ �ʱ�ȭ (�� ���� ���� ��)a
    /// </summary>
    public void ResetProgress()
    {
        if (progressData != null && defaultData != null && CommonUIManager.instance != null)
        {
            progressData.newGame = defaultData.newGame;
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            DeepCopy(defaultData.actionStatuses); // �׼� ����
            progressData.playerPosition = defaultData.playerPosition;
            progressData.playerEulerAngles = defaultData.playerEulerAngles;
            progressData.sanchi = defaultData.sanchi;
            progressData.fogName = defaultData.fogName;
            progressData.postProcessingName = defaultData.postProcessingName;
            progressData.hideInLocker = defaultData.hideInLocker;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            DeepCopy(defaultData.phoneDatas); // �޴��� ����

            progressData.schoolInventoryDatas = new List<string>(defaultData.schoolInventoryDatas);

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;
            progressData.quest = "";

            // Ÿ�Ӷ���
            DeepCopy(defaultData.timelineWatchedList);
            //�÷��̾� ���� ��ġ��
            DeepCopy(defaultData.playerTr);


            CommonUIManager.instance.ResetSoudVolume();
            /* Debug.Log("progressData.scene : " + progressData.scene);
             Debug.Log("progressData.storyProgress : " + progressData.storyProgress);
             if (progressData.phoneDatas != null && progressData.phoneDatas.Count > 0)
             {
                 Debug.Log("progressData.phoneDatas[0].hasPhone : " + progressData.phoneDatas[0].hasPhone);
                 Debug.Log("progressData.phoneDatas[0].isUnlocked : " + progressData.phoneDatas[0].isUnlocked);
             }*/
            //Debug.Log("���൵ �ʱ�ȭ��");
        }
    }

    public void InitTimeline()
    {
        progressData.timelineWatchedList = new List<TimelineEntry>();

        foreach (var playableAsset in TimeLineManager.instance.playableAssets)
        {
            progressData.timelineWatchedList.Add(new TimelineEntry { key = playableAsset.name, value = false });
        }
    }

    public void DeepCopy(List<TimelineEntry> list)
    {
        progressData.timelineWatchedList = new List<TimelineEntry>();

        foreach (var data in list)
        {
            progressData.timelineWatchedList.Add(new TimelineEntry { key = data.key, value = data.value });
        }
    }
    public void DeepCopy(List<PlayerTr> list)
    {
        progressData.playerTr = new List<PlayerTr>();

        foreach (var data in list)
        {
            progressData.playerTr.Add(new PlayerTr { scene_Name = data.scene_Name, tr = data.tr, rt = data.rt });
        }
    }
    public void DeepCopy(List<SavePhoneData> list)
    {
        progressData.phoneDatas = new List<SavePhoneData>();

        foreach (var data in list)
        {
            progressData.phoneDatas.Add(new SavePhoneData { name = data.name, hasPhone = false, isUnlocked = false }) ;
        }
    }

    public void DeepCopy(List<ActionStatus> list)
    {
        progressData.actionStatuses = new List<ActionStatus>();

        foreach(var data in list)
        {
            progressData.actionStatuses.Add(new ActionStatus { actionType = data.actionType, isCompleted = data.isCompleted });
        }
    }

    //LoadPlayerPositionForScene ����
    public PlayerTr GetPlayerTrForScene(string sceneName)
    {
        return progressData.playerTr.FirstOrDefault(data => data.scene_Name == sceneName);
    }

    //ProgressData�� PlayerTr����Ʈ�� ���̸��� ��Ī���� �ʿ��� ��ġ�� ��������
    public void LoadPlayerPositionForScene(string sceneName)
    {
        PlayerTr playerData = GetPlayerTrForScene(sceneName);
        if (playerData != null)
        {     
            progressData.playerEulerAngles = playerData.rt;

            if (ProgressManager.Instance.progressData.hideInLocker)
            {
                progressData.playerPosition = playerData.tr + (Quaternion.Euler(playerData.rt) * Vector3.forward); // ���� ���� �� �÷��̾ ��Ŀ�� �����־��� ���¶�� ��Ŀ ������ ������
                ProgressManager.Instance.progressData.hideInLocker = false;
            }
            else
            {
                progressData.playerPosition = playerData.tr;
            }

            Debug.Log($"[{sceneName}] Load �÷��̾� ��ġ/ȸ�� ���� ������Ʈ: {playerData.tr}, {playerData.rt}");
        }
        else
        {
            Debug.LogWarning($"[{sceneName}] ����� �÷��̾� ��ġ ������ �����ϴ�.");
        }
    }
    // SavePlayerTr����
    public void UpdatePlayerTrForScene(string sceneName, Vector3 position, Vector3 rotation)
    {
        PlayerTr existingData = progressData.playerTr.FirstOrDefault(data => data.scene_Name == sceneName);

        if (existingData != null)
        {
            //���� ������ ������Ʈ
            existingData.rt = rotation;
            existingData.tr = position;

            if(CommonUIManager.instance.uiManager is SchoolUIManager)
            {
                if (IsActionCompleted(ActionType.GetLockerKey) && !IsActionCompleted(ActionType.GetOutOfLocker))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;
                    existingData.tr = schoolUIManager.playerRespawnPoints[0].position;
                    existingData.rt = schoolUIManager.playerRespawnPoints[0].rotation.eulerAngles;
                }
                else if (IsActionCompleted(ActionType.EnteredBackRoom) && !IsActionCompleted(ActionType.ClearBackRoom))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;
                    existingData.tr = schoolUIManager.playerRespawnPoints[1].position;
                    existingData.rt = schoolUIManager.playerRespawnPoints[1].rotation.eulerAngles;
                }
                else if (IsActionCompleted(ActionType.ClearLockerRoom) && !IsActionCompleted(ActionType.FinishFinalChase))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;
                    existingData.tr = schoolUIManager.playerRespawnPoints[2].position;
                    existingData.rt = schoolUIManager.playerRespawnPoints[2].rotation.eulerAngles;
                }
                else if (IsActionCompleted(ActionType.FinishFinalChase))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;
                    existingData.tr = schoolUIManager.playerRespawnPoints[3].position;
                    existingData.rt = schoolUIManager.playerRespawnPoints[3].rotation.eulerAngles;
                    schoolUIManager.activeObjs[23].GetComponent<Collider>().enabled = true; // ���������� ���� ��ư
                }
            }

            Debug.Log($"[{sceneName}] Update �÷��̾� ��ġ/ȸ�� ���� ������Ʈ: {position}, {rotation}");
        }
        
    }

    // ProgressData�� �� ���� �÷��̾� ��ġ�� ����
    public void SavePlayerTr()
    {
        if (PlayerController.instance != null)
        {
            string currentSceneName = progressData.scene;
            Vector3 currentPosition = PlayerController.instance.transform.position;
            Vector3 currentRotation = PlayerController.instance.transform.eulerAngles;

            UpdatePlayerTrForScene(currentSceneName, currentPosition, currentRotation);
        }
        
    }
}


[System.Serializable]
public class PlayerTr
{
    public string scene_Name;
    public Vector3 tr;
    public Vector3 rt;
}

[System.Serializable]
public class ActionStatus
{
    public ActionType actionType;
    public bool isCompleted;
}
