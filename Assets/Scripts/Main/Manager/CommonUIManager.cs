using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SchoolUIManager;
using System.IO;
using System.Runtime.InteropServices;
using UnityStandardAssets.Characters.FirstPerson;

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance { get; private set; }

    [SerializeField] GameObject commonUICanvas;
    public GameObject optionUI;
    public GameObject interactionUI;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI conversationNameText;
    public TextMeshProUGUI conversationText;

    [Header("# Screen")]
    public Slider bgVolumeSlider;
    public Slider effectVolumeSlider;
    public Slider characterVolumeSlider;

    [Header("# Screen")]
    public GameObject fullScreenCheckImg;
    public GameObject windowedCheckImg;

    [Header("# Language")]
    [SerializeField] private TMP_Dropdown LanguageDropdown;
    [SerializeField] bool changingLanguage = false;
    public TextMeshProUGUI text;

    // ��� ���� ���� ���� ������ Ÿ�̸�
    float timeout = 120f; // �ִ� 120�� ���
    float timer = 0f;

    // CellPhone
    public CharacterPhoneInfo phoneInfos;

    [Header("# Blink")]
    public GameObject blinkObj;
    public Material eyeMaterial;
    public float blinkDuration = 1.0f;

    [Header("# UIManagers")]
    public TitleUIManager TitleUIManager;
    public MainUIManager mainUIManager;
    public SchoolUIManager schoolUIManager;

    [Header("# SaveData")]
    float bgVolume;
    [SerializeField] float effectVolume;
    float characterVolume;
    bool isFullScreen;
    string language;
    public SaveStevenPhoneData stevenPhoneData;

    public GameObject main_playerPrefab;
    public GameObject chap_playerPrefab;

    [Header("# ���� ���� Ȯ�� �׽�Ʈ")]
    public TextMeshProUGUI defaultPhone;
    public TextMeshProUGUI updatePhone;

    // Windows�� ���콺 �Է��� �ùķ��̼��ϴ� API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // ���콺 ���� ��ư ����
    private const int MOUSEEVENTF_LEFTUP = 0x04; // ���콺 ���� ��ư ��

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(commonUICanvas);
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �Ϸ� �̺�Ʈ ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
            Destroy(commonUICanvas);
        }

        FirstSet();

        // ù ��� ����
        StartCoroutine(ChangeLocalization(0));
        LanguageDropdown.value = 0;
    }

 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            MoveScene("Title Scene");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            MoveScene("DayHouse");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            MoveScene("Main_Map");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            MoveScene("DayHospital");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            MoveScene("School_Scene");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            MoveScene("NightHospital");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            MoveScene("Main_Map_Night");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            MoveScene("Main_Map_Night");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            MoveScene("NightHouse");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ư�� ���� �ε�Ǹ� �÷��̾� ���� ��û
        if (scene.name != "LoadingScene")
        {
            //SpawnPlayer(scene.name);
        }
    }

    void SpawnPlayer(string sceneName)
    {
        if (PlayerController.instance != null && ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
        {
            

            Vector3 spawnPosition = ProgressManager.Instance.progressData.playerPosition; // ����� �÷��̾� ��ġ ���
            PlayerController.instance.transform.position = spawnPosition;
        }
        else
        {
            Debug.Log("�÷��̾� ���� ����! ������ �Ǵ� ���� �����Ͱ� ����.");
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ���� ����
    }

    private void OnApplicationQuit()
    {
        stevenPhoneData = new SaveStevenPhoneData { name = phoneInfos.name, hasPhone = phoneInfos.hasPhone, isUnlocked = phoneInfos.isUnlocked };
        if (GameDataManager.instance != null && PlayerController.instance!=null) {
            ProgressManager.Instance.progressData.playerPosition = PlayerController.instance.transform.position;
            ProgressManager.Instance.progressData.newGame = false;
            GameDataManager.instance.SaveGame(); }
    }

    void FirstSet()
    {
        commonUICanvas.SetActive(true);
        optionUI.SetActive(false);
        phoneInfos = new CharacterPhoneInfo();

        FullScreenBtn();
    }

    public void BackToTitleBtn()
    {
        if (TitleUIManager != null)
        {
            optionUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            if (GameDataManager.instance != null && PlayerController.instance!=null && ProgressManager.Instance!=null) {
                Vector3 playerTr = PlayerController.instance.transform.position;
                ProgressManager.Instance.progressData.playerPosition = playerTr;
                ProgressManager.Instance.progressData.newGame = false;
                GameDataManager.instance.SaveGame();
            }
            MoveScene("Title Scene");
            optionUI.SetActive(false);
        }
    }

    public void FullScreenBtn()
    {
        // ��ü ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(true);
        windowedCheckImg.SetActive(false);

        isFullScreen = true;
    }

    public void WindowedBtn()
    {
        // â��� ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(false);
        windowedCheckImg.SetActive(true);

        isFullScreen = false;
    }

    // DropDown �� ���� �� ���� �Լ�
    public void ChangeLanguage()
    {
        int index = LanguageDropdown.value;

        // ��� ���� �� or �̹� ������ ���� ���� ��쿣 �ߺ� ���� X
        if (changingLanguage || LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code)
            return;

        changingLanguage = true;

        // ��� ����
        language = LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code;

        StartCoroutine(ChangeLocalization(index));
    }

    IEnumerator ChangeLocalization(int index)
    {
        // Localization �ý����� ������ �ݿ��� �ð��� �ֱ� ���� �ʱ�ȭ�� ���� ������ ���
        yield return LocalizationSettings.InitializationOperation;

        // ��� ����
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        while (CheckCurrentLanguage() != LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code)
        {
            Debug.Log("false");
            yield return null;

            if (timer > timeout)
            {
                Debug.LogWarning("Localization change timeout!");
                break;
            }
        }

        // UI Manager �� null ���� �ƴ� ��� ����
        schoolUIManager?.RebuildVerticalLayout(schoolUIManager.textBoxLayouts);
        mainUIManager?.RebuildVerticalLayout(mainUIManager.textBoxLayouts);

        changingLanguage = false;
    }

    // ���� text(����) ������ ������� ���� �� �������� �����ϴ� �Լ�
    string CheckCurrentLanguage() =>
    text.text switch
    {
        _ when text.text.Any(c => c is >= '\uAC00' and <= '\uD7A3') => "ko", // �ѱ�
        _ when text.text.Any(c => (c is >= '\u3040' and <= '\u30FF') || (c is >= '\u4E00' and <= '\u9FFF')) => "ja", // �Ϻ��� (���󰡳�, ��Ÿī��, ����)
        _ when text.text.Any(c => c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z') => "en", // ����
        _ => "unKnown"
    };

    public void SmartPhoneData()
    {
        phoneInfos.name = "Steven";
        phoneInfos.hasPhone = false;
        phoneInfos.isUnlocked = false;

        if (ProgressManager.Instance!=null)
        {

            phoneInfos.hasPhone = ProgressManager.Instance.progressData.phoneDatas[0].hasPhone;
            phoneInfos.isUnlocked = ProgressManager.Instance.progressData.phoneDatas[0].isUnlocked;
        }
        else
        {
            Debug.Log("������ �������� �ʽ��ϴ�.");
        }
    }

    // �� �̵� �Լ�
    public void MoveScene(string sceneName)
    {
        if (ProgressManager.Instance != null && !sceneName.Equals("Title Scene"))
            { ProgressManager.Instance.progressData.scene = sceneName; }// �� ����
        interactionUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // ȭ�� �߾��� Ŭ���ϴ� ȿ���� �߻���Ŵ (Windows ����)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        StartCoroutine(MoveSceneRoutine(sceneName));
    }

    IEnumerator MoveSceneRoutine(string sceneName)
    {
        Blink(false);

        yield return new WaitForSeconds(blinkDuration);
        // yield return null;

        LoadingSceneManager.LoadScene(sceneName);
    }
    public void StartNarrationScene()
    {
        StartCoroutine(NarrationSceneRoutine());
    }

    IEnumerator NarrationSceneRoutine()
    {
        Blink(false);

        yield return new WaitForSeconds(blinkDuration);
        // yield return null;

        SceneManager.LoadScene("Narration");
    }

    // �� ����/�ߴ� �ִϸ��̼� ���̴�
    public void Blink(bool open)
    {
        StartCoroutine(BlinkRoutine(open));
    }

    IEnumerator BlinkRoutine(bool open)
    {
        blinkObj.SetActive(true);

        if (open)
        {
            // �� �߱�
            for (float t = 0; t < blinkDuration; t += Time.deltaTime)
            {
                float cutoff = Mathf.Lerp(-0.1f, 0.6f, t / blinkDuration);
                eyeMaterial.SetFloat("_Cutoff", cutoff);
                yield return null;
            }
        }
        else
        {
            // �� ����
            for (float t = 0; t < blinkDuration; t += Time.deltaTime)
            {
                float cutoff = Mathf.Lerp(0.6f, -0.1f, t / blinkDuration);
                eyeMaterial.SetFloat("_Cutoff", cutoff);
                yield return null;
            }
        }

        yield return new WaitForSeconds(blinkDuration);

        blinkObj.SetActive(false);
    }

    public void SetBGVolume(float value)
    {
        // bgAudioSource.volume = value;
        bgVolume = value;
    }

    public void SetEffectVolume(float value)
    {
        // effectAudioSource.volume = value;
        effectVolume = value;
    }

    public void SetCharacterVolume(float value)
    {
        // characterAudioSource.volume = value;
        characterVolume = value;
    }



    // �޴��� ���� Class
    public class StevenPhoneInfo
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
        public GameObject cellPhoneObj;
        public GameObject cellPhoneUI;
    }

    public class SaveStevenPhoneData
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
    }
}