using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu_ctrl : MonoBehaviour
{
    [Header("Alert_Page")]
    public GameObject blocker;
    public GameObject Alert_Page;
    public Text Alert_Msg;
    public Button yes_Btn;
    public Button no_Btn;

    private void Start()
    {
        Debug.Log("초기화");
        blocker.SetActive(false);
        Alert_Page.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            string msg = "경고창 테스트";
            Show_Alert_Page(msg);
        }
    }

    //메인 메뉴 메소드
    public void Click_Start_Btn()
    {
        //SceneManager.LoadScene("");
        Debug.Log("게임 시작");
    }

    public void Click_Load_Btn()
    {
        Debug.Log("세이브 파일 불러오기");
        Show_Alert_Page("읎어요");
    }

    public void Click_Setting_Btn()
    {
        Debug.Log("설정");
        Show_Alert_Page("읎어요");
    }

    public void Click_Exit_Btn()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    //경고창 페이지

    public void Show_Alert_Page(string msg)
    {
        blocker.SetActive(true);
        Alert_Page.SetActive(true);

        Alert_Msg.text = msg;
    }

    public void Close_Alert_Page()
    {
        blocker.SetActive(false);
        Alert_Page.SetActive(false);
    }

    public void Click_Alert_Yes_Btn()
    {
        Debug.Log("click Yes");
        Close_Alert_Page();
    }

    public void Click_Alert_No_Btn()
    {
        Debug.Log("click No");
        Close_Alert_Page();
    }
}
