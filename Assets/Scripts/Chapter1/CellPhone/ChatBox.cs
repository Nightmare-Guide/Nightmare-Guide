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

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        changingLanguage = false;
    }

    void Start()
    {
        // ��� ����
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        // ������ ���̾ƿ� ������Ʈ
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());
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

        // Localization �ý����� ������ �ݿ��� �ð��� �ֱ� ���� �ʱ�ȭ�� ���� ������ ���
        yield return LocalizationSettings.InitializationOperation;
        yield return new WaitForSeconds(0.5f);

        // ������ ���̾ƿ� ������Ʈ
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        changingLanguage = false;
    }
}
