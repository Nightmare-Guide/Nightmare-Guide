using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;

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

    [Header ("# Lock Screen")]
    public GameObject LockPhoneUI;
    public GameObject sliderUI;
    public Image[] sliderImage;
    public Image[] puzzleUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public Scrollbar pwSlider;
    public TextMeshProUGUI pwText;
    public Material phoneBlurMat;
    public bool unLocked = false;

    [Header("# App Screen")]
    public GameObject appScreenUI;
    public Image[] appScreenImgs;
    public TextMeshProUGUI[] appScreenTexts;

    [Header("# ETC.")]
    public SchoolUIManager schoolUIManager;

    private void OnEnable()
    {
        // 퍼즐 해제 후, 바로 어플 화면 사용 가능하게 변경
        if (unLocked)
        {

        }
    }

    private void Start()
    {
        SetFirst();
    }

    // 처음 세팅 함수
    public void SetFirst()
    {
        phoneBlurMat.SetFloat("_Size", 0); // 휴대폰 Blur Spacing 값 초기화

        sliderUI.SetActive(true); // 슬라이더 활성화
        pwSlider.value = 0;
        PhoneSlider();
        puzzleUI[0].gameObject.SetActive(false); // 퍼즐 비활성화
        appScreenUI.SetActive(false);

        // 휴대폰 시간, 날짜, 슬라이드바 활성화
        sliderUI.SetActive(true);
        timeText.gameObject.SetActive(true);
        dateText.gameObject.SetActive(true);

        SchoolUIManager.instance.SetUIOpacity(sliderImage[0], true, 0.1f, 0f);
        SchoolUIManager.instance.SetUIOpacity(sliderImage[1], true, 0.1f, 0f);

        // 마지막 퍼즐 조각 제외 이미지 투명도 천천히 조정
        for (int i = 0; i < puzzleUI.Length; i++)
        {
            SchoolUIManager.instance.SetUIOpacity(puzzleUI[i], false, 0.1f, 0f);
        }

        // 퍼즐 초기화
        puzzleUI[1].GetComponent<PuzzleBoard>().InitializationPuzzle();
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

            if(!unLocked) // 해제를 못한 경우에는 블러 초기화
            {
                phoneBlurMat.SetFloat("_Size", 0); // 휴대폰 Blur Spacing 값 초기화
            }
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
            SchoolUIManager.instance.SetUIOpacity(sliderImage[0], false, 0.5f, 0f);
            SchoolUIManager.instance.SetUIOpacity(sliderImage[1], false, 0.5f, 0f);
            puzzleUI[0].gameObject.SetActive(true);

            // 마지막 퍼즐 조각 제외 이미지 투명도 천천히 조정
            for (int i = 0; i < puzzleUI.Length - 1; i++)
            {
                SchoolUIManager.instance.SetUIOpacity(puzzleUI[i], true, 0.6f, 0.2f);
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

        yield return new WaitForSeconds(moveSpeed - 0.05f);

        schoolUIManager.OpenUI(schoolUIManager.uiObjects[1]); // null 값용 UI 오브젝트 활성화

        // BoxCollider 비활성화
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
