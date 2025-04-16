using UnityEngine;



public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("진행도 데이터")]
    public ProgressData progressData;

    private const string ProgressKey = "GameProgress";
    private string defaultProgress = "0_0_0";

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

        LoadProgress();
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

        if (PlayerPrefs.HasKey(ProgressKey))
        {
            string saved = PlayerPrefs.GetString(ProgressKey);
            progressData.storyProgress = saved;
            Debug.Log($"진행도 로드 완료: {saved}");
        }
        else
        {
            progressData.storyProgress = defaultProgress;
            Debug.Log("저장된 진행도가 없어 기본값으로 설정됨");
        }
    }

    /// <summary>
    /// 현재 진행도를 저장함
    /// </summary>
    public void SaveProgress()
    {
        if (progressData != null)
        {
            PlayerPrefs.SetString(ProgressKey, progressData.storyProgress);
            PlayerPrefs.Save();
            Debug.Log($"진행도 저장됨: {progressData.storyProgress}");
        }
        else
        {
            Debug.LogError("진행도를 저장할 수 없습니다. ProgressData가 없음");
        }
    }

    /// <summary>
    /// 진행도 수동 갱신
    /// </summary>
    public void UpdateProgress(string newProgress)
    {
        if (progressData != null)
        {
            progressData.storyProgress = newProgress;
            Debug.Log($"진행도 업데이트됨: {newProgress}");
        }
    }

    /// <summary>
    /// 진행도 초기화 (새 게임 시작 등)
    /// </summary>
    public void ResetProgress()
    {
        if (progressData != null)
        {
            progressData.storyProgress = defaultProgress;
            PlayerPrefs.DeleteKey(ProgressKey);
            Debug.Log("진행도 초기화됨");
        }
    }
}
