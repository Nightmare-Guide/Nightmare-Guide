using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    [SerializeField] private CellPhone cellPhone;
    private Vector2[][] imgAnchors;
    private Vector2[] bigAnchors;
    [SerializeField] RectTransform[] imgRects;

    bool isSizeUpImg = false;
    int sizeUpImgIndex = -1;

    private void Awake()
    {
        imgAnchors = new Vector2[9][]
        {
            new Vector2[] { new Vector2(0.08f, 0.73f) , new Vector2(0.32f, 0.85f) },
            new Vector2[] { new Vector2(0.38f, 0.73f), new Vector2(0.62f, 0.85f) },
            new Vector2[] { new Vector2(0.68f, 0.73f), new Vector2(0.92f, 0.85f) },
            new Vector2[] { new Vector2(0.08f, 0.57f) , new Vector2(0.32f, 0.69f) },
            new Vector2[] { new Vector2(0.08f, 0.73f) , new Vector2(0.32f, 0.85f) },
            new Vector2[] { new Vector2(0.38f, 0.73f), new Vector2(0.62f, 0.85f) },
            new Vector2[] { new Vector2(0.68f, 0.73f), new Vector2(0.92f, 0.85f) },
            new Vector2[] { new Vector2(0.08f, 0.57f) , new Vector2(0.32f, 0.69f) },
            new Vector2[] { new Vector2(0.08f, 0.73f) , new Vector2(0.32f, 0.85f) },
        };

        bigAnchors = new Vector2[2];
        bigAnchors[0] = new Vector2(0f, 0.2f); // Min Value
        bigAnchors[1] = new Vector2(1f, 0.8f); // Max Value
    }

    public void BackButton()
    {
        if(isSizeUpImg && sizeUpImgIndex >= 0) // ������ Ŀ�� ���¸� �ٽ� �۾�����
        {
            StartCoroutine(AnchorsCoroutine(imgRects[sizeUpImgIndex], bigAnchors[0], bigAnchors[1], imgAnchors[sizeUpImgIndex][1], imgAnchors[sizeUpImgIndex][1], 0.15f));
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
        if (isSizeUpImg && sizeUpImgIndex >= 0) // ������ Ŀ�� ���¸� �ٽ� �۾�����
        {
            StartCoroutine(AnchorsCoroutine(imgRects[sizeUpImgIndex], bigAnchors[0], bigAnchors[1], imgAnchors[sizeUpImgIndex][1], imgAnchors[sizeUpImgIndex][1], 0.15f));
            isSizeUpImg = false;
            sizeUpImgIndex = -1;
        }
        cellPhone.HomeButton(this.GetComponent<RectTransform>());
    }

    public void SizeUpImgButton(int index)
    {
        if (isSizeUpImg)
            return;

        StartCoroutine(AnchorsCoroutine(imgRects[index], imgAnchors[index][0], imgAnchors[index][1], bigAnchors[0], bigAnchors[1], 0.15f));
        isSizeUpImg = true;
        sizeUpImgIndex = index;
    }

    IEnumerator AnchorsCoroutine(RectTransform rectTransform, Vector2 startMin, Vector2 startMax, Vector2 targetMin, Vector2 targetMax, float time)
    {
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

        cellPhone.ScrollToTop(); // ��ũ�� �ʱ�ȭ
    }
}
