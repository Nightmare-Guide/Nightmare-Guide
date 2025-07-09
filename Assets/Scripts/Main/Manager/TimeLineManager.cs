using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimeLineManager : MonoBehaviour
{
    public static TimeLineManager instance { get; private set; }

    public List<PlayableAsset> playableAssets;
    public PlayableDirector director;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    private void Start()
    {
        // defaultData �� timelineWatched �ʱ�ȭ
        ProgressManager.Instance.defaultData.timelineWatchedList = new List<TimelineEntry>();

        if (playableAssets.Count == 0)
            return;

        foreach (PlayableAsset playableAsset in playableAssets)
        {
            ProgressManager.Instance.defaultData.timelineWatchedList.Add(new TimelineEntry { key = playableAsset.name, value = false});
        }
    }
    public void PauseTimeline()
    {
        if (director != null && director.state == PlayState.Playing)
        {
            director.Pause();
        }
    }
    public void ResumeTimeline()
    {
        if (director != null && director.state == PlayState.Paused)
        {
            director.Play();
        }
    }

}

[System.Serializable]
public class TimelineEntry
{
    public string key;
    public bool value;
}
