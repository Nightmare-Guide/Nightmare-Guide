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
    public bool isTalkingWithNPC = false; // npc 와 대화 중 게임 정지 후, 다시 게임 진행 시 플레이어 움직임 막는 용도

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

    // 언어 변경 무한 루프 방지용 타이머
    float timeout = 120f; // 최대 120초 대기
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

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(commonUICanvas);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 완료 이벤트 구독
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
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

        // 특정 씬이 로드되면 플레이어 생성 요청
        if (!scene.name.Equals("LoadingScene") && !ProgressManager.Instance.progressData.newGame && !scene.name.Equals("Title Scene"))
        {
            StartCoroutine(DelaySpawnPlayer());
        }
    }
    private IEnumerator DelaySpawnPlayer()
    {
        yield return null; // 1프레임 대기
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (PlayerController.instance != null && ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
        {
            if (!ProgressManager.Instance.progressData.newGame)
            {
                Debug.Log(ProgressManager.Instance.progressData.scene + "씬 위치값 : " + ProgressManager.Instance.progressData.playerPosition + "로테이션 : " + ProgressManager.Instance.progressData.playerEulerAngles);
                PlayerController.instance.Close_PlayerController();
                PlayerController.instance.transform.eulerAngles = ProgressManager.Instance.progressData.playerEulerAngles;
                PlayerController.instance.transform.position = ProgressManager.Instance.progressData.playerPosition;
                PlayerController.instance.Open_PlayerController();
            }

        }
        else
        {
            Debug.Log("플레이어 생성 실패! 프리팹 또는 진행 데이터가 없음.");
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 구독 해제
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

        // 첫 언어 설정
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

            // 언어
            if(ProgressManager.Instance.progressData.language == "en") { StartCoroutine(ChangeLocalization(0)); LanguageDropdown.value = 0; }
            else if(ProgressManager.Instance.progressData.language == "ja") { StartCoroutine(ChangeLocalization(1)); LanguageDropdown.value = 1; }
            else if (ProgressManager.Instance.progressData.language == "ko") { StartCoroutine(ChangeLocalization(2)); LanguageDropdown.value = 2; }

            // 사운드
            SetBGVolume(ProgressManager.Instance.progressData.bgVolume);
            SetEffectVolume(ProgressManager.Instance.progressData.effectVolume);

            // 전체화면/창모드
            if(ProgressManager.Instance.progressData.isFullScreen) { FullScreenBtn(); }
            else { WindowedBtn(); }
        }
        else
        {
            //Debug.Log("파일이 존재하지 않습니다.");
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

    // DropDown 에 들어가는 값 변경 함수
    public void ChangeLanguage()
    {
        int index = LanguageDropdown.value;

        // 언어 변경 중 or 이미 선택한 언어와 같을 경우엔 중복 실행 X
        if (changingLanguage || LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code)
            return;

        changingLanguage = true;

        // 언어 저장
        language = LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code;

        StartCoroutine(ChangeLocalization(index));

        if(ProgressManager.Instance != null) { ProgressManager.Instance.progressData.language = language;}
    }

    IEnumerator ChangeLocalization(int index)
    {
        // Localization 시스템이 변경을 반영할 시간을 주기 위해 초기화가 끝날 때까지 대기
        yield return LocalizationSettings.InitializationOperation;

        // 언어 변경
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

        // UI Manager 가 null 값이 아닐 경우 실행
        if (uiManager is SchoolUIManager schoolUIManager) { schoolUIManager.RebuildVerticalLayout(schoolUIManager.textBoxLayouts); }
        if (uiManager is MainUIManager mainUIManager) { mainUIManager.RebuildVerticalLayout(mainUIManager.textBoxLayouts); }

        changingLanguage = false;
    }

    // 현재 text(변수) 내용을 기반으로 현재 언어가 무엇인지 추정하는 함수
    string CheckCurrentLanguage() =>
    text.text switch
    {
        _ when text.text.Any(c => c is >= '\uAC00' and <= '\uD7A3') => "ko", // 한글
        _ when text.text.Any(c => (c is >= '\u3040' and <= '\u30FF') || (c is >= '\u4E00' and <= '\u9FFF')) => "ja", // 일본어 (히라가나, 가타카나, 한자)
        _ when text.text.Any(c => c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z') => "en", // 영어
        _ => "unKnown"
    };

    public void SmartPhoneData()
    {
        if (ProgressManager.Instance == null)
            return;

        stevenPhone.hasPhone = ProgressManager.Instance.progressData.phoneDatas[0].hasPhone;
        stevenPhone.isUnlocked = ProgressManager.Instance.progressData.phoneDatas[0].isUnlocked;
    }

    // 씬 이동 함수
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

        // 씬 저장
        interactionUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // 화면 중앙을 클릭하는 효과를 발생시킴 (Windows 전용)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        StartCoroutine(MoveSceneRoutine(sceneName));
    }

    public void PlayerSpawnPoint(string scene)//MoveScene이 실행되면서 새게임이 아닐때 플레이어 스폰 장소 설정 
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

    // 눈 감고/뜨는 애니메이션 쉐이더
    public void Blink(bool open)
    {
        StartCoroutine(BlinkRoutine(open));
    }

    IEnumerator BlinkRoutine(bool open)
    {
        blinkObj.SetActive(true);

        if (open)
        {
            // 눈 뜨기
            for (float t = 0; t < blinkDuration; t += Time.deltaTime)
            {
                float cutoff = Mathf.Lerp(-0.1f, 0.6f, t / blinkDuration);
                eyeMaterial.SetFloat("_Cutoff", cutoff);
                yield return null;
            }
        }
        else
        {
            // 눈 감기
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

    // Fog 적용 함수
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
            ProgressManager.Instance.progressData.fogName = settings.name; // 데이터 저장
        }

        // 어두울 때, 손전등 켜져있으면 fog Density 값 낮춤
        if (settings.name == "Nightmare" && Camera_Rt.instance.postProecessingBehaviour.gameObject.GetComponent<RayCast_Aim>().flashlight.activeInHierarchy)
        {
            RenderSettings.fogDensity = 0.25f;
        }
    }

    // 휴대폰 정보 Class
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