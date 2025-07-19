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
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;
using static ProgressManager;
using UnityEngine.Localization.SmartFormat.Utilities;

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance { get; private set; }

    [Header("# UI Objs")]
    [SerializeField] GameObject commonUICanvas;
    public GameObject optionUI;
    public GameObject interactionUI;
    public bool isTalkingWithNPC = false; // npc �� ��ȭ �� ���� ���� ��, �ٽ� ���� ���� �� �÷��̾� ������ ���� �뵵

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

    [Header("# Fog")]

    // ��� ���� ���� ���� ������ Ÿ�̸�
    float timeout = 120f; // �ִ� 120�� ���
    float timer = 0f;

    // CellPhone
    public PhoneInfos stevenPhone;

    [Header("# Blink")]
    public GameObject blinkObj;
    public Material eyeMaterial;
    public float blinkDuration = 1.0f;

    [Header("# UIManagers")]
    public UIUtility uiManager;

    [Header("# SaveData")]
    float bgVolume;
    float effectVolume;
    float characterVolume;
    bool isFullScreen;
    string language;

    [Header("# Fog")]
    public List<FogSettings> fogSettings = new List<FogSettings>();

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
    }

    private void Start()
    {
        FirstSet();
        GetProgressData();
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
        if (ProgressManager.Instance == null)
            return;

        // Ư�� ���� �ε�Ǹ� �÷��̾� ���� ��û
        if (!scene.name.Equals("LoadingScene") && !ProgressManager.Instance.progressData.newGame && !scene.name.Equals("Title Scene"))
        {
            StartCoroutine(DelaySpawnPlayer());
        }
    }
    private IEnumerator DelaySpawnPlayer()
    {
        yield return null; // 1������ ���
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (PlayerController.instance != null && ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
        {
            if (!ProgressManager.Instance.progressData.newGame)
            {
                Debug.Log(ProgressManager.Instance.progressData.scene + "�� ��ġ�� : " + ProgressManager.Instance.progressData.playerPosition + "�����̼� : " + ProgressManager.Instance.progressData.playerEulerAngles);
                PlayerController.instance.Close_PlayerController();
                PlayerController.instance.transform.eulerAngles = ProgressManager.Instance.progressData.playerEulerAngles;
                PlayerController.instance.transform.position = ProgressManager.Instance.progressData.playerPosition;
                PlayerController.instance.Open_PlayerController();
            }

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
        if (GameDataManager.instance != null && PlayerController.instance != null)
        {
            ProgressManager.Instance.progressData.playerPosition = PlayerController.instance.transform.position;
            ProgressManager.Instance.progressData.newGame = false;
            GameDataManager.instance.SaveGame();
        }
    }

    void FirstSet()
    {
        commonUICanvas.SetActive(true);
        optionUI.SetActive(false);
        stevenPhone = new PhoneInfos();

        // ù ��� ����
        StartCoroutine(ChangeLocalization(0));
        LanguageDropdown.value = 0;

        fogSettings.Add(new FogSettings { name = "Nightmare", fogDensity = 0.55f });
        fogSettings.Add(new FogSettings { name = "Warm", fogColor = new Color(1f, 0.525f, 0f), fogDensity = 0.002f });
    }

    void GetProgressData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // ���
            if(ProgressManager.Instance.progressData.language == "en") { StartCoroutine(ChangeLocalization(0)); LanguageDropdown.value = 0; }
            else if(ProgressManager.Instance.progressData.language == "ja") { StartCoroutine(ChangeLocalization(1)); LanguageDropdown.value = 1; }
            else if (ProgressManager.Instance.progressData.language == "ko") { StartCoroutine(ChangeLocalization(2)); LanguageDropdown.value = 2; }

            // ����
            SetBGVolume(ProgressManager.Instance.progressData.bgVolume);
            SetEffectVolume(ProgressManager.Instance.progressData.effectVolume);

            // ��üȭ��/â���
            if(ProgressManager.Instance.progressData.isFullScreen) { FullScreenBtn(); }
            else { WindowedBtn(); }
        }
        else
        {
            //Debug.Log("������ �������� �ʽ��ϴ�.");
            FullScreenBtn();
        }
    }

    public void BackToTitleBtn()
    {
        if (uiManager is TitleUIManager)
        {
            optionUI.SetActive(false);
        }
        else
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName.Contains("Title"))
            {
                return;
            }
            Time.timeScale = 1;
            if (GameDataManager.instance != null && PlayerController.instance != null && ProgressManager.Instance != null)
            {
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
        fullScreenCheckImg.SetActive(true);
        windowedCheckImg.SetActive(false);

        isFullScreen = true;

        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);

        if(ProgressManager.Instance != null) { ProgressManager.Instance.progressData.isFullScreen = true; }
    }

    public void WindowedBtn()
    {
        fullScreenCheckImg.SetActive(false);
        windowedCheckImg.SetActive(true);

        isFullScreen = false;

        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);

        if (ProgressManager.Instance != null) { ProgressManager.Instance.progressData.isFullScreen = false; }
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

        if(ProgressManager.Instance != null) { ProgressManager.Instance.progressData.language = language;}
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
        if (uiManager is SchoolUIManager schoolUIManager) { schoolUIManager.RebuildVerticalLayout(schoolUIManager.textBoxLayouts); }
        if (uiManager is MainUIManager mainUIManager) { mainUIManager.RebuildVerticalLayout(mainUIManager.textBoxLayouts); }

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
        if (ProgressManager.Instance == null)
            return;

        stevenPhone.hasPhone = ProgressManager.Instance.progressData.phoneDatas[0].hasPhone;
        stevenPhone.isUnlocked = ProgressManager.Instance.progressData.phoneDatas[0].isUnlocked;
    }

    // �� �̵� �Լ�
    public void MoveScene(string sceneName)
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.bgmSource.Stop();
            SoundManager.instance.sfxSource.Stop();
        }

        if (PlayerController.instance != null) { PlayerController.instance.Close_PlayerController(); }
        if (Camera_Rt.instance != null) { Camera_Rt.instance.Close_Camera(); }

        if (ProgressManager.Instance != null && !sceneName.Equals("Title Scene"))
        {
            PlayerSpawnPoint(sceneName);
            ProgressManager.Instance.progressData.scene = sceneName;
        }

        // �� ����
        interactionUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // ȭ�� �߾��� Ŭ���ϴ� ȿ���� �߻���Ŵ (Windows ����)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        StartCoroutine(MoveSceneRoutine(sceneName));
    }

    public void PlayerSpawnPoint(string scene)//MoveScene�� ����Ǹ鼭 �������� �ƴҶ� �÷��̾� ���� ��� ���� 
    {
        if (ProgressManager.Instance == null)
            return;

        ProgressManager.Instance.SavePlayerTr();
        ProgressManager.Instance.LoadPlayerPositionForScene(scene);

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
        bgVolume = value;
        bgVolumeSlider.value = value;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.bgVolume = value;
        }
    }

    public void SetEffectVolume(float value)
    {
        effectVolume = value;
        effectVolumeSlider.value = value;
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.effectVolume = value;
        }

    }

    public void SetCharacterVolume(float value)
    {
        //characterAudioSource.volume = value;
        characterVolume = value;
        characterVolumeSlider.value = value;
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.characterVolume = value;
        }
    }

    public void ResetSoudVolume()
    {
        if (ProgressManager.Instance != null)
        {
            SetBGVolume(ProgressManager.Instance.defaultData.bgVolume);
            SetEffectVolume(ProgressManager.Instance.defaultData.effectVolume);
            SetCharacterVolume(ProgressManager.Instance.defaultData.characterVolume);
        }
    }
    public void LoadSoudVolume()
    {
        if (ProgressManager.Instance != null)
        {
            SetBGVolume(ProgressManager.Instance.progressData.bgVolume);
            SetEffectVolume(ProgressManager.Instance.progressData.effectVolume);
            SetCharacterVolume(ProgressManager.Instance.progressData.characterVolume);
        }
    }

    // Fog ���� �Լ�
    public void ApplyFog(FogSettings settings)
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = settings.fogColor;
        RenderSettings.fogMode = settings.fogMode;

        switch (settings.fogMode)
        {
            case FogMode.Linear:
                RenderSettings.fogStartDistance = settings.fogStartDistance;
                RenderSettings.fogEndDistance = settings.fogEndDistance;
                break;
            case FogMode.Exponential:
            case FogMode.ExponentialSquared:
                RenderSettings.fogDensity = settings.fogDensity;
                break;
        }

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.fogName = settings.name; // ������ ����
        }

        // ��ο� ��, ������ ���������� fog Density �� ����
        if (settings.name == "Nightmare" && Camera_Rt.instance.postProecessingBehaviour.gameObject.GetComponent<RayCast_Aim>().flashlight.activeInHierarchy)
        {
            RenderSettings.fogDensity = 0.25f;
        }
    }

    // �޴��� ���� Class
    public class PhoneInfos
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
        public GameObject cellPhoneObj;
        public GameObject cellPhoneUI;
    }

    [System.Serializable]
    public class SavePhoneData
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
    }

    public class Item
    {
        public string name;
        public Sprite itemImg;
        public GameObject uiObj;
        public SchoolUIManager schoolUIManager;
    }

    // Fog
    public class FogSettings
    {
        public string name;
        public FogMode fogMode = FogMode.Exponential;
        public Color fogColor = Color.black;
        public float fogDensity = 0.3f;
        public float fogStartDistance = 0f;
        public float fogEndDistance = 100f;
    }
}