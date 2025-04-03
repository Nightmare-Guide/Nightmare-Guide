using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    public TextMeshProUGUI text; // Text UI
    VerticalLayoutGroup verticalLayoutGroup; // ��� ���� �� chatBox �� ��ġ�� �������� ���� ���̾ƿ� ������Ʈ�� ���� ������.
    [SerializeField] bool changingLanguage;

    // ��� ���� ���� ���� ������ Ÿ�̸�
    float timeout = 120f; // �ִ� 120�� ���
    float timer = 0f;

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        changingLanguage = false;
    }

    void Start()
    {
        // ��� ����
        // LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        // ���� �����ӿ��� ���̾ƿ��� ����
        // LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            ChangeLanguage(0); // ����
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            ChangeLanguage(2); // �ѱ���
        }
    }

    public void ChangeLanguage(int index)
    {
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

        // ��� ���̾ƿ��� ����
        // LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        // ���� �����ӿ��� ���̾ƿ��� ����
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());

        changingLanguage = false;
    }

    string CheckCurrentLanguage() =>
        text.text switch
        {
            _ when text.text.Any(c => c is >= '\uAC00' and <= '\uD7A3') => "ko", // �ѱ�
            _ when text.text.Any(c => (c is >= '\u3040' and <= '\u30FF') || (c is >= '\u4E00' and <= '\u9FFF')) => "ja", // �Ϻ��� (���󰡳�, ��Ÿī��, ����)
            _ when text.text.Any(c => c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z') => "en", // ����
            _ => "unKnown"
        };
}
