using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class CSVRoad_Story : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slip;

    [Header("���丮 ���൵")]
    private List<Dictionary<string, object>> data; // CSV ���� �����
    private int progress = 0;    // ���� ���൵

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
            slip.text = "�����͸� ã�� �� �����ϴ�.";
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

        // ���� ��� ����
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
                // �̹� �ٹٲ� ���ڰ� ���Ե� ��� �״�� ��
                text = text.Replace("@@", "\n");
            }

            slip.text = text;
            Debug.Log($"���� ��: {slip.text}");

            yield return new WaitForSeconds(2f); // 2�� ���

            progress++;

            // ���� é�ͷ� �Ѿ�� ���� üũ
            if (progress < data.Count)
            {
                string nextChapter = data[progress]["Chapter"].ToString();
                if (!IsSameChapter(nextChapter))
                {
                    Debug.Log("���� é�ͷ� ��ȯ");
                    yield break;
                }
            }
        }

        Debug.Log("���� é�� �Ϸ�");
    }

    private bool IsSameChapter(string nextChapter)
    {
        string currentChapter = data[progress - 1]["Chapter"].ToString();
        return currentChapter.Split("_")[0] == nextChapter.Split("_")[0];
    }
}
