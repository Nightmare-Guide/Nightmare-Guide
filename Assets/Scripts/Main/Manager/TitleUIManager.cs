using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : UIUtility
{
    [SerializeField] GameObject titleUI;
    // [SerializeField] GameObject playerPrefab; // ���� CommonUIManager���� ����

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

        commonUIManager.TitleUIManager = this;
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
        commonUIManager.TitleUIManager = null;
    }

    public void NewGameBtn()
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
        string currentScene = ProgressManager.Instance.progressData.scene;
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.scene = currentScene;
        }
        //�÷��̾� �Ͽ콺
        commonUIManager.MoveScene(currentScene);
        
       
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