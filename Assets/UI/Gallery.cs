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
        Vector3 newPosition = bgImgRect.position; // ���� ��ġ ��������
        newPosition.z = 3; // Z �� ����
        bgImgRect.position = newPosition; // ����� ��ġ ����

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
        if(isSizeUpImg && sizeUpImgIndex >= 0) // ������ Ŀ�� ���¸� �ٽ� �۾�����
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
        if (isSizeUpImg && sizeUpImgIndex >= 0) // ������ Ŀ�� ���¸� �ٽ� �۾�����
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
        // ������ ��� �̹��� �� ������ �̹��� Pos Z �� ����
        if (sizeUp)
        {
            //// ������ ���
            //Vector3 newPosition = bgImgRect.position; // ���� ��ġ ��������
            //newPosition.z = -1; // Z �� ����
            //bgImgRect.position = newPosition; // ����� ��ġ ����

            //// ������ �̹���
            //newPosition = rectTransform.position; // ���� ��ġ ��������
            //newPosition.z = -2; // Z �� ����
            //rectTransform.position = newPosition; // ����� ��ġ ����
        }
        else
        {
            //// ������ ���
            //Vector3 newPosition = bgImgRect.position; // ���� ��ġ ��������
            //newPosition.z = 3; // Z �� ����
            //bgImgRect.position = newPosition; // ����� ��ġ ����

            //// ������ �̹���
            //newPosition = rectTransform.position; // ���� ��ġ ��������
            //newPosition.z = -1; // Z �� ����
            //rectTransform.position = newPosition; // ����� ��ġ ����
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

        cellPhone.ScrollToTop(); // ��ũ�� �ʱ�ȭ

        // ������ �̹��� ũ�� ���� �� �ٽ� Pos Z �� 0���� �ʱ�ȭ
        if (!sizeUp)
        {
            //// ������ �̹���
            //Vector3 newPosition = rectTransform.position; // ���� ��ġ ��������
            //newPosition.z = 0; // Z �� ����
            //rectTransform.position = newPosition; // ����� ��ġ ����
        }
    }
}
