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
    VerticalLayoutGroup verticalLayoutGroup; // 언어 변경 시 chatBox 의 위치가 망가져서 강제 레이아웃 업데이트를 위해 가져옴.
    [SerializeField] bool changingLanguage;

    // 언어 변경 무한 루프 방지용 타이머
    float timeout = 120f; // 최대 120초 대기
    float timer = 0f;

    private void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        changingLanguage = false;
    }

    void Start()
    {
        // 언어 변경
        // LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];

        // 다음 프레임에서 레이아웃을 갱신
        // LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            ChangeLanguage(0); // 영어
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            ChangeLanguage(2); // 한국어
        }
    }

    public void ChangeLanguage(int index)
    {
        // 언어 변경 중 or 이미 선택한 언어와 같을 경우엔 중복 실행 X
        if (changingLanguage || LocalizationSettings.AvailableLocales.Locales[index].Identifier.Code == LocalizationSettings.SelectedLocale.Identifier.Code)
            return;

        changingLanguage = true;

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

        // 즉시 레이아웃을 갱신
        // LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        // 다음 프레임에서 레이아웃을 갱신
        LayoutRebuilder.MarkLayoutForRebuild(verticalLayoutGroup.GetComponent<RectTransform>());

        changingLanguage = false;
    }

    string CheckCurrentLanguage() =>
        text.text switch
        {
            _ when text.text.Any(c => c is >= '\uAC00' and <= '\uD7A3') => "ko", // 한글
            _ when text.text.Any(c => (c is >= '\u3040' and <= '\u30FF') || (c is >= '\u4E00' and <= '\u9FFF')) => "ja", // 일본어 (히라가나, 가타카나, 한자)
            _ when text.text.Any(c => c is >= 'A' and <= 'Z' || c is >= 'a' and <= 'z') => "en", // 영어
            _ => "unKnown"
        };
}
