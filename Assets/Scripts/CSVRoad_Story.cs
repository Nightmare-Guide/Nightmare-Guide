using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using static ProgressManager;

public class CSVRoad_Story : MonoBehaviour
{
    public static CSVRoad_Story instance;

    [SerializeField] private string go_Story;

    [SerializeField] private TextMeshProUGUI dialogue; // 기본 자막
    [SerializeField] private TextMeshProUGUI dialogueName; // 기본 자막 이름
    [SerializeField] private GameObject dialogueBox;  // 대화창
    [SerializeField] private GameObject dialogueOptions; // 선택지
    [SerializeField] private TextMeshProUGUI option1; // 선택지 1
    [SerializeField] private TextMeshProUGUI option2; // 선택지 2
    [SerializeField] private GameObject questUI;
    [SerializeField] private TextMeshProUGUI questText;

    [Header("Story Progress")]
    private List<Dictionary<string, object>> data; // CSV 데이터
    private int progress = 0; // 현재 진행도
    private string currentChapter = ""; // 현재 챕터
    private int returnPoint = -1; // 리턴 포인트 저장 (-1은 초기화 상태)
    private int chapterEnd = 0;
    private NPC currentNPC;
    private int questIndex = 0;
    private bool choicebool = false; // 선택지 중인지 판별

    public NarrationManager narrationManager;

    void Awake()
    {
        data = CSVReader.Read("Story/" + go_Story);
        if (data != null && data.Count > 0)
        {
            //  Debug.Log($"CSV 데이터 로드 성공: 총 {data.Count}개 항목");
        }
        else
        {
            // Debug.LogWarning("CSV 데이터가 비어 있습니다.");
        }
        if (instance == null) { instance = this; }
    }

    private void Start()
    {
        //if (!String.IsNullOrEmpty(ProgressManager.Instance.progressData.quest))
        //{
        //    OpenQuestUI(ProgressManager.Instance.progressData.quest);
        //} 
        //else
        //{
        //    CloseQuestUI();
        //}
    }

    public void OnSelectChapter(string subChapterKey, NPC npc = null)
    {
        //  Debug.Log($"SubChapter {subChapterKey} 선택됨");
        currentNPC = npc;

        int start = -1, end = -1;
        for (int i = 0; i < data.Count; i++)
        {
            string chapter = data[i]["Chapter"].ToString();

            if (chapter.Equals(subChapterKey))
            {
                if (start == -1) start = i;
                end = i;
                chapterEnd = end;

            }
            else if (start != -1)
            {
                break;
            }
        }

        if (start == -1)
        {
            //  Debug.LogWarning($"{subChapterKey}에 해당하는 데이터가 없습니다.");
            return;
        }
        Debug.Log("현재 챕터 체크: " + data[progress]["Chapter"].ToString());
        Debug.Log("현재 챕터 종료 체크: " + chapterEnd);
        StartCoroutine(DisplayChapterDialogue(start, end));
    }

    public string GetQuest(string subChapterKey)
    {
        for (int i = questIndex; i < data.Count; i++)
        {
            string chapter = data[i]["Chapter"].ToString();

            if (chapter.Equals(subChapterKey))
            {
                string text = data[i][LocalizationSettings.SelectedLocale.Identifier.Code].ToString();
                questIndex = i;
                return text;
            }
        }

        return "";
    }

    private IEnumerator DisplayChapterDialogue(int start, int end)
    {
        dialogueBox.SetActive(true);

        for (int i = start; i <= end; i++)
        {
            // CSV 데이터의 현재 대사를 가져옴
            string text = FormatDialogue(data[i][LocalizationSettings.SelectedLocale.Identifier.Code].ToString());
            dialogue.text = text;

            dialogueBox.SetActive(string.IsNullOrEmpty(text) ? false : true); // text 내용이 없을 때에는 대화창 비활성화

            if (!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString())))
            {
                string name = FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString());
                dialogueName.text = name + " : ";
            }

            // ReturnPoint가 있으면 저장
            if (data[i].ContainsKey("ReturnPoint") && data[i]["ReturnPoint"].ToString() == "point")
            {
                returnPoint = i; // 현재 진행도를 ReturnPoint로 저장
                                 //   Debug.Log($"ReturnPoint 저장됨: {returnPoint}");
            }

            // 선택지 활성화 처리
            if (data[i]["Character"].ToString().Equals("Select"))
            {
                ActivateSelection(i + 1); // 선택지 처리
                yield break;
            }


            if (string.IsNullOrWhiteSpace(data[i]["Time"].ToString()))
            {
                // Time이 null, 빈칸, 공백 등일 경우
                yield return new WaitForSeconds(2f);
            }
            else
            {
                // 숫자 값이 존재할 경우
                if (float.TryParse(data[i]["Time"].ToString(), out float dialogueTime))
                {
                    yield return new WaitForSeconds(dialogueTime);
                }
                else
                {
                    Debug.LogWarning($"Time 값이 유효하지 않습니다: {data[i]["Time"]}");
                    yield return new WaitForSeconds(2f); // 기본 대기
                }
            }

            progress = i + 1;

            if (!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString()))) { dialogueName.text = ""; }
            dialogue.text = "";

            if (i == end)
            {
                Debug.Log($"SubChapter {data[start]["Chapter"]} 끝");
                // ProgressManager.Instance.progressData.storyProgress = data[start]["Chapter"].ToString();  // 테스트때문에 잠깐 비활성화
                // string chap = data[i - 1]["Chapter"].ToString();
                string chap = data[i]["Chapter"].ToString();
                NextAction(chap);
                chapterEnd = 0;
                UIUtility uiManager = CommonUIManager.instance.uiManager;
                if (uiManager != null && !(uiManager is TitleUIManager)) { CommonUIManager.instance.uiManager.CursorLocked(); } // 커서 비활성화
                CommonUIManager.instance.isTalkingWithNPC = false;
                // break;
            }
        }
        dialogueBox.SetActive(false);
    }

    private void ActivateSelection(int optionStartIndex)
    {
        dialogueBox.SetActive(false);
        dialogueOptions.SetActive(true);
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Close_PlayerController();
            Camera_Rt.instance.Close_Camera();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (optionStartIndex < data.Count)
        {
            option1.text = FormatDialogue(data[optionStartIndex][LocalizationSettings.SelectedLocale.Identifier.Code].ToString());
            option2.text = FormatDialogue(data[optionStartIndex + 1][LocalizationSettings.SelectedLocale.Identifier.Code].ToString());
        }
        else
        {
            Debug.LogWarning("선택지 데이터가 부족합니다.");
        }
    }

    public void OnSelectOption(int choice)
    {
        if (choice == 1) // 선택지 1: ReturnPoint로 돌아가기
        {
            if (returnPoint != -1)
            {
                Debug.Log("선택지 1 선택: ReturnPoint로 이동");
                StartCoroutine(DisplayChapterDialogue(returnPoint, data.Count - 1)); // ReturnPoint부터 다시 출력

                Debug.Log("현재 챕터 : " + data[progress]["Chapter"].ToString());
            }
            else
            {
                // Debug.LogWarning("ReturnPoint가 설정되지 않았습니다.");
            }
        }
        else if (choice == 2) // 선택지 2: 다음 대사 진행
        {
            Debug.Log("선택지 2 선택");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
            returnPoint = -1; // ReturnPoint 초기화
            EndCheck(currentChapter);
            Debug.Log("현재 챕터 : " + data[progress]["Chapter"].ToString());
            Debug.Log("현재 챕터 종료: " + chapterEnd);
            // progress가 업데이트된 상태에서 다시 대사 출력
            StartCoroutine(DisplayChapterDialogue(progress, chapterEnd));
        }
        else if (choice == 3)
        {   //선택지 상관없이 다음대사를 진행시키고 싶을떄 사용
            Debug.Log("다음 대사 진행");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
            // progress가 업데이트된 상태에서 다시 대사 출력
            StartCoroutine(DisplayChapterDialogue(progress, chapterEnd));
        }

        dialogueOptions.SetActive(false);
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Open_PlayerController();
            Camera_Rt.instance.Open_Camera();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        dialogueBox.SetActive(true);
    }

    public void EndCheck(string chap)
    {
        int start = -1, end = -1;

        for (int i = 0; i < data.Count; i++)
        {
            string chapter = data[i]["Chapter"].ToString();
            if (chapter.Equals(chap))
            {
                if (start == -1) start = i;
                end = i;
                chapterEnd = end;

            }
            else if (start != -1)
            {
                break;
            }
        }
    }

    private string FormatDialogue(string text)
    {
        // 대사에 있는 @@를 줄바꿈(\n)으로 변환
        return text.Replace("@@", "\n");
    }

    public void OpenQuestUI(string text)
    {
        questUI.SetActive(true);
        questText.text = text;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.quest = text;
        }

    }

    public void CloseQuestUI()
    {
        if (questUI != null)
        {
            questUI.SetActive(false);
            questText.text = "";
        }

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.progressData.quest = "";
        }

    }

    void NextAction(string chapter)
    {
        switch (chapter)
        {
            case "0_0_0":
                StartCoroutine(FinishNarration());
                break;
            case "0_1_0":
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.CompletedAction(ActionType.StartNewDay);
                }

                // OpenQuestUI(GetQuest("0_1_0_0"));
                break;
            case "0_2_0":
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.CompletedAction(ActionType.FirstMeetMichael);
                }

                if (currentNPC != null) { Michael michael = currentNPC as Michael; michael.DoSweepBroom(); }
                break;
            //case "0_3_0":
            //    if (currentNPC != null)
            //    {
            //        Supervisor supervisor = currentNPC as Supervisor;
            //        Camera_Rt.instance.Open_Camera();
            //        supervisor.GoHospitalRoom();
            //    }
            //    break;
            //case "0_3_1":
            //    if (currentNPC != null)
            //    {
            //        Supervisor supervisor = currentNPC as Supervisor;
            //        supervisor.WalktoIdle();
            //        supervisor.StartSelectBox();
            //    }
            //    break;
            //case "0_3_2":
            //    if (currentNPC != null)
            //    {
            //        Supervisor supervisor = currentNPC as Supervisor;
            //        supervisor.InHospitalRoom();
            //    }
            //    break;
            //case "0_3_3":
            //    if (currentNPC != null)
            //    {
            //        Debug.Log("0_3_3실행");
            //        EthanMother ethanMother = currentNPC as EthanMother;
            //        ethanMother.WorktoPosition();
            //    }
            //    break;
            //case "0_3_4":
            //    if (currentNPC != null)
            //    {
            //        Debug.Log("0_3_4실행");
            //        EthanMother ethanMother = currentNPC as EthanMother;
            //        ethanMother.supervisor.GoNightmare();
            //    }
            //    break;
            case "1_0_0":
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.CompletedAction(ActionType.FirstMeetEthan);
                }

                SoundManager.instance.WallMoveSound();
                if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager) { schoolUIManager.StartPlayerController(); }
                break;
            case "1_0_1":
                if (CommonUIManager.instance.uiManager is SchoolUIManager) { CommonUIManager.instance.uiManager.StartPlayerController(); }
                break;
            case "1_0_3":
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.CompletedAction(ActionType.FirstMeetMonster);
                }

                if (CommonUIManager.instance.uiManager is SchoolUIManager) { CommonUIManager.instance.uiManager.StartPlayerController(); }
                break;
            case "1_0_4":
            case "1_0_6":
                if (CommonUIManager.instance.uiManager is SchoolUIManager) { CommonUIManager.instance.uiManager.StartPlayerController(); }
                break;
            case "2_2_0":
                if (ProgressManager.Instance != null)
                {
                    ProgressManager.Instance.CompletedAction(ActionType.FirstMeetAlex);
                }

                dialogueBox.SetActive(false);
                if (currentNPC != null) { Alex alex = currentNPC as Alex; alex.WalkToOutSide(); }
                CommonUIManager.instance.uiManager.StartPlayerController();
                CommonUIManager.instance.uiManager.CursorLocked();
                CommonUIManager.instance.isTalkingWithNPC = false;
                break;
        }
    }

    IEnumerator FinishNarration()
    {
        narrationManager.SetUIOpacity(narrationManager.videoImg, false, 1f, 0f);

        yield return new WaitForSeconds(1.2f);

        // 비동기 로딩 작업
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

        // AsyncOperation : 시간이 걸리는 작업을 백그라운드에서 진행할 때, 그 상태를 확인하거나 제어할 수 있는 클래스
        AsyncOperation op = SceneManager.LoadSceneAsync("DayHouse"); // 다음 씬을 백그라운드에서 로딩 시작 (비동기)
        op.allowSceneActivation = false; // 로딩이 끝나도 바로 전환되지 않고 기다림. (ex: 로딩 애니메이션 다 보여주고 넘어갈 때 유용)

        while (!op.isDone) // 매 프레임마다 op.progress 값을 확인하면서 시간 누적 -> progress 0.9 : 씬 전환 준비 완료, 1 : 씬 전환 완료
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                Debug.Log("Preparing to switch scene");
            }
            else
            {
                Debug.Log("Finish to switch scene");

                op.allowSceneActivation = true;

                CommonUIManager.instance.Blink(true);
            }
        }
    }
}
