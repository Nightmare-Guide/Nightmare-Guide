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
    public ProgressData progressData; // �����Ϳ��� ���� ����

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    // ����� Ŭ������ �����ؼ� ����
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
        Debug.Log($"���� �����: {FilePath}");
    }

    // JSON �ҷ��ͼ� LocalData�� �ݿ�
    public void LoadGame()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(json);

            progressData.storyProgress = loadedData.storyProgress;
            progressData.getSmartPhone = loadedData.getSmartPhone;
            progressData.playerPosition = loadedData.playerPosition;

            Debug.Log("���� �ҷ����� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("���� ������ �����ϴ�.");
        }
    }
}


