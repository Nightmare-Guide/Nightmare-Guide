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

    // 언어 변경 무한 루프 방지용 타이머
    float timeout = 120f; // 최대 120초 대기
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
    public SaveStevenPhoneData phoneDatas;

    [Header("# Player")]
    [SerializeField] GameObject main_playerPrefab; // 메인씬
    [SerializeField] GameObject chap_playerPrefab; //챕터씬

    [Header("# 정보 저장 확인 테스트")]
    public TextMeshProUGUI defaultPhone;
    public TextMeshProUGUI updatePhone;
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

        FirstSet();

        // 첫 언어 설정
        StartCoroutine(ChangeLocalization(0));
        LanguageDropdown.value = 0;
    }

    private void Start()
    {

        SmartPhoneData();

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
        // 특정 씬이 로드되면 플레이어 생성 요청
        if (scene.name != "LoadingScene")
        {
            SpawnPlayer(scene.name);
        }
    }

    void SpawnPlayer(string sceneName)
    {
        if (main_playerPrefab != null&& chap_playerPrefab != null && ProgressManager.Instance != null && ProgressManager.Instance.progressData != null)
        {
           
            Vector3 spawnPosition = ProgressManager.Instance.progressData.playerPosition; // 저장된 플레이어 위치 사용
            GameObject player = null;
            if (sceneName.Contains("Main_Map")|| sceneName.Contains("House") || sceneName.Contains("Hospital"))
            {
                player = Instantiate(main_playerPrefab, spawnPosition, Quaternion.identity);
            }
            else if(sceneName.Contains("School"))
            {
                player = Instantiate(chap_playerPrefab, spawnPosition, Quaternion.identity);
            }
            if (player != null)
            {
                player.SetActive(true);
            }
          //  Debug.Log($"{sceneName} 씬에 플레이어 생성 성공! 위치: {player.transform.position}");
        }
        else
        {
            Debug.LogError("플레이어 생성 실패! 프리팹 또는 진행 데이터가 없음.");
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 구독 해제
    }

    private void OnApplicationQuit()
    {
        phoneDatas = new SaveStevenPhoneData { name = phoneInfos.name, hasPhone = phoneInfos.hasPhone, isUnlocked = phoneInfos.isUnlocked };
    }

    void FirstSet()
    {
        commonUICanvas.SetActive(true);
        optionUI.SetActive(false);

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
            MoveScene("Title Scene");
            optionUI.SetActive(false);
        }
    }

    public void FullScreenBtn()
    {
        // 전체 화면 코드 필요
        fullScreenCheckImg.SetActive(true);
        windowedCheckImg.SetActive(false);

        isFullScreen = true;
    }

    public void WindowedBtn()
    {
        // 창모드 화면 코드 필요
        fullScreenCheckImg.SetActive(false);
        windowedCheckImg.SetActive(true);

        isFullScreen = false;
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
        schoolUIManager?.RebuildVerticalLayout(schoolUIManager.textBoxLayouts);
        mainUIManager?.RebuildVerticalLayout(mainUIManager.textBoxLayouts);

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
        phoneInfos = new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false }; // cellPhoneObj 랑 cellPhoneUI 는 MainUIManager 에서 초기화
        //string path = Application.streamingAssetsPath + "/save.json";
        string path = Path.Combine(Application.streamingAssetsPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("파일이 존재합니다.");
            Debug.Log("폰 보유 여부 " + phoneInfos.hasPhone);
            phoneInfos.hasPhone = ProgressManager.Instance.progressData.stevenPhoneDatas.hasPhone;
            phoneInfos.isUnlocked = ProgressManager.Instance.progressData.stevenPhoneDatas.isUnlocked;
            Debug.Log("폰 보유 여부 " + phoneInfos.hasPhone);
        }
        else
        {
            Debug.Log("파일이 존재하지 않습니다.");
        }
    }

    // 씬 이동 함수
    public void MoveScene(string sceneName)
    {
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



    // 휴대폰 정보 Class
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