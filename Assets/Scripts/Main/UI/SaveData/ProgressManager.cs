using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static CommonUIManager;
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
        if (progressData != null && defaultData != null && CommonUIManager.instance != null)
        {
            progressData.newGame = defaultData.newGame;
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            progressData.playerPosition = defaultData.playerPosition;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            DeepCopy(defaultData.phoneDatas); // 휴대폰 정보

            progressData.schoolInventoryDatas = new List<string>(defaultData.schoolInventoryDatas);

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;

            // 타임라인
            DeepCopy(defaultData.timelineWatchedList);



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

    public void DeepCopy(List<SavePhoneData> list)
    {
        progressData.phoneDatas = new List<SavePhoneData>();

        foreach (var data in list)
        {
            progressData.phoneDatas.Add(new SavePhoneData { name = data.name, hasPhone = false, isUnlocked = false }) ;
        }
    }
}