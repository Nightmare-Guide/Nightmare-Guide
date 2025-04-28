using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static CommonUIManager;
using static SchoolUIManager;

[System.Serializable]
public class GameData
{
    public string scene = "0_1";
    public string storyProgress = "0_0_0";
    public bool getSmartPhone = false;
    public Vector3 playerPosition = new Vector3(-550, -67, 278);
    public int sanchi = 0;

    public List<string> mainInventoryDatas;
    public List<SavePhoneData> phoneDatas;
    public List<string> inventoryDatas;
    public SaveStevenPhoneData stevenPhoneDatas;

    public float bgVolume;
    public float effectVolume;
    public float characterVolume;
    public bool isFullScreen;
    public string language;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public ProgressData progressData; // ScriptableObject 연결 (에디터에서)
    public ProgressData newGame;      // 새로 시작하기용 초기 값

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame(); // 게임 시작 시 불러오기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        GameData saveData = new GameData
        {
            scene = progressData.scene,
            storyProgress = progressData.storyProgress,
            getSmartPhone = progressData.getSmartPhone,
            playerPosition = progressData.playerPosition,
            sanchi = progressData.sanchi,

            mainInventoryDatas = progressData.mainInventoryDatas,
            phoneDatas = progressData.phoneDatas,
            inventoryDatas = progressData.inventoryDatas,
            stevenPhoneDatas = progressData.stevenPhoneDatas,

            bgVolume = progressData.bgVolume,
            effectVolume = progressData.effectVolume,
            characterVolume = progressData.characterVolume,
            isFullScreen = progressData.isFullScreen,
            language = progressData.language
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(FilePath, json);
        // Debug.Log($"게임 저장 완료: {FilePath}");
    }

    public void LoadGame()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            GameData loadData = JsonUtility.FromJson<GameData>(json);

            progressData.scene = loadData.scene;
            progressData.storyProgress = loadData.storyProgress;
            progressData.getSmartPhone = loadData.getSmartPhone;
            progressData.playerPosition = loadData.playerPosition;
            progressData.sanchi = loadData.sanchi;

            progressData.mainInventoryDatas = loadData.mainInventoryDatas;
            progressData.phoneDatas = loadData.phoneDatas;
            progressData.inventoryDatas = loadData.inventoryDatas;
            progressData.stevenPhoneDatas = loadData.stevenPhoneDatas;

            progressData.bgVolume = loadData.bgVolume;
            progressData.effectVolume = loadData.effectVolume;
            progressData.characterVolume = loadData.characterVolume;
            progressData.isFullScreen = loadData.isFullScreen;
            progressData.language = loadData.language;

            // Debug.Log("게임 불러오기 완료");
        }
        else
        {
            // Debug.Log("저장된 게임 데이터가 없습니다.");
        }
    }

    public void UpdatePlayerPosition(Vector3 newPos)
    {
        progressData.playerPosition = newPos;
        SaveGame();
    }

    public bool StartNewGame()
    {
        progressData.scene = newGame.scene;
        progressData.storyProgress = newGame.storyProgress;
        progressData.getSmartPhone = newGame.getSmartPhone;
        progressData.playerPosition = newGame.playerPosition;
        progressData.sanchi = newGame.sanchi;

        progressData.mainInventoryDatas = new List<string>(newGame.mainInventoryDatas);
        progressData.phoneDatas = new List<SavePhoneData>(newGame.phoneDatas);
        progressData.inventoryDatas = new List<string>(newGame.inventoryDatas);
        progressData.stevenPhoneDatas = newGame.stevenPhoneDatas;

        progressData.bgVolume = newGame.bgVolume;
        progressData.effectVolume = newGame.effectVolume;
        progressData.characterVolume = newGame.characterVolume;
        progressData.isFullScreen = newGame.isFullScreen;
        progressData.language = newGame.language;

        ProgressManager.Instance.ResetProgress();

        //Debug.Log("새 게임이 시작되었습니다. 초기화된 데이터를 저장했습니다.");
        return true;
    }

}
