using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class TitleUIManager : UIUtility
{
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject newGameAlertUI;
    [SerializeField] GameObject loadGameAlertUI;
    public AudioClip titlebgm;


    private void Awake()
    {
        //마우스 커서 활성화
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // 커서를 보이게 하기
    }

    private void Start()
    {
        if (CommonUIManager.instance != null)
            commonUIManager = CommonUIManager.instance;
        if (TimeLineManager.instance != null)
            timeLineManager = TimeLineManager.instance;

        Cursor.visible = true; // 커서 보이게 하기
        titleUI.SetActive(true); // Start UI 활성화

        optionUI = commonUIManager.optionUI;
        uiObjects.Add(optionUI);

        commonUIManager.uiManager = this;
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayBGM(titlebgm);
            SoundManager.instance.sfxSource.Stop();
            SoundManager.instance.sfxSource.clip = null;
        }
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
        commonUIManager.uiManager = null;
    }

    public void NewGameBtn()
    {
        if (GameDataManager.instance.HasSaveData())
        {
            // 저장된 파일이 있습니다.
            newGameAlertUI.SetActive(true);
        }
        else
        {
            if (SoundManager.instance != null) { SoundManager.instance.StopBGM(); }
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        if (GameDataManager.instance.StartNewGame())
        {
            string currentScene = ProgressManager.Instance.progressData.scene;
            //Debug.Log(currentScene + "현재 씬" + GameDataManager.instance.progressData.scene+"저장씬");
            if (currentScene.Equals("DayHouse")) // 집 [낮]
            {
                commonUIManager.StartNarrationScene();
            }
            else
            {
                Debug.Log("Unknown scene");
            }
        }
    }

    public void LoadGameBtn()
    {
        if (GameDataManager.instance.HasSaveData())
        {
            string currentScene = ProgressManager.Instance.progressData.scene;
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.progressData.scene = currentScene;
            }

            if (SoundManager.instance != null) { SoundManager.instance.StopBGM(); }
            

            //플레이어 하우스
            commonUIManager.MoveScene(currentScene);
        }
        else
        {
            // 저장된 데이터가 없습니다.
            loadGameAlertUI.SetActive(true);
        }

    }

    public void OptionBtn()
    {
        optionUI.SetActive(true);
    }

    public void ExitGameBtn()
    {
        if (GameDataManager.instance != null)
        {
            ProgressManager.Instance.progressData.newGame = false;
            GameDataManager.instance.SaveGame();
        }
        Application.Quit();
    }

}