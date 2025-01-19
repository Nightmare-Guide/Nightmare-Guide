using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSVRoad_Story : MonoBehaviour
{
    [SerializeField] private string go_Story;

    [SerializeField] private TextMeshProUGUI dialogue; // �⺻ �ڸ�
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

    void Start()
    {
        data = CSVReader.Read(go_Story);
        if (data != null && data.Count > 0)
        {
            Debug.Log($"CSV ������ �ε� ����: �� {data.Count}�� �׸�");
        }
        else
        {
            Debug.LogWarning("CSV �����Ͱ� ��� �ֽ��ϴ�.");
        }
    }

    public void OnSelectChapter(string subChapterKey)
    {
        Debug.Log($"SubChapter {subChapterKey} ���õ�");

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
            Debug.LogWarning($"{subChapterKey}�� �ش��ϴ� �����Ͱ� �����ϴ�.");
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
            string text = FormatDialogue(data[i]["Dialogue_Korean"].ToString());
            dialogue.text = text;

            // ReturnPoint�� ������ ����
            if (data[i].ContainsKey("ReturnPoint") && data[i]["ReturnPoint"].ToString() == "point")
            {
                returnPoint = i; // ���� ���൵�� ReturnPoint�� ����
                Debug.Log($"ReturnPoint �����: {returnPoint}");
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
                returnPoint = -1; // ReturnPoint �ʱ�ȭ
            }
            else
            {
                Debug.LogWarning("ReturnPoint�� �������� �ʾҽ��ϴ�.");
            }
        }
        else if (choice == 2) // ������ 2: ���� ��� ����
        {
            Debug.Log("������ 2 ����");
            progress += 4;
            string currentChapter = data[progress]["Chapter"].ToString();
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
        dialogueBox.SetActive(true);
    }



    private string FormatDialogue(string text)
    {
        // ��翡 �ִ� @@�� �ٹٲ�(\n)���� ��ȯ
        return text.Replace("@@", "\n");
    }
}
