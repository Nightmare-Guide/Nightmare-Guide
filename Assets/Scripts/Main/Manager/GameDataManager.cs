using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static CommonUIManager;
using static SchoolUIManager;

[System.Serializable]
public class GameData
{
    
    public bool newGame = true;
    public string scene;
    public string storyProgress = "0_0_0";
    public List<ActionStatus> actionStatuses;

    public Vector3 playerPosition = new Vector3(-550, -67, 278);
    public Vector3 playerEulerAngles;

    public int sanchi = 0;
    public List<string> mainInventoryDatas;
    public List<SavePhoneData> phoneDatas;
    public List<string> schoolInventoryDatas;

    public float bgVolume = 50.0f;
    public float effectVolume = 50.0f;
    public float characterVolume = 50.0f;
    public bool isFullScreen = true;
    public string language = "en";
    public string quest = "";

    public List<TimelineEntry> timelineWatchedList;
    public List<PlayerTr> playerTr;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    // public ProgressData newGame; // 더 이상 필요 없음

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGame(); // 게임 시작 시 불러오기
    }
    //테스트용 저장 기능
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartNewGame();
        }

    }

    public bool HasSaveData()
    {
        return File.Exists(FilePath);
    }

    public void SaveGame()
    {
        if (ProgressManager.Instance == null || ProgressManager.Instance.progressData == null)
        {
            Debug.LogError("ProgressManager 인스턴스 또는 progressData가 없습니다.");
            return;
        }

        GameData saveData = new GameData
        {
            newGame = ProgressManager.Instance.progressData.newGame,
            scene = ProgressManager.Instance.progressData.scene,
            storyProgress = ProgressManager.Instance.progressData.storyProgress,
            actionStatuses = ProgressManager.Instance.progressData.actionStatuses,
            playerPosition = ProgressManager.Instance.progressData.playerPosition,
            playerEulerAngles= ProgressManager.Instance.progressData.playerEulerAngles,
            sanchi = ProgressManager.Instance.progressData.sanchi,

            mainInventoryDatas = ProgressManager.Instance.progressData.mainInventoryDatas,
            phoneDatas = ProgressManager.Instance.progressData.phoneDatas,
            schoolInventoryDatas = ProgressManager.Instance.progressData.schoolInventoryDatas,

            bgVolume = ProgressManager.Instance.progressData.bgVolume,
            effectVolume = ProgressManager.Instance.progressData.effectVolume,
            characterVolume = ProgressManager.Instance.progressData.characterVolume,
            isFullScreen = ProgressManager.Instance.progressData.isFullScreen,
            language = ProgressManager.Instance.progressData.language,
            quest = ProgressManager.Instance.progressData.quest,

            timelineWatchedList = ProgressManager.Instance.progressData.timelineWatchedList,
            playerTr = ProgressManager.Instance.progressData.playerTr
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(FilePath, json);
        Debug.Log($"게임 저장 완료: {FilePath}");
    }

    public void LoadGame() // 게임 실행 시 불러오는 설정값들.
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            GameData loadData = JsonUtility.FromJson<GameData>(json);

            if (ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
            {
                ProgressManager.Instance.progressData.newGame = loadData.newGame;
                ProgressManager.Instance.progressData.scene = loadData.scene;
                ProgressManager.Instance.progressData.storyProgress = loadData.storyProgress;
                ProgressManager.Instance.progressData.actionStatuses = loadData.actionStatuses;
                ProgressManager.Instance.progressData.playerPosition = loadData.playerPosition;
                ProgressManager.Instance.progressData.playerEulerAngles = loadData.playerEulerAngles;
                ProgressManager.Instance.progressData.sanchi = loadData.sanchi;

                ProgressManager.Instance.progressData.mainInventoryDatas = loadData.mainInventoryDatas;
                ProgressManager.Instance.progressData.phoneDatas = loadData.phoneDatas;
                ProgressManager.Instance.progressData.schoolInventoryDatas = loadData.schoolInventoryDatas;

                ProgressManager.Instance.progressData.bgVolume = loadData.bgVolume;
                ProgressManager.Instance.progressData.effectVolume = loadData.effectVolume;
                ProgressManager.Instance.progressData.characterVolume = loadData.characterVolume;
                ProgressManager.Instance.progressData.isFullScreen = loadData.isFullScreen;
                ProgressManager.Instance.progressData.language = loadData.language;
                ProgressManager.Instance.progressData.language = loadData.quest;

                if (loadData.timelineWatchedList.Count != 0)
                {
                    ProgressManager.Instance.DeepCopy(loadData.timelineWatchedList);
                }
                if (loadData.playerTr.Count != 0)
                {
                    ProgressManager.Instance.DeepCopy(loadData.playerTr);
                }
                ProgressManager.Instance.LoadProgress();
                CommonUIManager.instance.LoadSoudVolume();
              /*  Debug.Log("progressData.scene : " + ProgressManager.Instance.progressData.scene);
                Debug.Log("progressData.storyProgress : " + ProgressManager.Instance.progressData.storyProgress);
                if (ProgressManager.Instance.progressData.phoneDatas != null && ProgressManager.Instance.progressData.phoneDatas.Count > 0)
                {
                    Debug.Log("progressData.phoneDatas[0].hasPhone : " + ProgressManager.Instance.progressData.phoneDatas[0].hasPhone);
                    Debug.Log("progressData.phoneDatas[0].isUnlocked : " + ProgressManager.Instance.progressData.phoneDatas[0].isUnlocked);
                }*/
            }
            else
            {
                Debug.LogError("ProgressManager 인스턴스 또는 progressData가 없습니다.");
            }
        }
        else
        {
            Debug.Log("저장된 게임 데이터가 없습니다.");
            ProgressManager.Instance.InitTimeline();
        }
    }

    public void UpdatePlayerPosition(Vector3 newPos)
    {
        if (ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
        {
            ProgressManager.Instance.progressData.playerPosition = newPos;
            SaveGame();
        }
    }

    public bool StartNewGame()
    {
        if (ProgressManager.Instance != null && ProgressManager.Instance.defaultData != null && ProgressManager.Instance.progressData != null)
        {
            ProgressManager.Instance.ResetProgress();
            SaveGame();
            return true;
        }
        return false;
    }
}