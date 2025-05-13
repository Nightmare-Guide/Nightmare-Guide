using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimeLineManager : MonoBehaviour
{
    public static TimeLineManager instance { get; private set; }

    public List<PlayableAsset> playableAssets;
    public Dictionary<string, bool> timelineWatched;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    private void Start()
    {
        // defaultData 의 timelineWatched 초기화
        ProgressManager.Instance.defaultData.timelineWatchedList = new List<TimelineEntry>();

        if (playableAssets.Count == 0)
            return;

        foreach (PlayableAsset playableAsset in playableAssets)
        {
            ProgressManager.Instance.defaultData.timelineWatchedList.Add(new TimelineEntry { key = playableAsset.name, value = false});
        }
    }

}

[System.Serializable]
public class TimelineEntry
{
    public string key;
    public bool value;
}
