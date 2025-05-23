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

    [SerializeField] private TextMeshProUGUI dialogue; // �⺻ �ڸ�
    [SerializeField] private TextMeshProUGUI dialogueName; // �⺻ �ڸ� �̸�
    [SerializeField] private GameObject dialogueBox;  // ��ȭâ
    [SerializeField] private GameObject dialogueOptions; // ������
    [SerializeField] private TextMeshProUGUI option1; // ������ 1
    [SerializeField] private TextMeshProUGUI option2; // ������ 2

    [Header("Story Progress")]
    private List<Dictionary<string, object>> data; // CSV ������
    private int progress = 0; // ���� ���൵
    private string currentChapter = ""; // ���� é��
    private int returnPoint = -1; // ���� ����Ʈ ���� (-1�� �ʱ�ȭ ����)
    private int chapterEnd = 0;
    private NPC currentNPC;

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

    public void OnSelectChapter(string subChapterKey, NPC npc = null)
    {
        //  Debug.Log($"SubChapter {subChapterKey} ���õ�");
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
            //  Debug.LogWarning($"{subChapterKey}�� �ش��ϴ� �����Ͱ� �����ϴ�.");
            return;
        }

        StartCoroutine(DisplayChapterDialogue(start, end));
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

            if(!string.IsNullOrEmpty(FormatDialogue(data[i][$"{LocalizationSettings.SelectedLocale.Identifier.Code}_name"].ToString())))
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
            if (data[i]["Character"].ToString() == "Select")
            {
                ActivateSelection(i + 1); // ������ ó��
                yield break;
            }

            yield return new WaitForSeconds(2f);
            progress = i + 1;

            if (i == end)
            {
                Debug.Log($"SubChapter {data[start]["Chapter"]} ��");
                string chap = data[i - 1]["Chapter"].ToString();
                NextAction(chap);
                chapterEnd = 0;
                UIUtility uiManager = CommonUIManager.instance.uiManager;
                if (uiManager != null && !(uiManager is TitleUIManager)) { CommonUIManager.instance.uiManager.CursorLocked(); } // Ŀ�� ��Ȱ��ȭ
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



    private string FormatDialogue(string text)
    {
        // ��翡 �ִ� @@�� �ٹٲ�(\n)���� ��ȯ
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
