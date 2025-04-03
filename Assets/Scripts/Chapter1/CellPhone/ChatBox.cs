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
        StartCoroutine(ChangeLocalization(0)); // 영어 기본 세팅 {영어 : 0, 일본어 : 1, 한국어 : 2}
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

        // Localization 시스템이 변경을 반영할 시간을 주기 위해 초기화가 끝날 때까지 대기
        yield return LocalizationSettings.InitializationOperation;
        yield return new WaitForSeconds(0.5f);

        // chatBox 정렬
        if (this.gameObject.name.Contains("Other"))
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft; // 상대 채팅은 왼쪽에 배치
            Debug.Log("Other");
        }
        else
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperRight; // 내 채팅은 오른쪽에 배치
        }

        // 강제로 레이아웃 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.GetComponent<RectTransform>());

        changingLanguage = false;
    }

    // chatBox 크기 설정 및 글자 정렬
    void AdjustText(int index)
    {
        // 텍스트를 띄어쓰기 기준으로 나누기
        string[] words = text.text.Split(' ');
        string formattedText = "";
        int wordCount = 0;

        // 줄바꿈 처리 (3개 단어마다 개행)
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

        // 텍스트 적용
        text.text = formattedText.Trim();

        if (this.gameObject.name.Contains("Other"))
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft; // 상대 채팅은 왼쪽에 배치
            Debug.Log("Other");
        }
        else
        {
            verticalLayoutGroup.childAlignment = TextAnchor.UpperRight; // 내 채팅은 오른쪽에 배치
        }
    }
}
