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

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance { get; private set; }

    [SerializeField] GameObject commonUICanvas;
    public GameObject optionUI;
    public GameObject interactionUI;

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
    float effectVolume;
    float characterVolume;
    bool isFullScreen;
    string language;
    public SavePhoneData phoneDatas;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(commonUICanvas);
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
            Destroy(commonUICanvas);
        }
        Debug.Log("CommonUIManager Awake");
        FirstSet();

        // 첫 언어 설정
        StartCoroutine(ChangeLocalization(0));
        LanguageDropdown.value = 0;
    }

    private void Start()
    {
        phoneInfos = new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false }; // cellPhoneObj 랑 cellPhoneUI 는 MainUIManager 에서 초기화
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            MoveScene("Title Scene");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            MoveScene("Main_Map");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            MoveScene("School_Scene");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            MoveScene("Main_Map_Night");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            MoveScene("UI");
        }
    }

    private void OnDisable()
    {
        phoneDatas = new SavePhoneData { name = phoneInfos.name, hasPhone = phoneInfos.hasPhone, isUnlocked = phoneInfos.isUnlocked };
    }

    private void OnApplicationQuit()
    {
        phoneDatas = new SavePhoneData { name = phoneInfos.name, hasPhone = phoneInfos.hasPhone, isUnlocked = phoneInfos.isUnlocked };
    }

    void FirstSet()
    {
        commonUICanvas.SetActive(true);
        optionUI.SetActive(false);

        FullScreenBtn();
    }

    public void BackToTitleBtn()
    {
        if(TitleUIManager != null)
        {
            optionUI.SetActive(false);
        }
        else
        {
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

        Debug.Log("Start LoadScene()");
        LoadingSceneManager.LoadScene(sceneName);
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

    public class SavePhoneData
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
    }
}
