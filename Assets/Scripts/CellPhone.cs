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

    // ���ȭ�� UI
    public GameObject LockPhoneUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public Scrollbar pwSlider;
    public TextMeshProUGUI pwText;
    public Material phoneBlurMat;

    public SchoolUIManager schoolUIManager;

    private void Start()
    {
        // ���ȭ�� Text �ʱ�ȭ
        UnityEngine.Color color = pwText.color;  // ���� ���� ��������
        color.a = 160; // ���� ����
        pwText.color = color;  // �� ���� ����

        // �޴��� Blur
        phoneBlurMat.SetFloat("_Size", 0); // Spacing �� �ʱ�ȭ
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            GetDate();
            PhoneSlider();
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

    void PhoneSlider()
    {
        // Slider Value �� ���� ��ȭ
        UnityEngine.Color color = pwText.color;  // ���� ���� ��������
        color.a = Mathf.Lerp(160f / 255f, 0f, pwSlider.value * 2); // ���� ����  // 0 �� 160 / 255, 1 �� 0
        pwText.color = color;  // �� ���� ����

        // �޴��� Blur
        phoneBlurMat.SetFloat("_Size", pwSlider.value / 2.5f); // Spacing �� ����
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
