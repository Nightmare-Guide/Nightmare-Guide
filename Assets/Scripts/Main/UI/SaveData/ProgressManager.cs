using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
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
        if (progressData != null && defaultData != null && CommonUIManager.instance!=null)
        {
            progressData.newGame = defaultData.newGame;
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            progressData.playerPosition = defaultData.playerPosition;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            // progressData.phoneDatas = new List<SavePhoneData>(defaultData.phoneDatas);
            progressData.phoneDatas = new List<SavePhoneData>();

            for (int i = 0; i < defaultData.phoneDatas.Count; i++)
            {
                progressData.phoneDatas.Add(new SavePhoneData());
                progressData.phoneDatas[0].name = defaultData.phoneDatas[0].name;
                progressData.phoneDatas[0].hasPhone = defaultData.phoneDatas[0].hasPhone;
                progressData.phoneDatas[0].isUnlocked = defaultData.phoneDatas[0].isUnlocked;
            }
            progressData.inventoryDatas = new List<string>(defaultData.inventoryDatas);
            progressData.stevenPhoneDatas = defaultData.stevenPhoneDatas;

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;

            // 타임라인
            InitTimeLine(defaultData.timelineWatchedList);



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
        TimeLineManager.instance.timelineWatched = new Dictionary<string, bool>();
        ProgressManager.Instance.progressData.timelineWatchedList = new List<TimelineEntry>();

        foreach (var playableAsset in TimeLineManager.instance.playableAssets)
        {
            TimeLineManager.instance.timelineWatched.Add(playableAsset.name, false);
            ProgressManager.Instance.progressData.timelineWatchedList.Add(new TimelineEntry { key = playableAsset.name, value = false });
        }
    }

    public void InitTimeLine(List<TimelineEntry> list)
    {
        ProgressManager.Instance.progressData.timelineWatchedList = new List<TimelineEntry>();
        TimeLineManager.instance.timelineWatched = new Dictionary<string, bool>();

        foreach (var data in list)
        {
            ProgressManager.Instance.progressData.timelineWatchedList.Add(new TimelineEntry { key = data.key, value = data.value });
            TimeLineManager.instance.timelineWatched.Add(data.key, data.value);
        }
    }
}