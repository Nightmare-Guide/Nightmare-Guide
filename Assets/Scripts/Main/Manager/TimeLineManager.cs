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
            Destroy(gameObject); // �ߺ� ���� ����
        }

        // �ʱ�ȭ -> ������ �ε� or �ʱ�ȭ �ʿ�
        timelineWatched = new Dictionary<string, bool>();

        if (playableAssets.Count == 0)
            return;

        foreach (PlayableAsset playableAsset in playableAssets)
        {
            timelineWatched.Add(playableAsset.name, true);
        }
    }
}
