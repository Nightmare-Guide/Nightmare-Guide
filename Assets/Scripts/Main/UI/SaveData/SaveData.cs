using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string storyProgress ="0_0_0";
    public bool getSmartPhone = false;
    public Vector3 playerPosition;
    public int sanchi = 0;
}


public class SaveData : MonoBehaviour
{
    public ProgressData progressData; // 에디터에서 참조 연결

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    // 저장용 클래스에 복사해서 저장
    public void SaveGame()
    {
        GameData currentData = new GameData
        {
            storyProgress = progressData.storyProgress,
            getSmartPhone = progressData.getSmartPhone,
            playerPosition = progressData.playerPosition
        };

        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(FilePath, json);
        Debug.Log($"게임 저장됨: {FilePath}");
    }

    // JSON 불러와서 LocalData에 반영
    public void LoadGame()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            progressData.storyProgress = loadedData.storyProgress;
            progressData.getSmartPhone = loadedData.getSmartPhone;
            progressData.playerPosition = loadedData.playerPosition;

            Debug.Log("게임 불러오기 완료");
        }
        else
        {
            Debug.LogWarning("저장 파일이 없습니다.");
        }
    }
}


