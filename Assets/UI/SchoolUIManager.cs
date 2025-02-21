using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;
    public GameObject[] uiObjects;

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
        // 휴대폰 UI 시간 실시간 적용
        if (LockPhoneUI.activeInHierarchy)
        {
            GetDate();
        }

        // ui 닫기
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject uiObj in uiObjects)
            {
                if (uiObj.activeInHierarchy)
                {
                    CloseUI(uiObj);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //마우스 커서 중앙에 고정
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // 커서를 안보이게 하기

            Debug.Log("Cursor Locked");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //마우스 커서 중앙에 고정
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;  // 커서를 안보이게 하기

            Debug.Log("Cursor Enable");
        }
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

        //마우스 커서 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 커서를 안보이게 하기

        Debug.Log("UI First Setup");
    }

    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);

        //카메라 회전 활성화
        Camera_Rt.instance.Open_Camera();

        //플레이어 컨트롤 On
        PlayerController.instance.Open_PlayerController();

        //마우스 커서 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // 커서를 안보이게 하기

        Debug.Log("Close UI : " + ui.name);  
    }

    public void OpenPhoneUI()
    {
        blurBG.SetActive(true);
        LockPhoneUI.SetActive(true);

        //카메라 회전 정지
        Camera_Rt.instance.Close_Camera();

        //마우스 커서 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;  // 커서를 보이게 하기

        Debug.Log("Open PhoneUI");
    }
}
