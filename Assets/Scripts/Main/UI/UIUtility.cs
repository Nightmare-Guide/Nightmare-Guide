using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices;
using UnityStandardAssets.Characters.FirstPerson;
using Unity.VisualScripting;
using UnityEngine.Playables;
using UnityEditor.VersionControl;

public class UIUtility : MonoBehaviour
{
    [Header("# UI")]
    public List<GameObject> uiObjects;
    public GameObject aimUI;
    public GameObject optionUI;
    public Image fadeInOutImg;

    [Header("# Object")]
    [SerializeField] Camera playerCamera;

    [Header("# TimeLine")]
    public PlayableDirector playableDirector;

    [Header("# Singleton")]
    public CommonUIManager commonUIManager;
    public TimeLineManager timeLineManager;

    // Windows의 마우스 입력을 시뮬레이션하는 API
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 마우스 왼쪽 버튼 누름
    private const int MOUSEEVENTF_LEFTUP = 0x04; // 마우스 왼쪽 버튼 뗌

    // 인게임 UI 열기 함수 -> Player O
    public void InGameOpenUI(GameObject ui)
    {
        OpenUI(ui);

        // 플레이어 움직임 멈춤
        StopPlayerController();
    }

    // 인게임 UI 닫기 함수 -> Player O
    public void InGameCloseUI(GameObject ui)
    {
        CloseUI(ui);

        Time.timeScale = 1;

        if (!CommonUIManager.instance.isTalkingWithNPC)
        {
            StartPlayerController();
        }
    }

    public void OpenUI(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);
    }

    // 게임 일시 정지 함수
    protected void PauseGame(GameObject blur)
    {
        // TimeLine 이 실행 중이면 정지
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            playableDirector.Pause();
        }

        // 플레이어 움직임 멈춤
        StopPlayerController();

        // 시간 정지
        Time.timeScale = 0;

        // 일시 정지 UI 활성화
        blur.SetActive(true);
        optionUI.SetActive(true);
    }

    // 오브젝트 상호작용 시 플레이어 움직임 멈춤 함수
    public void StopPlayerController()
    {
        //플레이어 컨트롤 OFF
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Close_PlayerController();
        }

        if (Camera_Rt.instance != null)
        {
            //카메라 회전 정지
            Camera_Rt.instance.Close_Camera();
        }

        CursorUnLocked();
    }

    public void StartPlayerController()
    {
        if (PlayerController.instance != null)
        {
            //플레이어 컨트롤 On
            PlayerController.instance.Open_PlayerController();
        }

        if (Camera_Rt.instance != null)
        {
            //카메라 회전 활성화
            Camera_Rt.instance.Open_Camera();
        }

        // 마우스 커서 중앙에 고정
        CursorLocked();
    }

    // UI 오브젝트 모두 비활성화 상태인 지 확인
    protected bool AreAllObjectsDisabled(List<GameObject> uiObjs)
    {
        return uiObjs.All(obj => !obj.activeSelf);
    }

    // 커서 고정 함수
    public void CursorLocked()
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

    public void CursorUnLocked()
    {
        // 에임 UI 비활성화
        aimUI.SetActive(false);

        //마우스 커서 활성화
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;  // 커서를 보이게 하기
    }


    public void SetUIOpacity(Image img, bool up, float time, float waitTime)
    {
        StartCoroutine(SetOpacity(img, up, time, waitTime));
    }

    public void SetUIOpacity(RawImage img, bool up, float time, float waitTime)
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

    private IEnumerator SetOpacity(RawImage img, bool up, float time, float waitTime)
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

    // 부동 소수점 오차 방지 함수
    public bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }

    public bool ApproximatelyEqual(float a, float b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

    // VerticalLayoutGroup 초기화
    public void RebuildVerticalLayout(List<VerticalLayoutGroup> verticalLayoutGroup)
    {
        foreach (VerticalLayoutGroup group in verticalLayoutGroup)
        {
            LayoutRebuilder.MarkLayoutForRebuild(group.GetComponent<RectTransform>());
        }
    }


    // TimeLine
    public void FadeIn()
    {
        fadeInOutImg.gameObject.SetActive(true);
        SetUIOpacity(fadeInOutImg, false, 0.4f, 0f);
    }

    public void FadeOut()
    {
        SetUIOpacity(fadeInOutImg, true, 0.4f, 0f);
    }

    public void FinishedTimeLine()
    {
        Debug.Log("Finish TimeLine");

        // 데이터 key 값으로 찾아서 저장
        ProgressManager.Instance.progressData.timelineWatchedList.Find(e => e.key == playableDirector.playableAsset.name).value = true;

        playableDirector.Stop();
        playableDirector.playableAsset = null;

        // 에임 UI 활성화
        aimUI.SetActive(true);

        // StartPlayerController();
    }

    public void EnableCollider(Collider col)
    {
        col.enabled = true;
    }

    public void DisableCollider(Collider col)
    {
        col.enabled = false;
    }

    public IEnumerator EnableCollider(Collider col, float time)
    {
        yield return time;
        col.enabled = true;
    }

    public void StartTimeLine(PlayableAsset asset)
    {
        if (timeLineManager.playableAssets.Count > 0 && playableDirector != null)
        {
            // 이미 실행된 적 있으면 return
            if (ProgressManager.Instance.progressData.timelineWatchedList.Find(e => e.key == asset.name).value)
                return;
            // -> 추격 전 타임라인은 리스폰 후 데이터 초기화 필요

            // 에임 UI 비활성화
            aimUI.SetActive(false);

            //마우스 커서 비활성화
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;  // 커서를 안 보이게 하기

            StopPlayerController();

            // 타임라인 실행
            playableDirector.playableAsset = asset;
            playableDirector.Play();
        }
    }

    // 포스트 프로세싱, Fog 데이터 불러오기
    public void GetPostFogData()
    {
        CommonUIManager commonUIManager = CommonUIManager.instance;
        string fogName = ProgressManager.Instance.progressData.fogName;
        string postName = ProgressManager.Instance.progressData.postProcessingName;

        commonUIManager.ApplyFog(commonUIManager.fogSettings.Find(info => info.name.Equals(fogName)));
        Camera_Rt.instance.ApplyPostProcessing(postName);
    }

    // 데이터에 맞게 오브젝트 활성화/비활성화
    public void CheckObjData(ProgressManager.ActionType action, GameObject obj)
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager progress = ProgressManager.Instance;

            // Debug.Log($"obj : {obj.name}, active : {!progress.IsActionCompleted(action)}");
            obj.SetActive(!progress.IsActionCompleted(action));
        }
    }
    public void CheckObjData(ProgressManager.ActionType action, Collider collider)
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager progress = ProgressManager.Instance;

            collider.enabled = !progress.IsActionCompleted(action);
        }
    }
}
