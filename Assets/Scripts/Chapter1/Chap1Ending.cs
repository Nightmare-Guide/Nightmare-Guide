using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Text Mesh Pro를 사용하기 위해 추가

public class Chap1Ending : MonoBehaviour
{
    // UI 요소들을 연결할 변수들 (SerializeField로 인스펙터 노출)
    [SerializeField] private Image backgroundImage; // 1번 이미지 (어두워질 배경 이미지)
    [SerializeField] private RectTransform lintTop; // 상단 선 (LintTop)
    [SerializeField] private RectTransform lintBottom; // 하단 선 (LintBottom)

    [SerializeField] private TMP_Text chapterClearText; // "Chapter Clear" 텍스트
    [SerializeField] private TMP_Text returningText; // "Returning to the title screen shortly..." 텍스트
    [SerializeField] private TMP_Text countdownText; // 숫자 카운트다운 텍스트 (5)

    // 텍스트들을 가지고 있는 부모 오브젝트 (이 오브젝트를 활성화/비활성화)
    [SerializeField] private GameObject textParentPanel;

    // 애니메이션 시간 설정
    [SerializeField] private float backgroundFadeDuration = 3f; // 배경이 어두워지는 시간 (3초)
    [SerializeField] private float lintSlideDuration = 2f;      // 선이 슬라이드 되는 시간 (2초)
    [SerializeField] private float textFadeInDuration = 1f;     // 텍스트 페이드인 시간

    // 씬 이동에 사용할 씬 이름
    [SerializeField] private string titleSceneName = "Title Scene"; // "Title Scene"으로 변경 (띄어쓰기 주의)
    [SerializeField] private float sceneMoveDelayAfterCountdown = 2f; // 카운트다운 1초 후 씬 이동 딜레이 (2초)

    private Color initialTextColorAlphaZero; // 텍스트의 초기 투명 상태 저장을 위해
    private Color initialTextColorFullAlpha; // 텍스트의 초기 불투명 상태 (페이드인 목표)

    private bool isEndingSequenceActive = false; // 엔딩 시퀀스 중복 실행 방지 플래그

    void Awake()
    {
        // 1. BG 초기 투명도 0인 상태
        if (backgroundImage != null)
        {
            Color tempColor = backgroundImage.color;
            tempColor.a = 0f;
            backgroundImage.color = tempColor;
        }
        else
        {
            Debug.LogError("Awake: backgroundImage is not assigned in the Inspector!");
        }

        // 텍스트들의 초기 색상 저장 (TMP_Text는 initialTextColorFullAlpha 설정 시 필요)
        if (chapterClearText != null)
        {
            initialTextColorFullAlpha = chapterClearText.color; // 현재 색상(RGB) 가져오기
            initialTextColorFullAlpha.a = 1f; // 알파 값을 1 (완전 불투명)로 설정

            initialTextColorAlphaZero = new Color(initialTextColorFullAlpha.r, initialTextColorFullAlpha.g, initialTextColorFullAlpha.b, 0f);
        }
        else
        {
            Debug.LogError("Awake: chapterClearText is not assigned in the Inspector!");
        }

        // 모든 텍스트를 초기 투명 상태로 설정
        SetTextAlpha(chapterClearText, 0f);
        SetTextAlpha(returningText, 0f);
        SetTextAlpha(countdownText, 0f);

        if (countdownText != null)
        {
            countdownText.text = "5"; // 초기 카운트다운 숫자 설정
        }

        // 텍스트 부모 오브젝트를 비활성화 상태로 시작
        if (textParentPanel != null)
        {
            textParentPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Awake: textParentPanel is not assigned in the Inspector!");
        }

        // Lint 초기 위치 설정 (Inspector에서 설정하는 것이 가장 안정적이지만, 코드에서도 명시)
        if (lintTop != null)
        {
            lintTop.anchorMin = new Vector2(0f, lintTop.anchorMin.y);
            lintTop.anchorMax = new Vector2(1f, lintTop.anchorMax.y);
            lintTop.offsetMin = new Vector2(2000f, lintTop.offsetMin.y);
            lintTop.offsetMax = new Vector2(-2000f, lintTop.offsetMax.y);
        }
        else
        {
            Debug.LogError("Awake: lintTop is not assigned in the Inspector!");
        }
        if (lintBottom != null)
        {
            lintBottom.anchorMin = new Vector2(0f, lintBottom.anchorMin.y);
            lintBottom.anchorMax = new Vector2(1f, lintBottom.anchorMax.y);
            lintBottom.offsetMin = new Vector2(2000f, lintBottom.offsetMin.y);
            lintBottom.offsetMax = new Vector2(-2000f, lintBottom.offsetMax.y);
        }
        else
        {
            Debug.LogError("Awake: lintBottom is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // 1. 게임 중 Ctrl + F7번을 누르면 EndingAction 메소드가 실행
       /* if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F7))
        {
            if (!isEndingSequenceActive) // 엔딩 시퀀스가 실행 중이 아닐 때만 시작
            {
                Debug.Log("Ctrl + F7 pressed. Starting Ending Sequence.");
                StartEndingSequence();
            }
            else
            {
                Debug.Log("Ending sequence is already active. Ignoring input.");
            }
        }*/
    }

    public void StartEndingSequence()
    {
        if (isEndingSequenceActive)
        {
            Debug.LogWarning("StartEndingSequence: Attempted to start sequence while already active. Aborting.");
            return; // 중복 호출 방지
        }
        isEndingSequenceActive = true;
        Debug.Log("Starting Ending Sequence Coroutine.");
        StartCoroutine(EndingActionCoroutine());
    }

    IEnumerator EndingActionCoroutine()
    {
        // 텍스트 부모 오브젝트 활성화
        if (textParentPanel != null)
        {
            textParentPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: textParentPanel is null. Cannot activate.");
        }

        // 1. 배경이 먼저 어두워진다 (3초 동안)
        float timer = 0f;
        Color startBgColor = backgroundImage != null ? backgroundImage.color : Color.clear;
        Color endBgColor = new Color(startBgColor.r, startBgColor.g, startBgColor.b, 250f / 255f); // 목표 알파값 250/255f

        while (timer < backgroundFadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / backgroundFadeDuration);
            if (backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(startBgColor, endBgColor, progress);
            }
            yield return null;
        }
        if (backgroundImage != null) backgroundImage.color = endBgColor;

        // 2. Line이 양쪽에서 슬라이드 이동 (2초 동안)
        timer = 0f;

        Vector2 lintTopStartOffsetMin = lintTop != null ? lintTop.offsetMin : Vector2.zero;
        Vector2 lintTopStartOffsetMax = lintTop != null ? lintTop.offsetMax : Vector2.zero;
        Vector2 lintTopEndOffsetMin = new Vector2(0f, lintTopStartOffsetMin.y);
        Vector2 lintTopEndOffsetMax = new Vector2(0f, lintTopStartOffsetMax.y);

        Vector2 lintBottomStartOffsetMin = lintBottom != null ? lintBottom.offsetMin : Vector2.zero;
        Vector2 lintBottomStartOffsetMax = lintBottom != null ? lintBottom.offsetMax : Vector2.zero;
        Vector2 lintBottomEndOffsetMin = new Vector2(0f, lintBottomStartOffsetMin.y);
        Vector2 lintBottomEndOffsetMax = new Vector2(0f, lintBottomStartOffsetMax.y);

        while (timer < lintSlideDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / lintSlideDuration);

            if (lintTop != null)
            {
                lintTop.offsetMin = Vector2.Lerp(lintTopStartOffsetMin, lintTopEndOffsetMin, progress);
                lintTop.offsetMax = Vector2.Lerp(lintTopStartOffsetMax, lintTopEndOffsetMax, progress);
            }
            if (lintBottom != null)
            {
                lintBottom.offsetMin = Vector2.Lerp(lintBottomStartOffsetMin, lintBottomEndOffsetMin, progress);
                lintBottom.offsetMax = Vector2.Lerp(lintBottomStartOffsetMax, lintBottomEndOffsetMax, progress);
            }
            yield return null;
        }

        // 선 슬라이드 애니메이션 완료 후 정확한 위치로 설정
        if (lintTop != null)
        {
            lintTop.offsetMin = lintTopEndOffsetMin;
            lintTop.offsetMax = lintTopEndOffsetMax;
        }
        if (lintBottom != null)
        {
            lintBottom.offsetMin = lintBottomEndOffsetMin;
            lintBottom.offsetMax = lintBottomEndOffsetMax;
        }

        // 4. 텍스트 페이드인
        // "Chapter Clear" 텍스트 선명화
        if (chapterClearText != null)
        {
            yield return StartCoroutine(FadeInTMPText(chapterClearText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: chapterClearText is null, skipping fade-in.");
        }
        yield return new WaitForSeconds(1f); // 1초 대기

        // "Returning to the title screen shortly..." 텍스트 선명화
        if (returningText != null)
        {
            yield return StartCoroutine(FadeInTMPText(returningText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: returningText is null, skipping fade-in.");
        }
        yield return new WaitForSeconds(1f); // 1초 대기

        // 6. 카운트다운 (5 -> 4 -> 3 -> 2 -> 1)
        if (countdownText != null)
        {
            yield return StartCoroutine(FadeInTMPText(countdownText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: countdownText is null, skipping fade-in and countdown.");
        }
        yield return new WaitForSeconds(0.5f); // 텍스트 선명화 후 짧게 대기

        // 5부터 1까지 카운트다운
        for (int i = 5; i >= 1; i--) // 1이 될 때까지 반복
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f); // 1초 간격으로 숫자 변경
        }

        // 숫자가 1이 되면 텍스트 박스(countdownText) 꺼주기
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // 텍스트 박스가 꺼진 후 2초 뒤에 씬 이동
        yield return new WaitForSeconds(sceneMoveDelayAfterCountdown);

        // 7. 타이틀 씬 이동
        Debug.Log("Moving to " + titleSceneName + " scene.");
        if (CommonUIManager.instance != null)
        {
            CommonUIManager.instance.MoveScene(titleSceneName);
        }
        else
        {
            Debug.LogError("CommonUIManager.instance not found. Cannot move to " + titleSceneName + ".");
        }

        isEndingSequenceActive = false; // 시퀀스 종료
    }

    // TMP_Text를 서서히 선명하게 만드는 코루틴
    IEnumerator FadeInTMPText(TMP_Text targetText, float fadeDuration)
    {
        if (targetText == null)
        {
            Debug.LogWarning("FadeInTMPText: targetText is null. Aborting fade-in.");
            yield break;
        }

        // 시작과 끝 색상 정의
        Color startColor = initialTextColorAlphaZero;
        Color endColor = initialTextColorFullAlpha;

        Debug.Log($"FadeInTMPText for '{targetText.name}': Starting fade from alpha {startColor.a} to {endColor.a}. Duration: {fadeDuration}s");

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeDuration);
            targetText.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }
        targetText.color = endColor; // 정확한 끝 값으로 마무리
        Debug.Log($"FadeInTMPText for '{targetText.name}': Fade-in complete. Final Alpha: {targetText.color.a:F2}");
    }


    // TMP_Text의 알파값만 설정하는 헬퍼 함수
    private void SetTextAlpha(TMP_Text targetText, float alpha)
    {
        if (targetText != null)
        {
            Color tempColor = targetText.color;
            tempColor.a = alpha;
            targetText.color = tempColor;
        }
        else
        {
            Debug.LogError($"SetTextAlpha: Attempted to set alpha on a null TMP_Text object.");
        }
    }
}