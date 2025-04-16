using UnityEngine;



public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("���൵ ������")]
    public ProgressData progressData;

    private const string ProgressKey = "GameProgress";
    private string defaultProgress = "0_0_0";

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

        LoadProgress();
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

        if (PlayerPrefs.HasKey(ProgressKey))
        {
            string saved = PlayerPrefs.GetString(ProgressKey);
            progressData.storyProgress = saved;
            Debug.Log($"���൵ �ε� �Ϸ�: {saved}");
        }
        else
        {
            progressData.storyProgress = defaultProgress;
            Debug.Log("����� ���൵�� ���� �⺻������ ������");
        }
    }

    /// <summary>
    /// ���� ���൵�� ������
    /// </summary>
    public void SaveProgress()
    {
        if (progressData != null)
        {
            PlayerPrefs.SetString(ProgressKey, progressData.storyProgress);
            PlayerPrefs.Save();
            Debug.Log($"���൵ �����: {progressData.storyProgress}");
        }
        else
        {
            Debug.LogError("���൵�� ������ �� �����ϴ�. ProgressData�� ����");
        }
    }

    /// <summary>
    /// ���൵ ���� ����
    /// </summary>
    public void UpdateProgress(string newProgress)
    {
        if (progressData != null)
        {
            progressData.storyProgress = newProgress;
            Debug.Log($"���൵ ������Ʈ��: {newProgress}");
        }
    }

    /// <summary>
    /// ���൵ �ʱ�ȭ (�� ���� ���� ��)
    /// </summary>
    public void ResetProgress()
    {
        if (progressData != null)
        {
            progressData.storyProgress = defaultProgress;
            PlayerPrefs.DeleteKey(ProgressKey);
            Debug.Log("���൵ �ʱ�ȭ��");
        }
    }
}
