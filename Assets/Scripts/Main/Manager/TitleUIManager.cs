using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : UIUtility
{
    private void Start()
    {
        uiObjects[2].SetActive(true); // BG Img 활성화
        uiObjects[3].SetActive(true); // Start UI 활성화
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
