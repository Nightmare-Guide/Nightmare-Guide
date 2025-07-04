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


//게임 진행도 관리
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("진행도 데이터")]
    public ProgressData progressData;
    public ProgressData defaultData;

    public enum ActionType
    {
        // 진행 순서대로 입력 (진행 분기점 -> 연속되는 동작의 끝을 기준)
        StartNewDay,
        FirstMeetMichael,
        FirstMeetJames,
        TalkWithEthanMom,
        EnteredSchool,
        FirstMeetEthan,
        GetFlashlight,
        EnteredControlRoom,
        GetLockerKey,
        FirstMeetMonster,
        GetOutOfLocker,
        LeaveEthan,
        StartSchoolNightmare,
        SolvedPortalRoom,
        GetDavidCellPhone,
        SecondMeetMonster,
        EnteredBackRoom,
        EnteredEthanHouse,
        GetEthanCellPhone,
        EnteredLockerRoom,
        SolvedLockerRoom,
        StartFinalChase,
        FinishFinalChase,
        TalkWarmlyToEthan,
        BackToHospital,
        FinishFirstWork,
        FirstMeetAlex,
        CompleteFirstDay
    }

    void Awake()
    {
        // 싱글톤 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAllActions();
    }

    // Action 저장값 초기화 함수
    void InitAllActions()
    {
        defaultData.actionStatuses = new List<ActionStatus>();

        foreach (ActionType type in Enum.GetValues(typeof(ActionType)))
        {
            defaultData.actionStatuses.Add(new ActionStatus { actionType = type, isCompleted = false });
        }
    }

    // Action 이 실행되었는 지 안되었는 지 확인하는 함수
    public bool IsActionCompleted(ActionType type)
    {
        return ProgressManager.Instance.progressData.actionStatuses.Find(a => a.actionType == type).isCompleted;
    }

    public void CompletedAction(ActionType type)
    {
        ProgressManager.Instance.progressData.actionStatuses.Find(a => a.actionType == type).isCompleted = true;
    }

    /// <summary>
    /// PlayerPrefs에서 저장된 진행도 불러옴
    /// </summary>
    public void LoadProgress()
    {
        if (progressData == null)
        {
            Debug.LogError("ProgressData가 연결되지 않았습니다!");
            return;
        }
        // GameDataManager에서 진행도 데이터를 이미 progressData에 로드했으므로,
        // 여기서는 추가적인 로딩 로직이 필요하다면 구현합니다.
        // 예: 씬에 따른 초기 설정 등
        Debug.Log("ProgressData 로드 완료 (GameDataManager에서 데이터 로드)");
    }



    public void UpdateProgress(string newScene, string newProgress)
    {
        if (progressData != null)
        {
            progressData.scene = newScene;
            progressData.storyProgress = newProgress;
            // Debug.Log("진행도 업데이트 : " + newScene +" :: "+newProgress);
        }
    }

    /// <summary>
    /// 진행도 초기화 (새 게임 시작 등)a
    /// </summary>
    public void ResetProgress()
    {
        if (progressData != null && defaultData != null && CommonUIManager.instance != null)
        {
            progressData.newGame = defaultData.newGame;
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            DeepCopy(defaultData.actionStatuses); // 액션 상태
            progressData.playerPosition = defaultData.playerPosition;
            progressData.playerEulerAngles = defaultData.playerEulerAngles;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            DeepCopy(defaultData.phoneDatas); // 휴대폰 정보

            progressData.schoolInventoryDatas = new List<string>(defaultData.schoolInventoryDatas);

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;
            progressData.quest = "";

            // 타임라인
            DeepCopy(defaultData.timelineWatchedList);
            //플레이어 씬별 위치값
            DeepCopy(defaultData.playerTr);


            CommonUIManager.instance.ResetSoudVolume();
            /* Debug.Log("progressData.scene : " + progressData.scene);
             Debug.Log("progressData.storyProgress : " + progressData.storyProgress);
             if (progressData.phoneDatas != null && progressData.phoneDatas.Count > 0)
             {
                 Debug.Log("progressData.phoneDatas[0].hasPhone : " + progressData.phoneDatas[0].hasPhone);
                 Debug.Log("progressData.phoneDatas[0].isUnlocked : " + progressData.phoneDatas[0].isUnlocked);
             }*/
            //Debug.Log("진행도 초기화됨");
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

    //LoadPlayerPositionForScene 파츠
    public PlayerTr GetPlayerTrForScene(string sceneName)
    {
        return progressData.playerTr.FirstOrDefault(data => data.scene_Name == sceneName);
    }

    //ProgressData의 PlayerTr리스트의 씬이름과 매칭시켜 필요한 위치값 가져오기
    public void LoadPlayerPositionForScene(string sceneName)
    {
        PlayerTr playerData = GetPlayerTrForScene(sceneName);
        if (playerData != null)
        {     
            progressData.playerPosition = playerData.tr;
            progressData.playerEulerAngles = playerData.rt;
            Debug.Log($"[{sceneName}] Load 플레이어 위치/회전 정보 업데이트: {playerData.tr}, {playerData.rt}");
        }
        else
        {
            Debug.LogWarning($"[{sceneName}] 저장된 플레이어 위치 정보가 없습니다.");
        }
    }
    // SavePlayerTr파츠
    public void UpdatePlayerTrForScene(string sceneName, Vector3 position, Vector3 rotation)
    {
        PlayerTr existingData = progressData.playerTr.FirstOrDefault(data => data.scene_Name == sceneName);

        if (existingData != null)
        {
            //기존 데이터 업데이트
            existingData.tr = position;
            existingData.rt = rotation;
            Debug.Log($"[{sceneName}] Update 플레이어 위치/회전 정보 업데이트: {position}, {rotation}");
        }
        
    }

    // ProgressData의 각 씬별 플레이어 위치값 저장
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
