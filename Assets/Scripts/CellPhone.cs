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
    // Position ��
    public Vector3 finalPos;
    public float moveSpeed;

    // Rotatioin ��
    public Vector3 finalRotate;
    public float rotateSpeed;

    [Header ("# Locked")]
    // ���ȭ�� UI
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

    public SchoolUIManager schoolUIManager;

    private void OnEnable()
    {
        // ���� ���� ��, �ٷ� ���� ȭ�� ��� �����ϰ� ����
        if (unLocked)
        {

        }
    }

    private void Start()
    {
        phoneBlurMat.SetFloat("_Size", 0); // �޴��� Blur Spacing �� �ʱ�ȭ

        sliderUI.SetActive(true); // �����̴� Ȱ��ȭ
        puzzleUI[0].gameObject.SetActive(false); // ���� ��Ȱ��ȭ
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            GetDate();
        }
    }

    public void UpPhone(Vector3 pos, Vector3 rotate)
    {
        finalPos = pos;
        finalRotate = rotate;
        StartCoroutine(DoTween());
    }

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
    }

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
            SchoolUIManager.instance.SetUIOpacity(sliderImage[0], true, 1f, 0.2f);
            SchoolUIManager.instance.SetUIOpacity(sliderImage[1], true, 1f, 0.2f);
            puzzleUI[0].gameObject.SetActive(true);

            // ������ ���� ���� ���� �̹��� ���� õõ�� ����
            for (int i = 0; i < puzzleUI.Length - 1; i++)
            {
                SchoolUIManager.instance.SetUIOpacity(puzzleUI[i], true, 0.6f, 0.2f);
            }
        }
    }


    IEnumerator DoTween()
    {
        // InQuad : ������ �� ������ ����, ���� �� ����
        // OutQuad : ������ �� ����, ���� �� ����
        transform.DOMove(finalPos, moveSpeed).SetEase(Ease.InOutQuad);

        transform.DORotate(finalRotate, rotateSpeed).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(moveSpeed - 0.05f);

        schoolUIManager.OpenUI(schoolUIManager.uiObjects[2]);

        // BoxCollider ��Ȱ��ȭ
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
