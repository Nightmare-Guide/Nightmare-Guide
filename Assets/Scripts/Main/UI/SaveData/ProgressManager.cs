using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static SchoolUIManager;


//���� ���൵ ����
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("���൵ ������")]
    public ProgressData progressData;
    public ProgressData defaultData;

    void Awake()
    {
        // �̱��� ó��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    /// <summary>
    /// PlayerPrefs���� ����� ���൵ �ҷ���
    /// </summary>
    public void LoadProgress()
    {
        if (progressData == null)
        {
            Debug.LogError("ProgressData�� ������� �ʾҽ��ϴ�!");
            return;
        }
        // GameDataManager���� ���൵ �����͸� �̹� progressData�� �ε������Ƿ�,
        // ���⼭�� �߰����� �ε� ������ �ʿ��ϴٸ� �����մϴ�.
        // ��: ���� ���� �ʱ� ���� ��
        Debug.Log("ProgressData �ε� �Ϸ� (GameDataManager���� ������ �ε�)");
    }


   
    public void UpdateProgress(string newScene, string newProgress)
    {
        if (progressData != null)
        {
            progressData.scene = newScene;
            progressData.storyProgress = newProgress;
            // Debug.Log("���൵ ������Ʈ : " + newScene +" :: "+newProgress);
        }
    }

    /// <summary>
    /// ���൵ �ʱ�ȭ (�� ���� ���� ��)a
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

            // Ÿ�Ӷ���
            InitTimeLine(defaultData.timelineWatchedList);



            CommonUIManager.instance.ResetSoudVolume();
           /* Debug.Log("progressData.scene : " + progressData.scene);
            Debug.Log("progressData.storyProgress : " + progressData.storyProgress);
            if (progressData.phoneDatas != null && progressData.phoneDatas.Count > 0)
            {
                Debug.Log("progressData.phoneDatas[0].hasPhone : " + progressData.phoneDatas[0].hasPhone);
                Debug.Log("progressData.phoneDatas[0].isUnlocked : " + progressData.phoneDatas[0].isUnlocked);
            }*/
            //Debug.Log("���൵ �ʱ�ȭ��");
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