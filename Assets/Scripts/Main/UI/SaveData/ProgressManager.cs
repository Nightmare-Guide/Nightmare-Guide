using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static SchoolUIManager;


//게임 진행도 관리
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("진행도 데이터")]
    public ProgressData progressData;
    public ProgressData defaultData;

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
       
    }
    private void Start()
    {
        if(GameDataManager.instance != null)
        {
           // LoadProgress();
        }
        
    }
    /// <summary>
    /// PlayerPrefs에서 저장된 진행도 불러옴
    /// </summary>
    public void LoadProgress()
    {
        if (progressData == null)
        {
            //Debug.LogError("ProgressData가 연결되지 않았습니다!");
            return;
        }
        else//게임 데이터 매니저에서 진행도 불러오기
        {
            progressData.scene = GameDataManager.instance.progressData.scene;
            progressData.storyProgress = GameDataManager.instance.progressData.storyProgress;
            progressData.playerPosition = GameDataManager.instance.progressData.playerPosition;
            progressData.sanchi = GameDataManager.instance.progressData.sanchi;

            progressData.mainInventoryDatas = new List<string>(GameDataManager.instance.progressData.mainInventoryDatas);
            progressData.phoneDatas = new List<SavePhoneData>(GameDataManager.instance.progressData.phoneDatas);
            progressData.inventoryDatas = new List<string>(GameDataManager.instance.progressData.inventoryDatas);
            progressData.stevenPhoneDatas = GameDataManager.instance.progressData.stevenPhoneDatas;

            progressData.bgVolume = GameDataManager.instance.progressData.bgVolume;
            progressData.effectVolume = GameDataManager.instance.progressData.effectVolume;
            progressData.characterVolume = GameDataManager.instance.progressData.characterVolume;
            progressData.isFullScreen = GameDataManager.instance.progressData.isFullScreen;
            progressData.language = GameDataManager.instance.progressData.language;
        }
        

    }

    /// <summary>
    /// 현재 진행도를 저장함
    /// </summary>
    public void SaveProgress()
    {
        if (progressData != null)
        {
            GameDataManager.instance.progressData.scene = progressData.scene;
            GameDataManager.instance.progressData.storyProgress = progressData.storyProgress;
            GameDataManager.instance.progressData.playerPosition = progressData.playerPosition;
            GameDataManager.instance.progressData.sanchi= progressData.sanchi;

            GameDataManager.instance.progressData.mainInventoryDatas= progressData.mainInventoryDatas;
            GameDataManager.instance.progressData.phoneDatas= progressData.phoneDatas;
            GameDataManager.instance.progressData.inventoryDatas= progressData.inventoryDatas;
            GameDataManager.instance.progressData.stevenPhoneDatas= progressData.stevenPhoneDatas;

            GameDataManager.instance.progressData.bgVolume= progressData.bgVolume;
            GameDataManager.instance.progressData.effectVolume= progressData.effectVolume;
            GameDataManager.instance.progressData.characterVolume= progressData.characterVolume;
            GameDataManager.instance.progressData.isFullScreen= progressData.isFullScreen;
            GameDataManager.instance.progressData.language= progressData.language;
            GameDataManager.instance.SaveGame();
        }
        else
        {
         //   Debug.LogError("진행도를 저장할 수 없습니다. ProgressData가 없음");
        }
    }

    /// <summary>
    /// 진행도 수동 갱신
    /// </summary>
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
        if (progressData != null && defaultData !=null)
        {
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            progressData.playerPosition = defaultData.playerPosition;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            progressData.phoneDatas = new List<SavePhoneData>(defaultData.phoneDatas);
            progressData.inventoryDatas = new List<string>(defaultData.inventoryDatas);
            progressData.stevenPhoneDatas = defaultData.stevenPhoneDatas;

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;
            //Debug.Log("진행도 초기화됨");
        }
    }
}
