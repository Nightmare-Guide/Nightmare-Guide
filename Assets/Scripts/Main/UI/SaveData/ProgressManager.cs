using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    private void Start()
    {
        if(GameDataManager.instance != null)
        {
           // LoadProgress();
        }
        
    }
    /// <summary>
    /// PlayerPrefs���� ����� ���൵ �ҷ���
    /// </summary>
    public void LoadProgress()
    {
        if (progressData == null)
        {
            //Debug.LogError("ProgressData�� ������� �ʾҽ��ϴ�!");
            return;
        }
        else//���� ������ �Ŵ������� ���൵ �ҷ�����
        {
            progressData.scene = GameDataManager.instance.progressData.scene;
            progressData.storyProgress = GameDataManager.instance.progressData.storyProgress;
            progressData.playerPosition = GameDataManager.instance.progressData.playerPosition;
            progressData.sanchi = GameDataManager.instance.progressData.sanchi;

            progressData.mainInventoryDatas = new List<string>(GameDataManager.instance.progressData.mainInventoryDatas);
            progressData.phoneDatas = new List<SavePhoneData>(GameDataManager.instance.progressData.phoneDatas);
            progressData.inventoryDatas = new List<string>(GameDataManager.instance.progressData.inventoryDatas);
            progressData.stevenPhoneDatas = GameDataManager.instance.progressData.stevenPhoneDatas;

            progressData.bgVolume = GameDataManager.instance.progressData.bgVolume;
            progressData.effectVolume = GameDataManager.instance.progressData.effectVolume;
            progressData.characterVolume = GameDataManager.instance.progressData.characterVolume;
            progressData.isFullScreen = GameDataManager.instance.progressData.isFullScreen;
            progressData.language = GameDataManager.instance.progressData.language;
        }
        

    }

    /// <summary>
    /// ���� ���൵�� ������
    /// </summary>
    public void SaveProgress()
    {
        if (progressData != null)
        {
            GameDataManager.instance.progressData.scene = progressData.scene;
            GameDataManager.instance.progressData.storyProgress = progressData.storyProgress;
            GameDataManager.instance.progressData.playerPosition = progressData.playerPosition;
            GameDataManager.instance.progressData.sanchi= progressData.sanchi;

            GameDataManager.instance.progressData.mainInventoryDatas= progressData.mainInventoryDatas;
            GameDataManager.instance.progressData.phoneDatas= progressData.phoneDatas;
            GameDataManager.instance.progressData.inventoryDatas= progressData.inventoryDatas;
            GameDataManager.instance.progressData.stevenPhoneDatas= progressData.stevenPhoneDatas;

            GameDataManager.instance.progressData.bgVolume= progressData.bgVolume;
            GameDataManager.instance.progressData.effectVolume= progressData.effectVolume;
            GameDataManager.instance.progressData.characterVolume= progressData.characterVolume;
            GameDataManager.instance.progressData.isFullScreen= progressData.isFullScreen;
            GameDataManager.instance.progressData.language= progressData.language;
            GameDataManager.instance.SaveGame();
        }
        else
        {
         //   Debug.LogError("���൵�� ������ �� �����ϴ�. ProgressData�� ����");
        }
    }

    /// <summary>
    /// ���൵ ���� ����
    /// </summary>
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
        if (progressData != null && defaultData !=null)
        {
            progressData.scene = defaultData.scene;
            progressData.storyProgress = defaultData.storyProgress;
            progressData.playerPosition = defaultData.playerPosition;
            progressData.sanchi = defaultData.sanchi;

            progressData.mainInventoryDatas = new List<string>(defaultData.mainInventoryDatas);
            progressData.phoneDatas = new List<SavePhoneData>(defaultData.phoneDatas);
            progressData.inventoryDatas = new List<string>(defaultData.inventoryDatas);
            progressData.stevenPhoneDatas = defaultData.stevenPhoneDatas;

            progressData.bgVolume = defaultData.bgVolume;
            progressData.effectVolume = defaultData.effectVolume;
            progressData.characterVolume = defaultData.characterVolume;
            progressData.isFullScreen = defaultData.isFullScreen;
            progressData.language = defaultData.language;
            //Debug.Log("���൵ �ʱ�ȭ��");
        }
    }
}
