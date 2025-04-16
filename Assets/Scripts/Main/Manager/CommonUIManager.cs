using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using static System.Net.Mime.MediaTypeNames;
using static UnityEditor.Progress;

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance { get; private set; }

    [SerializeField] GameObject commonUICanvas;
    public GameObject optionUI;
    public GameObject fullScreenCheckImg;
    public GameObject windowedCheckImg;

    [SerializeField] private TMP_Dropdown LanguageDropdown;
    [SerializeField] bool changingLanguage = false;
    public TextMeshProUGUI text;

    // ��� ���� ���� ���� ������ Ÿ�̸�
    float timeout = 120f; // �ִ� 120�� ���
    float timer = 0f;

    [Header("# UIManagers")]
    public TitleUIManager TitleUIManager;
    public MainUIManager mainUIManager;
    public SchoolUIManager schoolUIManager;

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
            // Title ������ �̵�
        }
    }

    public void FullScreenBtn()
    {
        // ��ü ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(true);
        windowedCheckImg.SetActive(false);
    }

    public void WindowedBtn()
    {
        // â��� ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(false);
        windowedCheckImg.SetActive(true);
    }

    // DropDown �� ���� �� ���� �Լ�
    public void ChangeLanguage()
    {
        int index = LanguageDropdown.value;

        // ��� ���� �� or �̹� ������ ���� ���� ��쿣 �ߺ� ���� X
        if (changingLanguage || LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code)
            return;

        changingLanguage = true;

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
}
