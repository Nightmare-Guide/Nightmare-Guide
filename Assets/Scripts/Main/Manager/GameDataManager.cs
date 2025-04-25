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
    //[Header("�÷��̾� ��ġ")]
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

    public ProgressData progressData; // ScriptableObject ���� (�����Ϳ���)

    private string fileName = "save.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame(); // ���� ���� �� �ҷ�����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���� ������ ����
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
       // Debug.Log($"���� ���� �Ϸ�: {FilePath}");
    }

    // ���� ������ �ҷ�����
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

            //Debug.Log("���� �ҷ����� �Ϸ�");
        }
        else
        {
           // Debug.Log("����� ���� �����Ͱ� �����ϴ�. �� �������� �����մϴ�.");
        }
    }
    public void UpdatePlayerPosition(Vector3 newPos)
    {
        progressData.playerPosition = newPos;
        SaveGame();
    }

}
