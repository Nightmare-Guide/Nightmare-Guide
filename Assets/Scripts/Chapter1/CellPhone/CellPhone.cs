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
    // Position 값
    public Vector3 finalPos;
    public float moveSpeed;

    // Rotatioin 값
    public Vector3 finalRotate;
    public float rotateSpeed;

    // bool 값
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
    [SerializeField] private GameObject bottomBar; // 자연스러움을 위해 추가된 Img 오브젝트
    [SerializeField] private ScrollRect[] scrollRects;

    [Header("# ETC.")]
    public SchoolUIManager schoolUIManager;

    private void Awake()
    {
        // 어플 화면 기본 anchors 값 저장 -> min, max
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
        phoneBlurMat.SetFloat("_Size", 0); // 휴대폰 Blur Spacing 값 초기화
    }

    // 처음 세팅 함수
    public void SetFirst()
    {
        phoneBlurMat.SetFloat("_Size", 0); // 휴대폰 Blur Spacing 값 초기화


        // 휴대폰 잠금 해제 여부 확인 후 잠금화면 초기화
        foreach (CharacterPhoneInfo cellPhone in SchoolUIManager.instance.phoneInfos)
        {
            if (!this.gameObject.name.Contains(cellPhone.name)) continue;

            if (cellPhone.isUnlocked) break;

            sliderUI.SetActive(true); // 슬라이더 활성화
            pwSlider.value = 0;
            PhoneSlider();
            appScreenUI.SetActive(false);

            // 휴대폰 시간, 날짜, 슬라이드바 활성화
            sliderUI.SetActive(true);
            timeText.gameObject.SetActive(true);
            dateText.gameObject.SetActive(true);

            schoolUIManager.SetUIOpacity(sliderImage[0], true, 0.1f, 0f);
            schoolUIManager.SetUIOpacity(sliderImage[1], true, 0.1f, 0f);

            // 마지막 퍼즐 조각 제외 이미지 투명도 천천히 조정
            for (int i = 0; i < puzzleUI.Length; i++)
            {
                schoolUIManager.SetUIOpacity(puzzleUI[i], false, 0.1f, 0f);
            }

            // 퍼즐 초기화
            if (!this.gameObject.name.Contains("Steven"))
            {
                puzzleUI[1].GetComponent<PuzzleBoard>().InitializationPuzzle(); // 퍼즐 초기화
                puzzleUI[0].gameObject.SetActive(false); // 퍼즐 비활성화
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

    // 인게임 휴대폰 오브젝트 상호작용 함수
    public void UpPhone(Vector3 pos, Vector3 rotate)
    {
        isUsing = true;

        finalPos = pos;
        finalRotate = rotate;

        StartCoroutine(DoTween());
    }

    // 휴대폰 날짜 및 시간 입력 함수
    void GetDate()
    {
        // 현재 날짜와 시간 가져오기
        DateTime currentTime = DateTime.Now;

        // 미국 기준으로 날짜 포맷팅 (MM/dd/yyyy)
        string formattedDate = currentTime.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture);

        // 시간 포맷팅 (자정은 00시, 13시에서 24시는 -12로 표기, 1시부터 12시는 그대로 표시)
        int hour = currentTime.Hour;
        string formattedTime;

        if (hour == 0)
        {
            // 자정은 00시로 표시
            formattedTime = "00:" + currentTime.ToString("mm");
        }
        else if (hour >= 13)
        {
            // 13시에서 24시는 -12로 변환
            formattedTime = (hour - 12).ToString("00") + ":" + currentTime.ToString("mm");
        }
        else
        {
            // 1시에서 12시는 그대로 표시
            formattedTime = hour.ToString("00") + ":" + currentTime.ToString("mm");
        }

        // UI 텍스트에 날짜와 시간 표시
        dateText.text = formattedDate;
        timeText.text = formattedTime;
        appScreenTexts[0].text = formattedTime;
    }

    // 휴대폰 잠금화면 슬라이더 상호작용 함수
    public void PhoneSlider()
    {
        // Slider Value 에 따른 변화
        UnityEngine.Color color = pwText.color;  // 기존 색상 가져오기
        color.a = Mathf.Lerp(1f, 0f, pwSlider.value * 2); // 투명도 변경  // 0 → 160 / 255, 1 → 0
        pwText.color = color;  // 새 색상 적용

        // 휴대폰 Blur
        phoneBlurMat.SetFloat("_Size", pwSlider.value / 2.5f); // Spacing 값 변경

        if (pwSlider.value >= 1)
        {
            schoolUIManager.SetUIOpacity(sliderImage[0], false, 0.5f, 0f);
            schoolUIManager.SetUIOpacity(sliderImage[1], false, 0.5f, 0f);

            // 에단,데이비드 휴대폰만 슬라이드 퍼즐로 연결
            if (!this.gameObject.name.Contains("Steven"))
            {
                puzzleUI[0].gameObject.SetActive(true);

                // 마지막 퍼즐 조각 제외 이미지 투명도 천천히 조정
                for (int i = 0; i < puzzleUI.Length - 1; i++)
                {
                    schoolUIManager.SetUIOpacity(puzzleUI[i], true, 0.6f, 0.2f);
                }
            }
            else
            {
                // 스티븐 휴대폰은 슬라이더로 잠금 해제 후, 바로 잠금해제
                schoolUIManager.phoneInfos[2].isUnlocked = true;

                // App Screen UI 활성화
                appScreenUI.SetActive(true);

                foreach (Image img in appScreenImgs)
                {
                    schoolUIManager.SetUIOpacity(img, true, 0.5f, 0f);
                }
                foreach (TextMeshProUGUI text in appScreenTexts)
                {
                    schoolUIManager.SetUIOpacity(text, true, 0.5f, 0f);
                }

                // 시간, 날짜 text 서서히 비활성화
                schoolUIManager.SetUIOpacity(timeText, false, 0.2f, 0f);
                schoolUIManager.SetUIOpacity(dateText, false, 0.2f, 0f);
            }
        }
    }

    // DoTween 에셋을 활용한 이동함수
    IEnumerator DoTween()
    {
        // InQuad : 시작할 때 빠르게 가속, 끝날 때 감속
        // OutQuad : 시작할 때 감속, 끝날 때 가속
        transform.DOMove(finalPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(finalRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        transform.DOScale(2.5f, moveSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(moveSpeed - 0.05f);

        schoolUIManager.OpenUI(schoolUIManager.uiObjects[1]); // null 값용 UI 오브젝트 활성화

        // BoxCollider 비활성화
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
            bottomBar.SetActive(false); // 종료 시에는 하단 바 Img 비활성화
        }

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time; // 0 ~ 1 사이의 값

            rectTransform.anchorMin = Vector2.Lerp(startMin, targetMin, t);
            rectTransform.anchorMax = Vector2.Lerp(startMax, targetMax, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 최종 값 보정
        rectTransform.anchorMin = targetMin;
        rectTransform.anchorMax = targetMax;

        ScrollToTop(); // 스크롤 초기화

        if (!use)
        {
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            bottomBar.SetActive(true); // ui 활성화 후, 하단 바 Img 활성화
        }
    }

    // 스크롤을 맨 위로 올리는 함수
    public void ScrollToTop()
    {
        foreach (ScrollRect scroll in scrollRects)
        {
            scroll.verticalNormalizedPosition = 1f;
        }
    }
}
