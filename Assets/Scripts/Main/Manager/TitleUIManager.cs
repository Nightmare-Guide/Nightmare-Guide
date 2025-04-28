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
            if (currentScene.Equals("0_1")) // �� [��]
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
            // StartCoroutine(WaitForSceneLoadAndSpawnPlayer()); // ����
        }
        else if (currentScene.Equals("3_1")) // é�� 1
        {
            CommonUIManager.instance.MoveScene("School_Scene");
            // StartCoroutine(WaitForSceneLoadAndSpawnPlayer()); // ����
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