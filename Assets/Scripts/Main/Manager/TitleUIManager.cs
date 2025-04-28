using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : UIUtility
{
    [SerializeField] GameObject titleUI;
    // [SerializeField] GameObject playerPrefab; // 이제 CommonUIManager에서 관리

    private void Awake()
    {
        //마우스 커서 활성화
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // 커서를 보이게 하기
    }

    private void Start()
    {
        titleUI.SetActive(true); // Start UI 활성화

        optionUI = CommonUIManager.instance.optionUI;
        uiObjects.Add(optionUI);

        CommonUIManager.instance.TitleUIManager = this;
    }

    private void Update()
    {
        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 모든 UI 닫기
            foreach (GameObject uiObj in uiObjects)
            {
                if (uiObj.activeInHierarchy)
                {
                    CloseUI(uiObj);
                }
            }
        }
    }

    private void OnDisable()
    {
        CommonUIManager.instance.TitleUIManager = null;
    }

    public void NewGameBtn()
    {
        if (GameDataManager.instance.StartNewGame())
        {
            string currentScene = ProgressManager.Instance.progressData.scene;
            //Debug.Log(currentScene + "현재 씬" + GameDataManager.instance.progressData.scene+"저장씬");
            if (currentScene.Equals("0_1")) // 집 [낮]
            {
                CommonUIManager.instance.MoveScene("DayHouse");
            }
            else
            {
                Debug.Log("Unknown scene");
            }
        }
    }

    public void LoadGameBtn()
    {
        string currentScene = ProgressManager.Instance.progressData.scene;
        
        //플레이어 하우스
        if (currentScene.Equals("0_1") ){CommonUIManager.instance.MoveScene("DayHouse");}
        //플레이어 하우스
        else if (currentScene.Equals("0_2")) { CommonUIManager.instance.MoveScene("NightHouse"); }
        //플레이어 하우스
        else if (currentScene.Equals("1_1")) { CommonUIManager.instance.MoveScene("Main_Map"); }
        //플레이어 하우스
        else if (currentScene.Equals("1_2")) { CommonUIManager.instance.MoveScene("Main_Map_Night"); }
        //플레이어 하우스
        else if (currentScene.Equals("2_1")) { CommonUIManager.instance.MoveScene("DayHospital"); }
        //플레이어 하우스
        else if (currentScene.Equals("2_2")) { CommonUIManager.instance.MoveScene("NightHospital"); }
        //챕터1
        else if (currentScene.Equals("3_1")) { CommonUIManager.instance.MoveScene("School_Scene"); }
        else {   Debug.Log("Unknown scene");   }
    }

    public void OptionBtn()
    {
        optionUI.SetActive(true);
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }
  
}