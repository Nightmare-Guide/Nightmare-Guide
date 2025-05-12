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

        // 초기화 -> 데이터 로드 or 초기화 필요
        timelineWatched = new Dictionary<string, bool>();

        if (playableAssets.Count == 0)
            return;

        foreach (PlayableAsset playableAsset in playableAssets)
        {
            timelineWatched.Add(playableAsset.name, true);
        }
    }
}
