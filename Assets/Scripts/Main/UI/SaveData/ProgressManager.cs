using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static CommonUIManager;
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
        if (progressData != null && defaultData != null && CommonUIManager.instance != null)
        {
            progressData.newGame = defaultData.newGame;
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            progressData.playerPosition = defaultData.playerPosition;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            DeepCopy(defaultData.phoneDatas); // �޴��� ����

            progressData.schoolInventoryDatas = new List<string>(defaultData.schoolInventoryDatas);

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;

            // Ÿ�Ӷ���
            DeepCopy(defaultData.timelineWatchedList);



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