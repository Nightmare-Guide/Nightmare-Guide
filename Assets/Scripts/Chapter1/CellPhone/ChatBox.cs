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
    public RectTransform imageRect;     // 배경 이미지 RectTransform

    private float widthMultiplier_en = 9.6f; // 영어 글자 수에 따른 width 배율
    private float widthMultiplier_ko = 17.2f; // 한글 글자 수에 따른 width 배율
    private float baseHeight = 25f;       // 기본 높이
    private float lineHeight = 15f;       // 한 줄 추가될 때 증가하는 높이

    void Start()
    {
        StartCoroutine(WaitForFirstLocalizationAndAdjust(0)); // 영어 기본 세팅
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            StartCoroutine(WaitForFirstLocalizationAndAdjust(0)); // 영어
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            StartCoroutine(WaitForFirstLocalizationAndAdjust(2)); // 한국어
        }
    }

    IEnumerator WaitForFirstLocalizationAndAdjust(int index)
    {
        Debug.Log("현재 언어: " + LocalizationSettings.AvailableLocales.Locales[index]);

        bool isLocaleChanged = false;

        // 이벤트를 먼저 등록
        LocalizationSettings.SelectedLocaleChanged += (locale) =>
        {
            isLocaleChanged = true;
            Debug.Log("언어 변경 완료: " + locale);
        };

        // 언어 변경
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        // Localization 시스템이 변경을 반영할 시간을 주기 위해 초기화가 끝날 때까지 대기
        yield return LocalizationSettings.InitializationOperation;

        // 변경이 완료될 때까지 대기
        yield return new WaitUntil(() => isLocaleChanged);

        // 변경 완료 후 실행
        AdjustText(index);
    }

    void AdjustText(int index)
    {
        // 텍스트를 띄어쓰기 기준으로 나누기
        string[] words = text.text.Split(' ');
        string formattedText = "";
        int wordCount = 0;
        float maxLineLength = 0; // 가장 긴 줄의 문자 수

        // 특수기호 목록 (필요하면 추가 가능)
        char[] specialCharacters = { '.', ',', '!', '?', '-', '_', ':', ';', '"', '\'' };

        // 줄바꿈 처리 (4개 단어마다 개행)
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

        // 텍스트 적용
        text.text = formattedText.Trim();

        // 줄 개수 계산
        string[] lines = text.text.Split('\n');
        int lineCount = lines.Length;

        // 가장 긴 줄 글자 수 찾기
        foreach (string line in lines)
        {
            float actualLength = 0; // 실제 글자 수 계산
            int spaceAndSpecialCount = 0; // 띄어쓰기 및 특수문자 개수

            foreach (char c in line)
            {
                if (c == ' ' || specialCharacters.Contains(c)) // 띄어쓰기 or 특수기호
                {
                    spaceAndSpecialCount++;
                }
                else // 일반 글자
                {
                    actualLength++;
                }
            }

            // 띄어쓰기 & 특수기호 2개당 1글자로 처리
            actualLength += (float)spaceAndSpecialCount / 2;

            if (actualLength > maxLineLength)
            {
                maxLineLength = actualLength;
            }
        }

        // width = (최대 글자 수 * 각 언어별 설정 크기)
        float newWidth = 0;
        if (index == 2) { newWidth = maxLineLength * widthMultiplier_en; Debug.Log("English"); } // 영어
        else if(index == 0) { newWidth = maxLineLength * widthMultiplier_ko; Debug.Log("Korean"); } // 한국어

        // height = 기본값(25) + (줄 개수 - 1) * 15
        float newHeight = baseHeight + (lineCount - 1) * lineHeight;

        Debug.Log($"maxLineLength : {maxLineLength}, newWidth : {newWidth}, lineCount : {lineCount}, newHeight : {newHeight}, text : {text.text}");

        // 크기 조정
        imageRect.sizeDelta = new Vector2(newWidth, newHeight);
    }
}
