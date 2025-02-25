using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.Characters.FirstPerson;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;
using System.Drawing;
using UnityEngine.UIElements;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;
    public GameObject[] uiObjects;
    public GameObject aimUI;

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌

    private void Awake()
    {
        FirstSetUP();
    }

    private void Update()
    {
        // ESC 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled())
            {
                // 일시정지 UI 활성화
                PauseGame();
            }
            else
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
    }

    void FirstSetUP()
    {
        blurBG.SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // 마우스 커서 중앙에 고정

        Debug.Log("UI First Setup");
    }

    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);

        // 플레이어 움직임 멈춤
        StopPlayerController();

        Debug.Log("Open PhoneUI");

    }
    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);

        //카메라 회전 활성화
        Camera_Rt.instance.Open_Camera();

        //플레이어 컨트롤 On
        PlayerController.instance.Open_PlayerController();

        // 마우스 커서 중앙에 고정
        CursorLocked();

        Debug.Log("Close UI : " + ui.name);  
    }

    public void PauseGame()
    {
        // 일시 정지 UI 활성화
        blurBG.SetActive(true);

        // 플레이어 움직임 멈춤
        StopPlayerController();

        Debug.Log("Pause Game");
    }

    void StopPlayerController()
    {
        // 에임 UI 비활성화
        aimUI.SetActive(false);

        //카메라 회전 정지
        Camera_Rt.instance.Close_Camera();

        //마우스 커서 활성화
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // 커서를 보이게 하기
    }

    // UI 오브젝트 모두 비활성화 상태인 지 확인
    bool AreAllObjectsDisabled()
    {
        Debug.Log("All UI Objects Disabled");

        return uiObjects.All(obj => !obj.activeSelf);
    }

    void CursorLocked()
    {
        // 에임 UI 활성화
        aimUI.SetActive(true);

        // 게임 시작 시 기본적으로 Locked 상태 유지
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // 화면 중앙을 클릭하는 효과를 발생시킴 (Windows 전용)
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}
