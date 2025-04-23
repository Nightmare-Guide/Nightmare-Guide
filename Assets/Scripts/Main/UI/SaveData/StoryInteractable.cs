using UnityEngine;

public class StoryInteractable : MonoBehaviour
{
    [Header("이 오브젝트가 작동 가능한 스토리 진행도")]
    public string requiredStoryProgress; 

    [Header("정상 대사")]
    public string successDialogue;

    [Header("조건 불충족시 대사")]
    public string failDialogue;

    // 인터랙트 실행
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
