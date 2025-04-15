using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : UIUtility
{
    [SerializeField] GameObject titleUI;

    private void OnEnable()
    {
        CommonUIManager.instance.TitleUIManager = this;
        optionUI = CommonUIManager.instance.optionUI;
    }

    private void Start()
    {
        uiObjects[2].SetActive(true); // BG Img Ȱ��ȭ
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
        
    }

    public void LoadGameBtn()
    {

    }

    public void OptionBtn()
    {
        optionUI.SetActive(true);
    }

    public void ExitGameBtn()
    {

    }
}
