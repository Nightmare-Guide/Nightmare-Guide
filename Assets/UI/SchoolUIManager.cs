using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;

    // 휴대폰 잠금화면
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
