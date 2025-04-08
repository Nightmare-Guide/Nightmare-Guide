using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;
using static SchoolUIManager;

public class CellPhone : MonoBehaviour
{
    [Header("# Normal")]
    // Position ��
    public Vector3 finalPos;
    public float moveSpeed;

    // Rotatioin ��
    public Vector3 finalRotate;
    public float rotateSpeed;

    // bool ��
    public bool isUsing = false;

    [Header("# Lock Screen")]
    public GameObject LockPhoneUI;
    public GameObject sliderUI;
    public Image[] sliderImage;
    public Image[] puzzleUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public Scrollbar pwSlider;
    public TextMeshProUGUI pwText;
    public Material phoneBlurMat;

    [Header("# App Screen")]
    public GameObject appScreenUI;
    public Image[] appScreenImgs;
    public TextMeshProUGUI[] appScreenTexts;
    private Vector2[][] appScreenAnchors;
    [SerializeField] private GameObject bottomBar; // �ڿ��������� ���� �߰��� Img ������Ʈ
    [SerializeField] private ScrollRect[] scrollRects;

    [Header("# ETC.")]
    public SchoolUIManager schoolUIManager;

    private void Awake()
    {
        // ���� ȭ�� �⺻ anchors �� ���� -> min, max
        appScreenAnchors = new Vector2[4][]
        {
            new Vector2[] { new Vector2(0.08f, 0.73f) , new Vector2(0.32f, 0.85f) }, // Dial App Screen
            new Vector2[] { new Vector2(0.38f, 0.73f), new Vector2(0.62f, 0.85f) }, // Message App Screen
            new Vector2[] { new Vector2(0.68f, 0.73f), new Vector2(0.92f, 0.85f) }, // Note App Screen
            new Vector2[] { new Vector2(0.08f, 0.57f) , new Vector2(0.32f, 0.69f) }, // Gallery App Screen
        };
    }

    private void Start()
    {
        SetFirst();
    }

    private void OnDisable()
    {
        phoneBlurMat.SetFloat("_Size", 0); // �޴��� Blur Spacing �� �ʱ�ȭ
    }

    // ó�� ���� �Լ�
    public void SetFirst()
    {
        phoneBlurMat.SetFloat("_Size", 0); // �޴��� Blur Spacing �� �ʱ�ȭ


        // �޴��� ��� ���� ���� Ȯ�� �� ���ȭ�� �ʱ�ȭ
        foreach (CharacterPhoneInfo cellPhone in SchoolUIManager.instance.phoneInfos)
        {
            if (!this.gameObject.name.Contains(cellPhone.name)) continue;

            if (cellPhone.isUnlocked) break;

            sliderUI.SetActive(true); // �����̴� Ȱ��ȭ
            pwSlider.value = 0;
            PhoneSlider();
            appScreenUI.SetActive(false);

            // �޴��� �ð�, ��¥, �����̵�� Ȱ��ȭ
            sliderUI.SetActive(true);
            timeText.gameObject.SetActive(true);
            dateText.gameObject.SetActive(true);

            schoolUIManager.SetUIOpacity(sliderImage[0], true, 0.1f, 0f);
            schoolUIManager.SetUIOpacity(sliderImage[1], true, 0.1f, 0f);

            // ������ ���� ���� ���� �̹��� ���� õõ�� ����
            for (int i = 0; i < puzzleUI.Length; i++)
            {
                schoolUIManager.SetUIOpacity(puzzleUI[i], false, 0.1f, 0f);
            }

            // ���� �ʱ�ȭ
            if (!this.gameObject.name.Contains("Steven"))
            {
                puzzleUI[1].GetComponent<PuzzleBoard>().InitializationPuzzle(); // ���� �ʱ�ȭ
                puzzleUI[0].gameObject.SetActive(false); // ���� ��Ȱ��ȭ
            }
        }
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            GetDate();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isUsing)
        {
            isUsing = false;
            this.gameObject.SetActive(false);
        }
    }

    // �ΰ��� �޴��� ������Ʈ ��ȣ�ۿ� �Լ�
    public void UpPhone(Vector3 pos, Vector3 rotate)
    {
        isUsing = true;

        finalPos = pos;
        finalRotate = rotate;

        StartCoroutine(DoTween());
    }

    // �޴��� ��¥ �� �ð� �Է� �Լ�
    void GetDate()
    {
        // ���� ��¥�� �ð� ��������
        DateTime currentTime = DateTime.Now;

        // �̱� �������� ��¥ ������ (MM/dd/yyyy)
        string formattedDate = currentTime.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture);

        // �ð� ������ (������ 00��, 13�ÿ��� 24�ô� -12�� ǥ��, 1�ú��� 12�ô� �״�� ǥ��)
        int hour = currentTime.Hour;
        string formattedTime;

        if (hour == 0)
        {
            // ������ 00�÷� ǥ��
            formattedTime = "00:" + currentTime.ToString("mm");
        }
        else if (hour >= 13)
        {
            // 13�ÿ��� 24�ô� -12�� ��ȯ
            formattedTime = (hour - 12).ToString("00") + ":" + currentTime.ToString("mm");
        }
        else
        {
            // 1�ÿ��� 12�ô� �״�� ǥ��
            formattedTime = hour.ToString("00") + ":" + currentTime.ToString("mm");
        }

        // UI �ؽ�Ʈ�� ��¥�� �ð� ǥ��
        dateText.text = formattedDate;
        timeText.text = formattedTime;
        appScreenTexts[0].text = formattedTime;
    }

    // �޴��� ���ȭ�� �����̴� ��ȣ�ۿ� �Լ�
    public void PhoneSlider()
    {
        // Slider Value �� ���� ��ȭ
        UnityEngine.Color color = pwText.color;  // ���� ���� ��������
        color.a = Mathf.Lerp(1f, 0f, pwSlider.value * 2); // ���� ����  // 0 �� 160 / 255, 1 �� 0
        pwText.color = color;  // �� ���� ����

        // �޴��� Blur
        phoneBlurMat.SetFloat("_Size", pwSlider.value / 2.5f); // Spacing �� ����

        if (pwSlider.value >= 1)
        {
            schoolUIManager.SetUIOpacity(sliderImage[0], false, 0.5f, 0f);
            schoolUIManager.SetUIOpacity(sliderImage[1], false, 0.5f, 0f);

            // ����,���̺�� �޴����� �����̵� ����� ����
            if (!this.gameObject.name.Contains("Steven"))
            {
                puzzleUI[0].gameObject.SetActive(true);

                // ������ ���� ���� ���� �̹��� ���� õõ�� ����
                for (int i = 0; i < puzzleUI.Length - 1; i++)
                {
                    schoolUIManager.SetUIOpacity(puzzleUI[i], true, 0.6f, 0.2f);
                }
            }
            else
            {
                // ��Ƽ�� �޴����� �����̴��� ��� ���� ��, �ٷ� �������
                schoolUIManager.phoneInfos[2].isUnlocked = true;

                // App Screen UI Ȱ��ȭ
                appScreenUI.SetActive(true);

                foreach (Image img in appScreenImgs)
                {
                    schoolUIManager.SetUIOpacity(img, true, 0.5f, 0f);
                }
                foreach (TextMeshProUGUI text in appScreenTexts)
                {
                    schoolUIManager.SetUIOpacity(text, true, 0.5f, 0f);
                }

                // �ð�, ��¥ text ������ ��Ȱ��ȭ
                schoolUIManager.SetUIOpacity(timeText, false, 0.2f, 0f);
                schoolUIManager.SetUIOpacity(dateText, false, 0.2f, 0f);
            }
        }
    }

    // DoTween ������ Ȱ���� �̵��Լ�
    IEnumerator DoTween()
    {
        // InQuad : ������ �� ������ ����, ���� �� ����
        // OutQuad : ������ �� ����, ���� �� ����
        transform.DOMove(finalPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(finalRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        transform.DOScale(2.5f, moveSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(moveSpeed - 0.05f);

        schoolUIManager.OpenUI(schoolUIManager.uiObjects[1]); // null ���� UI ������Ʈ Ȱ��ȭ

        // BoxCollider ��Ȱ��ȭ
        this.GetComponent<BoxCollider>().enabled = false;
    }

    public void AppIconButton(RectTransform appScreenRect)
    {
        StartCoroutine(AnchorsCoroutine(appScreenRect, appScreenRect.anchorMin, appScreenRect.anchorMax, Vector2.zero, Vector2.one, 0.15f, true));
    }

    public void UpScreenButton(RectTransform appScreenRect)
    {
        StartCoroutine(AnchorsCoroutine(appScreenRect, Vector2.zero, new Vector2(1, 0), Vector2.zero, Vector2.one, 0.08f, true));
    }

    public void BackButton(RectTransform appScreenRect)
    {
        StartCoroutine(AnchorsCoroutine(appScreenRect, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(1, 0), 0.08f, true));
    }

    public void HomeButton(RectTransform appScreenRect)
    {
        string objectName = appScreenRect.gameObject.name;

        if (objectName.Contains("Dial"))
        {
            StartCoroutine(AnchorsCoroutine(appScreenRect, appScreenRect.anchorMin, appScreenRect.anchorMax, appScreenAnchors[0][0], appScreenAnchors[0][1], 0.15f, false));
        }
        else if (objectName.Contains("Message") || objectName.Contains("Conversation"))
        {
            StartCoroutine(AnchorsCoroutine(appScreenRect, appScreenRect.anchorMin, appScreenRect.anchorMax, appScreenAnchors[1][0], appScreenAnchors[1][1], 0.15f, false));
        }
        else if (objectName.Contains("Note") || objectName.Contains("Page"))
        {
            StartCoroutine(AnchorsCoroutine(appScreenRect, appScreenRect.anchorMin, appScreenRect.anchorMax, appScreenAnchors[2][0], appScreenAnchors[2][1], 0.15f, false));
        }
        else if (objectName.Contains("Gallery"))
        {
            StartCoroutine(AnchorsCoroutine(appScreenRect, appScreenRect.anchorMin, appScreenRect.anchorMax, appScreenAnchors[3][0], appScreenAnchors[3][1], 0.15f, false));
        }
    }

    IEnumerator AnchorsCoroutine(RectTransform rectTransform, Vector2 startMin, Vector2 startMax, Vector2 targetMin, Vector2 targetMax, float time, bool use)
    {
        if (use)
        {
            rectTransform.gameObject.SetActive(true);
        }
        else
        {
            bottomBar.SetActive(false); // ���� �ÿ��� �ϴ� �� Img ��Ȱ��ȭ
        }

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time; // 0 ~ 1 ������ ��

            rectTransform.anchorMin = Vector2.Lerp(startMin, targetMin, t);
            rectTransform.anchorMax = Vector2.Lerp(startMax, targetMax, t);

            yield return null; // ���� �����ӱ��� ���
        }

        // ���� �� ����
        rectTransform.anchorMin = targetMin;
        rectTransform.anchorMax = targetMax;

        ScrollToTop(); // ��ũ�� �ʱ�ȭ

        if (!use)
        {
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            bottomBar.SetActive(true); // ui Ȱ��ȭ ��, �ϴ� �� Img Ȱ��ȭ
        }
    }

    // ��ũ���� �� ���� �ø��� �Լ�
    public void ScrollToTop()
    {
        foreach (ScrollRect scroll in scrollRects)
        {
            scroll.verticalNormalizedPosition = 1f;
        }
    }
}
