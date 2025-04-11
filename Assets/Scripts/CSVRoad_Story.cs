using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSVRoad_Story : MonoBehaviour
{
    [SerializeField] private string go_Story;

    [SerializeField] private TextMeshProUGUI dialogue; // 기본 자막
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

    void Start()
    {
        data = CSVReader.Read("Story/"+go_Story);
        if (data != null && data.Count > 0)
        {
            Debug.Log($"CSV 데이터 로드 성공: 총 {data.Count}개 항목");
        }
        else
        {
            Debug.LogWarning("CSV 데이터가 비어 있습니다.");
        }
    }

    public void OnSelectChapter(string subChapterKey)
    {
        Debug.Log($"SubChapter {subChapterKey} 선택됨");

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
            Debug.LogWarning($"{subChapterKey}에 해당하는 데이터가 없습니다.");
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
            string text = FormatDialogue(data[i]["Dialogue_Korean"].ToString());
            dialogue.text = text;

            // ReturnPoint가 있으면 저장
            if (data[i].ContainsKey("ReturnPoint") && data[i]["ReturnPoint"].ToString() == "point")
            {
                returnPoint = i; // 현재 진행도를 ReturnPoint로 저장
                Debug.Log($"ReturnPoint 저장됨: {returnPoint}");
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
                chapterEnd = 0;
                break;
            }
        }

        dialogueBox.SetActive(false);
    }

    private void ActivateSelection(int optionStartIndex)
    {
        dialogueBox.SetActive(false);
        dialogueOptions.SetActive(true);

        if (optionStartIndex < data.Count)
        {
            option1.text = FormatDialogue(data[optionStartIndex]["Dialogue_Korean"].ToString());
            option2.text = FormatDialogue(data[optionStartIndex + 1]["Dialogue_Korean"].ToString());
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
                returnPoint = -1; // ReturnPoint 초기화
            }
            else
            {
                Debug.LogWarning("ReturnPoint가 설정되지 않았습니다.");
            }
        }
        else if (choice == 2) // 선택지 2: 다음 대사 진행
        {
            Debug.Log("선택지 2 선택");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
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
        dialogueBox.SetActive(true);
    }



    private string FormatDialogue(string text)
    {
        // 대사에 있는 @@를 줄바꿈(\n)으로 변환
        return text.Replace("@@", "\n");
    }
}
