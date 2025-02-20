using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;

    // �޴��� ���ȭ��
    [Header ("#Lock")]
    public GameObject LockPhoneUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    private void Awake()
    {
        FirstSetUP();
    }

    private void Update()
    {
        GetDate();
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

    void FirstSetUP()
    {
        blurBG.SetActive(false);
        LockPhoneUI.SetActive(false);
    }

    public void OpenPhoneUI()
    {
        blurBG.SetActive(true);
        LockPhoneUI.SetActive(true);
    }
}
