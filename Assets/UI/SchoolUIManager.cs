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

public class SchoolUIManager : MonoBehaviour
{
    [Header("# UI")]
    public GameObject[] uiObjects;
    public GameObject aimUI;

    [Header("# Object")]
    public GameObject cellPhone;
    public bool getCellPhone = false;
    [SerializeField] Camera playerCamera;

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌

    public static SchoolUIManager instance { get; private set; }


    private void Awake()
    {
        FirstSetUP();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
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

        // 테스트용 -> 인벤토리 휴대폰 상호작용
        if (Input.GetKeyDown(KeyCode.Keypad7) && getCellPhone)
        {
            OpenCellPhoneItem();
        }
    }
    
    // 시작 세팅 함수
    void FirstSetUP()
    {
        uiObjects[0].SetActive(false);
        aimUI.SetActive(true);

        CursorLocked(); // 마우스 커서 중앙에 고정

        Debug.Log("UI First Setup");
    }

    // UI 열기 함수
    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);

        // 플레이어 움직임 멈춤
        StopPlayerController();

        Debug.Log("Open PhoneUI");

    }

    // UI 닫기 함수
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

    // 게임 일시 정지 함수
    public void PauseGame()
    {
        // 일시 정지 UI 활성화
        uiObjects[0].SetActive(true);

        // 플레이어 움직임 멈춤
        StopPlayerController();

        //플레이어 컨트롤 OFF
        PlayerController.instance.Close_PlayerController();

        Debug.Log("Pause Game");
    }

    // 오브젝트 상호작용 시 플레이어 움직임 멈춤 함수
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

    // 커서 고정 함수
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

    
    public void SetUIOpacity(Image img, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(img, up, time, waitTime));
    }

    public void SetUIOpacity(TextMeshProUGUI text, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(text, up, time, waitTime));
    }

    // Image 투명도 조절 코루틴
    private IEnumerator SetOpacity(Image img, bool up, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (up) { img.gameObject.SetActive(true); }

        float elapsed = 0f;

        float start = up ? 0f : 1f;
        float end = up ? 1f : 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            UnityEngine.Color color = img.color;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            img.color = color;
            yield return null;
        }

        // 최종 값 보정
        UnityEngine.Color finalColor = img.color;
        finalColor.a = end;
        img.color = finalColor;

        if (!up) { img.gameObject.SetActive(false); }
    }

    // TextMeshProUGUI 투명도 조절 코루틴
    private IEnumerator SetOpacity(TextMeshProUGUI text, bool up, float time, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (up) { text.gameObject.SetActive(true); }

        float elapsed = 0f;

        float start = up ? 0f : 1f;
        float end = up ? 1f : 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            UnityEngine.Color color = text.color;
            color.a = Mathf.Lerp(start, end, elapsed / time);
            text.color = color;
            yield return null;
        }

        // 최종 값 보정
        UnityEngine.Color finalColor = text.color;
        finalColor.a = end;
        text.color = finalColor;

        if (!up) { text.gameObject.SetActive(false); }
    }

    // 인벤토리 휴대폰 버튼 함수
    public void OpenCellPhoneItem()
    {
        // 휴대폰 포지션값 설정
        Vector3[] cellPhoneTransform = playerCamera.GetComponent<RayCast_Aim>().GetCameraInfo();

        cellPhone.GetComponent<Transform>().position = cellPhoneTransform[0];
        cellPhone.GetComponent<Transform>().rotation = Quaternion.Euler(cellPhoneTransform[1]);

        cellPhone.SetActive(true);

        OpenUI(uiObjects[1]); // null 값용 UI 오브젝트 활성화


        // 잠금해제를 한 상태가 아니라면 초기화
        if (!cellPhone.GetComponent<CellPhone>().unLocked)
        {
            cellPhone.GetComponent<CellPhone>().SetFirst();
        }
    }
}
