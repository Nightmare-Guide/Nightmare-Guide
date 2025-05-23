using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

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

    [Header("Story Progress")]
    private List<Dictionary<string, object>> data; // CSV 데이터
    private int progress = 0; // 현재 진행도
    private string currentChapter = ""; // 현재 챕터
    private int returnPoint = -1; // 리턴 포인트 저장 (-1은 초기화 상태)
    private int chapterEnd = 0;
    private NPC currentNPC;

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

    public void OnSelectChapter(string subChapterKey, NPC npc = null)
    {
        //  Debug.Log($"SubChapter {subChapterKey} 선택됨");
        currentNPC = npc;

        int start = -1, end = -1;

        for (int i = 0; i < data.Count; i++)
        {
            string chapter = data[i]["Chapter"].ToString();
            if (chapter == subChapterKey)
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

        StartCoroutine(DisplayChapterDialogue(start, end));
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

            if(!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString())))
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
            if (data[i]["Character"].ToString() == "Select")
            {
                ActivateSelection(i + 1); // 선택지 처리
                yield break;
            }

            yield return new WaitForSeconds(2f);
            progress = i + 1;

            if (i == end)
            {
                Debug.Log($"SubChapter {data[start]["Chapter"]} 끝");
                string chap = data[i - 1]["Chapter"].ToString();
                NextAction(chap);
                chapterEnd = 0;
                UIUtility uiManager = CommonUIManager.instance.uiManager;
                if (uiManager != null && !(uiManager is TitleUIManager)) { CommonUIManager.instance.uiManager.CursorLocked(); } // 커서 비활성화
                break;
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



    private string FormatDialogue(string text)
    {
        // 대사에 있는 @@를 줄바꿈(\n)으로 변환
        return text.Replace("@@", "\n");
    }

    void NextAction(string chapter)
    {
        switch (chapter)
        {
            case "0_0_0":
                StartCoroutine(FinishNarration());
                break;
            case "0_2_0":
                if(currentNPC != null) { Michael michael = currentNPC as Michael; michael.DoSweepBroom(); }
                break;
            case "0_3_0":
                Supervisor.instance.StartHospitalRoom();
                break;
            case "2_2_0":
                if (currentNPC != null) { Alex alex = currentNPC as Alex; alex.WalkToOutSide(); }
                break;
        }
    }

    IEnumerator FinishNarration()
    {
        narrationManager.SetUIOpacity(narrationManager.videoImg, false, 1f, 0f);

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene("DayHouse");

        CommonUIManager.instance.Blink(true);
    }
}
