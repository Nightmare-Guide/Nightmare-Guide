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
    VerticalLayoutGroup verticalLayoutGroup; // 언어 변경 시 chatBox 의 위치가 망가져서 강제 레이아웃 업데이트를 위해 가져옴.

    // 언어 변경 무한 루프 방지용 타이머
    float timeout = 120f; // 최대 120초 대기
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
        // Localization 시스템이 변경을 반영할 시간을 주기 위해 초기화가 끝날 때까지 대기
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

        // 다음 프레임에서 레이아웃을 갱신
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
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
}
