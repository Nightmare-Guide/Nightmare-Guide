using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : UIUtility
{
    private void Start()
    {
        uiObjects[2].SetActive(true); // BG Img Ȱ��ȭ
        uiObjects[3].SetActive(true); // Start UI Ȱ��ȭ
        CursorUnLocked();
    }

    public void StartGame()
    {

    }

    public void LoadGame()
    {

    }

    public void ExitGame()
    {

    }
}
