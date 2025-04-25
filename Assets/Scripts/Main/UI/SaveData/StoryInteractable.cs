using UnityEngine;

public class StoryInteractable : MonoBehaviour
{
    [Header("�� ������Ʈ�� �۵� ������ ���丮 ���൵")]
    public string requiredStoryProgress; 

    [Header("���� ���")]
    public string successDialogue;

    [Header("���� �������� ���")]
    public string failDialogue;

    // ���ͷ�Ʈ ����
    public void Interact()
    {
        string currentProgress = ProgressManager.Instance.progressData.storyProgress;

        if (currentProgress == requiredStoryProgress)
        {
            CSVRoad_Story.instance.OnSelectChapter(successDialogue);
        }
        else
        {
            CSVRoad_Story.instance.OnSelectChapter(failDialogue);
        }
    }
}
