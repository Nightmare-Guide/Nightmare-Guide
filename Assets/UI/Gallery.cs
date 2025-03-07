using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Gallery : MonoBehaviour
{
    [SerializeField] private CellPhone cellPhone;
    private Vector2[][] imgAnchors;
    private Vector2[] bigAnchors;
    [SerializeField] RectTransform[] imgRects;

    bool isSizeUpImg = false;
    int sizeUpImgIndex = -1;
    [SerializeField] RectTransform bgImgRect;

    private void Awake()
    {
        Vector3 newPosition = bgImgRect.position; // 현재 위치 가져오기
        newPosition.z = 3; // Z 값 변경
        bgImgRect.position = newPosition; // 변경된 위치 적용

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

    public void BackButton()
    {
        if(isSizeUpImg && sizeUpImgIndex >= 0) // 사진이 커진 상태면 다시 작아지게
        {
            StartCoroutine(AnchorsCoroutine(imgRects[sizeUpImgIndex], bigAnchors[0], bigAnchors[1], imgAnchors[sizeUpImgIndex][0], imgAnchors[sizeUpImgIndex][1], 0.15f, false));
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
        if (isSizeUpImg && sizeUpImgIndex >= 0) // 사진이 커진 상태면 다시 작아지게
        {
            StartCoroutine(AnchorsCoroutine(imgRects[sizeUpImgIndex], bigAnchors[0], bigAnchors[1], imgAnchors[sizeUpImgIndex][1], imgAnchors[sizeUpImgIndex][1], 0.15f, false));
            isSizeUpImg = false;
            sizeUpImgIndex = -1;
        }
        cellPhone.HomeButton(this.GetComponent<RectTransform>());
    }

    public void SizeUpImgButton(int index)
    {
        if (isSizeUpImg)
            return;

        StartCoroutine(AnchorsCoroutine(imgRects[index], imgAnchors[index][0], imgAnchors[index][1], bigAnchors[0], bigAnchors[1], 0.15f, true));
        isSizeUpImg = true;
        sizeUpImgIndex = index;
    }

    IEnumerator AnchorsCoroutine(RectTransform rectTransform, Vector2 startMin, Vector2 startMax, Vector2 targetMin, Vector2 targetMax, float time, bool sizeUp)
    {
        // 검은색 배경 이미지 및 선택한 이미지 Pos Z 값 변경
        if (sizeUp)
        {
            //// 검은색 배경
            //Vector3 newPosition = bgImgRect.position; // 현재 위치 가져오기
            //newPosition.z = -1; // Z 값 변경
            //bgImgRect.position = newPosition; // 변경된 위치 적용

            //// 선택한 이미지
            //newPosition = rectTransform.position; // 현재 위치 가져오기
            //newPosition.z = -2; // Z 값 변경
            //rectTransform.position = newPosition; // 변경된 위치 적용
        }
        else
        {
            //// 검은색 배경
            //Vector3 newPosition = bgImgRect.position; // 현재 위치 가져오기
            //newPosition.z = 3; // Z 값 변경
            //bgImgRect.position = newPosition; // 변경된 위치 적용

            //// 선택한 이미지
            //newPosition = rectTransform.position; // 현재 위치 가져오기
            //newPosition.z = -1; // Z 값 변경
            //rectTransform.position = newPosition; // 변경된 위치 적용
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
            //// 선택한 이미지
            //Vector3 newPosition = rectTransform.position; // 현재 위치 가져오기
            //newPosition.z = 0; // Z 값 변경
            //rectTransform.position = newPosition; // 변경된 위치 적용
        }
    }
}
