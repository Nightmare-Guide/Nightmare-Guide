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
        Cursor.visible = true; // Ŀ�� ���̰� �ϱ�
        titleUI.SetActive(true); // Start UI Ȱ��ȭ

        optionUI = CommonUIManager.instance.optionUI;
        uiObjects.Add(optionUI);

        CommonUIManager.instance.TitleUIManager = this;
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
        CommonUIManager.instance.TitleUIManager = null;
    }

    public void NewGameBtn()
    {
        if (GameDataManager.instance.StartNewGame())
        {
            string currentScene = ProgressManager.Instance.progressData.scene;
            //Debug.Log(currentScene + "���� ��" + GameDataManager.instance.progressData.scene+"�����");
            if (currentScene.Equals("DayHouse")) // �� [��]
            {
                CommonUIManager.instance.StartNarrationScene();
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
       
        if (CommonUIManager.instance!=null){CommonUIManager.instance.MoveScene(currentScene);}
        
        else {   Debug.Log("CommonUIManager.instance�� null��");   }
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