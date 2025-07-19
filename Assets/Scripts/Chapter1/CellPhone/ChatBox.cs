using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    public TextMeshProUGUI text; // Text UI
    VerticalLayoutGroup verticalLayoutGroup; // ��� ���� �� chatBox �� ��ġ�� �������� ���� ���̾ƿ� ������Ʈ�� ���� ������.

    // ��� ���� ���� ���� ������ Ÿ�̸�
    float timeout = 120f; // �ִ� 120�� ���
    float timer = 0f;

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    private void OnEnable()
    {
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
    }

    private void OnDisable()
    {
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
    }

    public void ChangeLanguage(int index)
    {
        StartCoroutine(ChangeLocalization(index));
    }

    IEnumerator ChangeLocalization(int index)
    {
        // Localization �ý����� ������ �ݿ��� �ð��� �ֱ� ���� �ʱ�ȭ�� ���� ������ ���
        yield return LocalizationSettings.InitializationOperation;

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

        // ���� �����ӿ��� ���̾ƿ��� ����
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
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
