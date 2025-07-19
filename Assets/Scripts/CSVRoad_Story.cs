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

    [SerializeField] private TextMeshProUGUI dialogue; // �⺻ �ڸ�
    [SerializeField] private TextMeshProUGUI dialogueName; // �⺻ �ڸ� �̸�
    [SerializeField] private GameObject dialogueBox;  // ��ȭâ
    [SerializeField] private GameObject dialogueOptions; // ������
    [SerializeField] private TextMeshProUGUI option1; // ������ 1
    [SerializeField] private TextMeshProUGUI option2; // ������ 2
    [SerializeField] private GameObject questUI;
    [SerializeField] private TextMeshProUGUI questText;

    [Header("Story Progress")]
    private List<Dictionary<string, object>> data; // CSV ������
    private int progress = 0; // ���� ���൵
    private string currentChapter = ""; // ���� é��
    private int returnPoint = -1; // ���� ����Ʈ ���� (-1�� �ʱ�ȭ ����)
    private int chapterEnd = 0;
    private NPC currentNPC;
    private int questIndex = 0;
    private bool choicebool = false; // ������ ������ �Ǻ�

    public NarrationManager narrationManager;

    void Awake()
    {
        data = CSVReader.Read("Story/" + go_Story);
        if (data != null && data.Count > 0)
        {
            //  Debug.Log($"CSV ������ �ε� ����: �� {data.Count}�� �׸�");
        }
        else
        {
            // Debug.LogWarning("CSV �����Ͱ� ��� �ֽ��ϴ�.");
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
        //  Debug.Log($"SubChapter {subChapterKey} ���õ�");
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
            //  Debug.LogWarning($"{subChapterKey}�� �ش��ϴ� �����Ͱ� �����ϴ�.");
            return;
        }
        Debug.Log("���� é�� üũ: " + data[progress]["Chapter"].ToString());
        Debug.Log("���� é�� ���� üũ: " + chapterEnd);
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
            // CSV �������� ���� ��縦 ������
            string text = FormatDialogue(data[i][LocalizationSettings.SelectedLocale.Identifier.Code].ToString());
            dialogue.text = text;

            dialogueBox.SetActive(string.IsNullOrEmpty(text) ? false : true); // text ������ ���� ������ ��ȭâ ��Ȱ��ȭ

            if (!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString())))
            {
                string name = FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString());
                dialogueName.text = name + " : ";
            }

            // ReturnPoint�� ������ ����
            if (data[i].ContainsKey("ReturnPoint") && data[i]["ReturnPoint"].ToString() == "point")
            {
                returnPoint = i; // ���� ���൵�� ReturnPoint�� ����
                                 //   Debug.Log($"ReturnPoint �����: {returnPoint}");
            }

            // ������ Ȱ��ȭ ó��
            if (data[i]["Character"].ToString().Equals("Select"))
            {
                ActivateSelection(i + 1); // ������ ó��
                yield break;
            }


            if (string.IsNullOrWhiteSpace(data[i]["Time"].ToString()))
            {
                // Time�� null, ��ĭ, ���� ���� ���
                yield return new WaitForSeconds(2f);
            }
            else
            {
                // ���� ���� ������ ���
                if (float.TryParse(data[i]["Time"].ToString(), out float dialogueTime))
                {
                    yield return new WaitForSeconds(dialogueTime);
                }
                else
                {
                    Debug.LogWarning($"Time ���� ��ȿ���� �ʽ��ϴ�: {data[i]["Time"]}");
                    yield return new WaitForSeconds(2f); // �⺻ ���
                }
            }

            progress = i + 1;

            if (!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString()))) { dialogueName.text = ""; }
            dialogue.text = "";

            if (i == end)
            {
                Debug.Log($"SubChapter {data[start]["Chapter"]} ��");
                // ProgressManager.Instance.progressData.storyProgress = data[start]["Chapter"].ToString();  // �׽�Ʈ������ ��� ��Ȱ��ȭ
                // string chap = data[i - 1]["Chapter"].ToString();
                string chap = data[i]["Chapter"].ToString();
                NextAction(chap);
                chapterEnd = 0;
                UIUtility uiManager = CommonUIManager.instance.uiManager;
                if (uiManager != null && !(uiManager is TitleUIManager)) { CommonUIManager.instance.uiManager.CursorLocked(); } // Ŀ�� ��Ȱ��ȭ
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
            Debug.LogWarning("������ �����Ͱ� �����մϴ�.");
        }
    }

    public void OnSelectOption(int choice)
    {
        if (choice == 1) // ������ 1: ReturnPoint�� ���ư���
        {
            if (returnPoint != -1)
            {
                Debug.Log("������ 1 ����: ReturnPoint�� �̵�");
                StartCoroutine(DisplayChapterDialogue(returnPoint, data.Count - 1)); // ReturnPoint���� �ٽ� ���

                Debug.Log("���� é�� : " + data[progress]["Chapter"].ToString());
            }
            else
            {
                // Debug.LogWarning("ReturnPoint�� �������� �ʾҽ��ϴ�.");
            }
        }
        else if (choice == 2) // ������ 2: ���� ��� ����
        {
            Debug.Log("������ 2 ����");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
            returnPoint = -1; // ReturnPoint �ʱ�ȭ
            EndCheck(currentChapter);
            Debug.Log("���� é�� : " + data[progress]["Chapter"].ToString());
            Debug.Log("���� é�� ����: " + chapterEnd);
            // progress�� ������Ʈ�� ���¿��� �ٽ� ��� ���
            StartCoroutine(DisplayChapterDialogue(progress, chapterEnd));
        }
        else if (choice == 3)
        {   //������ ������� ������縦 �����Ű�� ������ ���
            Debug.Log("���� ��� ����");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
            // progress�� ������Ʈ�� ���¿��� �ٽ� ��� ���
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
        // ��翡 �ִ� @@�� �ٹٲ�(\n)���� ��ȯ
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
            //        Debug.Log("0_3_3����");
            //        EthanMother ethanMother = currentNPC as EthanMother;
            //        ethanMother.WorktoPosition();
            //    }
            //    break;
            //case "0_3_4":
            //    if (currentNPC != null)
            //    {
            //        Debug.Log("0_3_4����");
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

        // �񵿱� �ε� �۾�
        UnityEngine.Application.backgroundLoadingPriority = ThreadPriority.Low;

        // AsyncOperation : �ð��� �ɸ��� �۾��� ��׶��忡�� ������ ��, �� ���¸� Ȯ���ϰų� ������ �� �ִ� Ŭ����
        AsyncOperation op = SceneManager.LoadSceneAsync("DayHouse"); // ���� ���� ��׶��忡�� �ε� ���� (�񵿱�)
        op.allowSceneActivation = false; // �ε��� ������ �ٷ� ��ȯ���� �ʰ� ��ٸ�. (ex: �ε� �ִϸ��̼� �� �����ְ� �Ѿ �� ����)

        while (!op.isDone) // �� �����Ӹ��� op.progress ���� Ȯ���ϸ鼭 �ð� ���� -> progress 0.9 : �� ��ȯ �غ� �Ϸ�, 1 : �� ��ȯ �Ϸ�
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
