using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSVRoad_Story : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogue;

    [Header("���丮 ���൵")]
    private List<Dictionary<string, object>> data; // CSV ���� �����
    private int progress = 0;    // ���� ���൵

    [Header("������")]
    [SerializeField] GameObject dialogue_Box;
    [SerializeField] GameObject dialogue_Obj;
    [SerializeField] TextMeshProUGUI select_1;
    [SerializeField] TextMeshProUGUI select_2;
    public int dialogue_Cnt = 0;  // �÷��̾�/npc/������ ��ġ
    public string dialogue_State; // �÷��̾� /npc/ ������ �Ǻ�

    void Start()
    {
        // CSV ���� �б�
        data = CSVReader.Read("Story");

        if (data != null && data.Count > 0)
        {
            Debug.Log($"CSV ������ �ε� ����: �� {data.Count}�� �׸�");
            StoryProgress(); // ���丮 ����
        }
        else
        {
            Debug.LogWarning("CSV �����Ͱ� ��� �ֽ��ϴ�.");
            dialogue.text = "�����͸� ã�� �� �����ϴ�.";
        }
    }

    public void StoryProgress()
    {
        if (progress >= data.Count)
        {
            Debug.Log("��� ���丮�� �Ϸ��߽��ϴ�.");
            return;
        }

        string currentChapter = data[progress]["Chapter"].ToString();
        string[] chapterParts = currentChapter.Split("_");
        dialogue_State = data[dialogue_Cnt]["Character"].ToString();

        dialogue_Box.SetActive(true);

        if (chapterParts.Length == 2)
        {
            int currentChapterNum = int.Parse(chapterParts[0]);
            int currentDialogueNum = int.Parse(chapterParts[1]);
            Debug.Log($"���� é��: {currentChapterNum}, ���൵: {currentDialogueNum}");
        }
        else
        {
            Debug.LogError("é�� ������ �߸��Ǿ����ϴ�.");
        }

     
      
            StartCoroutine(ChapterProgress());
        
    }

    private IEnumerator ChapterProgress()
    {
        while (progress < data.Count)
        {
            // ���� ��� ���
            string text = data[progress]["Dialogue_Korean"].ToString();
            int index = text.IndexOf("@@");

            if (index != -1)
            {
                text = text.Replace("@@", "\n");
            }

            dialogue.text = text;
            Debug.Log($"���� ��: {dialogue.text}");

            yield return new WaitForSeconds(2f); // 2�� ���

            progress++;
            dialogue_Cnt++;

            // ���� é�ͷ� �Ѿ�� ���� üũ
            if (progress < data.Count)
            {
                string nextChapter = data[progress]["Chapter"].ToString();
                if (!IsSameChapter(nextChapter))
                {
                    Debug.Log("���� é�ͷ� ��ȯ");
                    yield break;
                }

                // ������ ó��
                dialogue_State = data[dialogue_Cnt]["Character"].ToString();
                if (dialogue_State.Equals("Select"))
                {
                    ActivateSelection();
                    yield break;
                }
            }
        }

        Debug.Log("���� é�� �Ϸ�");
        dialogue_Box.SetActive(false);
        dialogue_Obj.SetActive(false);
    }

    private bool IsSameChapter(string nextChapter)
    {
        string currentChapter = data[progress - 1]["Chapter"].ToString();
        return currentChapter.Split("_")[0] == nextChapter.Split("_")[0];
    }

    private void ActivateSelection()
    {
        dialogue_Obj.SetActive(true); // ������ ���̾�α� Ȱ��ȭ
        dialogue_Box.SetActive(false); // ���� ���̾�α� �ڽ� ��Ȱ��ȭ

        // ������ �ؽ�Ʈ ����
        select_1.text = data[progress + 1]["Dialogue_Korean"].ToString();
        select_2.text = data[progress + 2]["Dialogue_Korean"].ToString();

        Debug.Log($"������ ���� �Ϸ�: {select_1.text}, {select_2.text}");
    }

    public void OnSelect(int choice)
    {
        // ���������� ����ڰ� ������ �׸� ó��
        if (choice == 1)
        {
            Debug.Log($"������ 1 ����: {select_1.text}");
            progress -=7;
        }
        else if (choice == 2)
        {
            Debug.Log($"������ 2 ����: {select_2.text}");
            progress += 3;
        }
   
        dialogue_Obj.SetActive(false); // ������ ���̾�α� ��Ȱ��ȭ
        dialogue_Box.SetActive(true);
        StoryProgress(); // ���� ���丮�� ����
    }
}
