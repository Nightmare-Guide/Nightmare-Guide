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
    VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] bool changingLanguage;

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        changingLanguage = false;
    }

    void Start()
    {
        StartCoroutine(ChangeLocalization(0)); // ���� �⺻ ���� {���� : 0, �Ϻ��� : 1, �ѱ��� : 2}
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

        // Localization �ý����� ������ �ݿ��� �ð��� �ֱ� ���� �ʱ�ȭ�� ���� ������ ���
        yield return LocalizationSettings.InitializationOperation;
        yield return new WaitForSeconds(0.5f);

        // chatBox ����
        if (this.gameObject.name.Contains("Other"))
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft; // ��� ä���� ���ʿ� ��ġ
            Debug.Log("Other");
        }
        else
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperRight; // �� ä���� �����ʿ� ��ġ
        }

        // ������ ���̾ƿ� ������Ʈ
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        changingLanguage = false;
    }

    // chatBox ũ�� ���� �� ���� ����
    void AdjustText(int index)
    {
        // �ؽ�Ʈ�� ���� �������� ������
        string[] words = text.text.Split(' ');
        string formattedText = "";
        int wordCount = 0;

        // �ٹٲ� ó�� (3�� �ܾ�� ����)
        for (int i = 0; i < words.Length; i++)
        {
            if (wordCount != 3)
            {
                formattedText += words[i] + " ";
                wordCount++;
            }
            else
            {
                formattedText += words[i] + "\n";
                wordCount = 0;
            }
        }

        // �ؽ�Ʈ ����
        text.text = formattedText.Trim();

        if (this.gameObject.name.Contains("Other"))
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft; // ��� ä���� ���ʿ� ��ġ
            Debug.Log("Other");
        }
        else
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperRight; // �� ä���� �����ʿ� ��ġ
        }
    }
}
