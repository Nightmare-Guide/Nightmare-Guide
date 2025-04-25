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
    public string storyProgress = "0_0_0";
    public bool getSmartPhone = false;
    public Vector3 playerPosition;
    //[Header("플레이어 산치")]
    public int sanchi = 0;
   // [Header("MainUIManager")]
    public List<String> mainInventoryDatas;
   // [Header("SchoolUIManager")]
    public List<SavePhoneData> phoneDatas;
    public List<String> inventoryDatas;
  //  [Header("CommonUIManager")]
    public SaveStevenPhoneData stevenPhoneDatas;
    float bgVolume;
    float effectVolume;
    float characterVolume;
    bool isFullScreen;
    string language;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public ProgressData progressData; // ScriptableObject 연결 (에디터에서)

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        // 싱글턴 패턴 설정
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

    // 게임 데이터 저장
    public void SaveGame()
    {
        GameData saveData = new GameData
        {
            storyProgress = progressData.storyProgress,
            getSmartPhone = progressData.getSmartPhone,
            playerPosition = progressData.playerPosition,
            sanchi = progressData.sanchi
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(FilePath, json);
       // Debug.Log($"게임 저장 완료: {FilePath}");
    }

    // 게임 데이터 불러오기
    public void LoadGame()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            GameData loadData = JsonUtility.FromJson<GameData>(json);

            progressData.storyProgress = loadData.storyProgress;
            progressData.getSmartPhone = loadData.getSmartPhone;
            progressData.playerPosition = loadData.playerPosition;
            progressData.sanchi = loadData.sanchi;

            //Debug.Log("게임 불러오기 완료");
        }
        else
        {
           // Debug.Log("저장된 게임 데이터가 없습니다. 새 게임으로 시작합니다.");
        }
    }
    public void UpdatePlayerPosition(Vector3 newPos)
    {
        progressData.playerPosition = newPos;
        SaveGame();
    }

}
