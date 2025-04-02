using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChatBox : MonoBehaviour
{
    public TextMeshProUGUI text; // Text UI
    public RectTransform imageRect;     // ��� �̹��� RectTransform

    private float widthMultiplier_en = 9.6f; // ���� ���� ���� ���� width ����
    private float widthMultiplier_ko = 17.2f; // �ѱ� ���� ���� ���� width ����
    private float baseHeight = 25f;       // �⺻ ����
    private float lineHeight = 15f;       // �� �� �߰��� �� �����ϴ� ����

    void Start()
    {
        StartCoroutine(WaitForFirstLocalizationAndAdjust(0)); // ���� �⺻ ����
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(WaitForFirstLocalizationAndAdjust(0)); // ����
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            StartCoroutine(WaitForFirstLocalizationAndAdjust(2)); // �ѱ���
        }
    }

    IEnumerator WaitForFirstLocalizationAndAdjust(int index)
    {
        Debug.Log("���� ���: " + LocalizationSettings.AvailableLocales.Locales[index]);

        bool isLocaleChanged = false;

        // �̺�Ʈ�� ���� ���
        LocalizationSettings.SelectedLocaleChanged += (locale) =>
        {
            isLocaleChanged = true;
            Debug.Log("��� ���� �Ϸ�: " + locale);
        };

        // ��� ����
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        // Localization �ý����� ������ �ݿ��� �ð��� �ֱ� ���� �ʱ�ȭ�� ���� ������ ���
        yield return LocalizationSettings.InitializationOperation;

        // ������ �Ϸ�� ������ ���
        yield return new WaitUntil(() => isLocaleChanged);

        // ���� �Ϸ� �� ����
        AdjustText(index);
    }

    void AdjustText(int index)
    {
        // �ؽ�Ʈ�� ���� �������� ������
        string[] words = text.text.Split(' ');
        string formattedText = "";
        int wordCount = 0;
        float maxLineLength = 0; // ���� �� ���� ���� ��

        // Ư����ȣ ��� (�ʿ��ϸ� �߰� ����)
        char[] specialCharacters = { '.', ',', '!', '?', '-', '_', ':', ';', '"', '\'' };

        // �ٹٲ� ó�� (4�� �ܾ�� ����)
        for (int i = 0; i < words.Length; i++)
        {
            formattedText += words[i] + " ";
            wordCount++;

            if (wordCount == 4)
            {
                formattedText += "\n";
                wordCount = 0;
            }
        }

        // �ؽ�Ʈ ����
        text.text = formattedText.Trim();

        // �� ���� ���
        string[] lines = text.text.Split('\n');
        int lineCount = lines.Length;

        // ���� �� �� ���� �� ã��
        foreach (string line in lines)
        {
            float actualLength = 0; // ���� ���� �� ���
            int spaceAndSpecialCount = 0; // ���� �� Ư������ ����

            foreach (char c in line)
            {
                if (c == ' ' || specialCharacters.Contains(c)) // ���� or Ư����ȣ
                {
                    spaceAndSpecialCount++;
                }
                else // �Ϲ� ����
                {
                    actualLength++;
                }
            }

            // ���� & Ư����ȣ 2���� 1���ڷ� ó��
            actualLength += (float)spaceAndSpecialCount / 2;

            if (actualLength > maxLineLength)
            {
                maxLineLength = actualLength;
            }
        }

        // width = (�ִ� ���� �� * �� �� ���� ũ��)
        float newWidth = 0;
        if (index == 2) { newWidth = maxLineLength * widthMultiplier_en; Debug.Log("English"); } // ����
        else if(index == 0) { newWidth = maxLineLength * widthMultiplier_ko; Debug.Log("Korean"); } // �ѱ���

        // height = �⺻��(25) + (�� ���� - 1) * 15
        float newHeight = baseHeight + (lineCount - 1) * lineHeight;

        Debug.Log($"maxLineLength : {maxLineLength}, newWidth : {newWidth}, lineCount : {lineCount}, newHeight : {newHeight}, text : {text.text}");

        // ũ�� ����
        imageRect.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
