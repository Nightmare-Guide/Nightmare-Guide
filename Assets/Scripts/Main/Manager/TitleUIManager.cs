using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : UIUtility
{
    [SerializeField] GameObject titleUI;


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
        SceneManager.LoadScene("Main_Map");
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
        Application.Quit();
    }
}
