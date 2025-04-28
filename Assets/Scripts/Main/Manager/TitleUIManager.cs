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
                CommonUIManager.instance.MoveScene("Main_Map");
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
       
        if (currentScene.Equals("0_1") || currentScene.Equals("0_2") || currentScene.Equals("1_1") || currentScene.Equals("1_2") || currentScene.Equals("2_1") || currentScene.Equals("2_2") || currentScene.Equals("3_2"))
        {
            CommonUIManager.instance.MoveScene("Main_Map");
            // StartCoroutine(WaitForSceneLoadAndSpawnPlayer()); // 제거
        }
        else if (currentScene.Equals("3_1")) // 챕터 1
        {
            CommonUIManager.instance.MoveScene("School_Scene");
            // StartCoroutine(WaitForSceneLoadAndSpawnPlayer()); // 제거
        }
        else
        {
            Debug.Log("Unknown scene");
        }
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