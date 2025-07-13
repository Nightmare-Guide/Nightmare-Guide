using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Text Mesh Pro�� ����ϱ� ���� �߰�

public class Chap1Ending : MonoBehaviour
{
    // UI ��ҵ��� ������ ������ (SerializeField�� �ν����� ����)
    [SerializeField] private Image backgroundImage; // 1�� �̹��� (��ο��� ��� �̹���)
    [SerializeField] private RectTransform lintTop; // ��� �� (LintTop)
    [SerializeField] private RectTransform lintBottom; // �ϴ� �� (LintBottom)

    [SerializeField] private TMP_Text chapterClearText; // "Chapter Clear" �ؽ�Ʈ
    [SerializeField] private TMP_Text returningText; // "Returning to the title screen shortly..." �ؽ�Ʈ
    [SerializeField] private TMP_Text countdownText; // ���� ī��Ʈ�ٿ� �ؽ�Ʈ (5)

    // �ؽ�Ʈ���� ������ �ִ� �θ� ������Ʈ (�� ������Ʈ�� Ȱ��ȭ/��Ȱ��ȭ)
    [SerializeField] private GameObject textParentPanel;

    // �ִϸ��̼� �ð� ����
    [SerializeField] private float backgroundFadeDuration = 3f; // ����� ��ο����� �ð� (3��)
    [SerializeField] private float lintSlideDuration = 2f;      // ���� �����̵� �Ǵ� �ð� (2��)
    [SerializeField] private float textFadeInDuration = 1f;     // �ؽ�Ʈ ���̵��� �ð�

    // �� �̵��� ����� �� �̸�
    [SerializeField] private string titleSceneName = "Title Scene"; // "Title Scene"���� ���� (���� ����)
    [SerializeField] private float sceneMoveDelayAfterCountdown = 2f; // ī��Ʈ�ٿ� 1�� �� �� �̵� ������ (2��)

    private Color initialTextColorAlphaZero; // �ؽ�Ʈ�� �ʱ� ���� ���� ������ ����
    private Color initialTextColorFullAlpha; // �ؽ�Ʈ�� �ʱ� ������ ���� (���̵��� ��ǥ)

    private bool isEndingSequenceActive = false; // ���� ������ �ߺ� ���� ���� �÷���

    void Awake()
    {
        // 1. BG �ʱ� ���� 0�� ����
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

        // �ؽ�Ʈ���� �ʱ� ���� ���� (TMP_Text�� initialTextColorFullAlpha ���� �� �ʿ�)
        if (chapterClearText != null)
        {
            initialTextColorFullAlpha = chapterClearText.color; // ���� ����(RGB) ��������
            initialTextColorFullAlpha.a = 1f; // ���� ���� 1 (���� ������)�� ����

            initialTextColorAlphaZero = new Color(initialTextColorFullAlpha.r, initialTextColorFullAlpha.g, initialTextColorFullAlpha.b, 0f);
        }
        else
        {
            Debug.LogError("Awake: chapterClearText is not assigned in the Inspector!");
        }

        // ��� �ؽ�Ʈ�� �ʱ� ���� ���·� ����
        SetTextAlpha(chapterClearText, 0f);
        SetTextAlpha(returningText, 0f);
        SetTextAlpha(countdownText, 0f);

        if (countdownText != null)
        {
            countdownText.text = "5"; // �ʱ� ī��Ʈ�ٿ� ���� ����
        }

        // �ؽ�Ʈ �θ� ������Ʈ�� ��Ȱ��ȭ ���·� ����
        if (textParentPanel != null)
        {
            textParentPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Awake: textParentPanel is not assigned in the Inspector!");
        }

        // Lint �ʱ� ��ġ ���� (Inspector���� �����ϴ� ���� ���� ������������, �ڵ忡���� ���)
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
        // 1. ���� �� Ctrl + F7���� ������ EndingAction �޼ҵ尡 ����
       /* if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F7))
        {
            if (!isEndingSequenceActive) // ���� �������� ���� ���� �ƴ� ���� ����
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
            return; // �ߺ� ȣ�� ����
        }
        isEndingSequenceActive = true;
        Debug.Log("Starting Ending Sequence Coroutine.");
        StartCoroutine(EndingActionCoroutine());
    }

    IEnumerator EndingActionCoroutine()
    {
        // �ؽ�Ʈ �θ� ������Ʈ Ȱ��ȭ
        if (textParentPanel != null)
        {
            textParentPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: textParentPanel is null. Cannot activate.");
        }

        // 1. ����� ���� ��ο����� (3�� ����)
        float timer = 0f;
        Color startBgColor = backgroundImage != null ? backgroundImage.color : Color.clear;
        Color endBgColor = new Color(startBgColor.r, startBgColor.g, startBgColor.b, 250f / 255f); // ��ǥ ���İ� 250/255f

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

        // 2. Line�� ���ʿ��� �����̵� �̵� (2�� ����)
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

        // �� �����̵� �ִϸ��̼� �Ϸ� �� ��Ȯ�� ��ġ�� ����
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

        // 4. �ؽ�Ʈ ���̵���
        // "Chapter Clear" �ؽ�Ʈ ����ȭ
        if (chapterClearText != null)
        {
            yield return StartCoroutine(FadeInTMPText(chapterClearText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: chapterClearText is null, skipping fade-in.");
        }
        yield return new WaitForSeconds(1f); // 1�� ���

        // "Returning to the title screen shortly..." �ؽ�Ʈ ����ȭ
        if (returningText != null)
        {
            yield return StartCoroutine(FadeInTMPText(returningText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: returningText is null, skipping fade-in.");
        }
        yield return new WaitForSeconds(1f); // 1�� ���

        // 6. ī��Ʈ�ٿ� (5 -> 4 -> 3 -> 2 -> 1)
        if (countdownText != null)
        {
            yield return StartCoroutine(FadeInTMPText(countdownText, textFadeInDuration));
        }
        else
        {
            Debug.LogError("EndingActionCoroutine: countdownText is null, skipping fade-in and countdown.");
        }
        yield return new WaitForSeconds(0.5f); // �ؽ�Ʈ ����ȭ �� ª�� ���

        // 5���� 1���� ī��Ʈ�ٿ�
        for (int i = 5; i >= 1; i--) // 1�� �� ������ �ݺ�
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f); // 1�� �������� ���� ����
        }

        // ���ڰ� 1�� �Ǹ� �ؽ�Ʈ �ڽ�(countdownText) ���ֱ�
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // �ؽ�Ʈ �ڽ��� ���� �� 2�� �ڿ� �� �̵�
        yield return new WaitForSeconds(sceneMoveDelayAfterCountdown);

        // 7. Ÿ��Ʋ �� �̵�
        Debug.Log("Moving to " + titleSceneName + " scene.");
        if (CommonUIManager.instance != null)
        {
            CommonUIManager.instance.MoveScene(titleSceneName);
        }
        else
        {
            Debug.LogError("CommonUIManager.instance not found. Cannot move to " + titleSceneName + ".");
        }

        isEndingSequenceActive = false; // ������ ����
    }

    // TMP_Text�� ������ �����ϰ� ����� �ڷ�ƾ
    IEnumerator FadeInTMPText(TMP_Text targetText, float fadeDuration)
    {
        if (targetText == null)
        {
            Debug.LogWarning("FadeInTMPText: targetText is null. Aborting fade-in.");
            yield break;
        }

        // ���۰� �� ���� ����
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
        targetText.color = endColor; // ��Ȯ�� �� ������ ������
        Debug.Log($"FadeInTMPText for '{targetText.name}': Fade-in complete. Final Alpha: {targetText.color.a:F2}");
    }


    // TMP_Text�� ���İ��� �����ϴ� ���� �Լ�
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