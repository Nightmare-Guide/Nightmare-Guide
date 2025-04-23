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
            Destroy(gameObject); // �ߺ� ���� ����
            Destroy(commonUICanvas);
        }
        Debug.Log("CommonUIManager Awake");
        FirstSet();

        // ù ��� ����
        StartCoroutine(ChangeLocalization(0));
        LanguageDropdown.value = 0;
    }

    private void Start()
    {
        phoneInfos = new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false }; // cellPhoneObj �� cellPhoneUI �� MainUIManager ���� �ʱ�ȭ
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


    // �� �̵� �Լ�
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

    public class SavePhoneData
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
    }
}
