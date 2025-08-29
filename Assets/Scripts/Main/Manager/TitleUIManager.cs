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
        //���콺 Ŀ�� Ȱ��ȭ
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // Ŀ���� ���̰� �ϱ�
    }

    private void Start()
    {
        if (CommonUIManager.instance != null)
            commonUIManager = CommonUIManager.instance;
        if (TimeLineManager.instance != null)
            timeLineManager = TimeLineManager.instance;

        Cursor.visible = true; // Ŀ�� ���̰� �ϱ�
        titleUI.SetActive(true); // Start UI Ȱ��ȭ

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
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ��� UI �ݱ�
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
            // ����� ������ �ֽ��ϴ�.
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
            //Debug.Log(currentScene + "���� ��" + GameDataManager.instance.progressData.scene+"�����");
            if (currentScene.Equals("DayHouse")) // �� [��]
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
            

            //�÷��̾� �Ͽ콺
            commonUIManager.MoveScene(currentScene);
        }
        else
        {
            // ����� �����Ͱ� �����ϴ�.
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