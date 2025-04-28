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
        
        //�÷��̾� �Ͽ콺
        if (currentScene.Equals("0_1") ){CommonUIManager.instance.MoveScene("DayHouse");}
        //�÷��̾� �Ͽ콺
        else if (currentScene.Equals("0_2")) { CommonUIManager.instance.MoveScene("NightHouse"); }
        //�÷��̾� �Ͽ콺
        else if (currentScene.Equals("1_1")) { CommonUIManager.instance.MoveScene("Main_Map"); }
        //�÷��̾� �Ͽ콺
        else if (currentScene.Equals("1_2")) { CommonUIManager.instance.MoveScene("Main_Map_Night"); }
        //�÷��̾� �Ͽ콺
        else if (currentScene.Equals("2_1")) { CommonUIManager.instance.MoveScene("DayHospital"); }
        //�÷��̾� �Ͽ콺
        else if (currentScene.Equals("2_2")) { CommonUIManager.instance.MoveScene("NightHospital"); }
        //é��1
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