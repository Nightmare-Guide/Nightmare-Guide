using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Gallery : MonoBehaviour
{
    [SerializeField] private CellPhone cellPhone;
    private Vector2[][] imgAnchors;
    private Vector2[] bigAnchors;
    [SerializeField] Image[] galleryImgs;
    [SerializeField] RectTransform mainImgRect;
    [SerializeField] GameObject bgImg;

    bool isSizeUpImg = false;
    int sizeUpImgIndex = -1;

    private void Awake()
    {
        SetFrist();

        imgAnchors = new Vector2[9][]
        {
            new Vector2[] { new Vector2(0f, 0.8f) , new Vector2(0.33f, 1f) },
            new Vector2[] { new Vector2(0.335f, 0.8f), new Vector2(0.665f, 1f) },
            new Vector2[] { new Vector2(0.67f, 0.8f), new Vector2(1f, 1f) },
            new Vector2[] { new Vector2(0f, 0.595f) , new Vector2(0.33f, 0.795f) },
            new Vector2[] { new Vector2(0.335f, 0.595f) , new Vector2(0.665f, 0.795f) },
            new Vector2[] { new Vector2(0.67f, 0.595f), new Vector2(1f, 0.795f) },
            new Vector2[] { new Vector2(0f, 0.39f), new Vector2(0.33f, 0.59f) },
            new Vector2[] { new Vector2(0.335f, 0.39f) , new Vector2(0.665f, 0.59f) },
            new Vector2[] { new Vector2(0.67f, 0.39f) , new Vector2(1f, 0.59f) },
        };

        bigAnchors = new Vector2[2];
        bigAnchors[0] = new Vector2(0f, 0.2f); // Min Value
        bigAnchors[1] = new Vector2(1f, 0.8f); // Max Value
    }

    private void OnEnable()
    {
        SetFrist();
    }

    void SetFrist()
    {
        isSizeUpImg = false;
        sizeUpImgIndex = -1;
        bgImg.SetActive(false);
        mainImgRect.anchorMax = Vector2.zero;
        mainImgRect.anchorMin = Vector2.zero;
    }

    public void BackButton()
    {
        if(isSizeUpImg && sizeUpImgIndex >= 0) // 사진이 커진 상태면 다시 작아지게
        {
            StartCoroutine(AnchorsCoroutine(mainImgRect, bigAnchors[0], bigAnchors[1], imgAnchors[sizeUpImgIndex][0], imgAnchors[sizeUpImgIndex][1], 0.15f, false));
            isSizeUpImg = false;
            sizeUpImgIndex = -1;
        }
        else
        {
            cellPhone.HomeButton(this.GetComponent<RectTransform>());
        }
    }
    
    public void HomeButton()
    {
        cellPhone.HomeButton(this.GetComponent<RectTransform>()); // 갤러리 축소
        isSizeUpImg = false;
        sizeUpImgIndex = -1;
    }

    public void SizeUpImgButton(int index)
    {
        if (isSizeUpImg)
            return;

        sizeUpImgIndex = index;
        isSizeUpImg = true;
        StartCoroutine(AnchorsCoroutine(mainImgRect, imgAnchors[index][0], imgAnchors[index][1], bigAnchors[0], bigAnchors[1], 0.15f, true));
    }

    IEnumerator AnchorsCoroutine(RectTransform rectTransform, Vector2 startMin, Vector2 startMax, Vector2 targetMin, Vector2 targetMax, float time, bool sizeUp)
    {
        if (sizeUp)
        {
            // Main Img 오브젝트 활성화
            mainImgRect.gameObject.SetActive(true);
            mainImgRect.GetComponent<Image>().sprite = galleryImgs[sizeUpImgIndex].sprite; // 이미지 변경
            bgImg.SetActive(true);
        }
        else
        {
            bgImg.SetActive(false);
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

        cellPhone.ScrollToTop(); // 스크롤 초기화

        // 선택한 이미지 크기 줄인 후 다시 Pos Z 값 0으로 초기화
        if (!sizeUp)
        {
            // Main Img 오브젝트 비활성화
            mainImgRect.gameObject.SetActive(false);
        }
    }
}
