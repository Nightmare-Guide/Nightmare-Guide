using UnityEngine;

public class StoryInteractable : MonoBehaviour
{
    [Header("이 오브젝트가 작동 가능한 스토리 진행도")]
    public string requiredStoryProgress; 

    [Header("정상 대사")]
    public string successDialogue;

    [Header("조건 만족시 대사여부")]
    public bool onScript = true;

    [Header("조건 불충족시 대사")]
    public string failDialogue;

    [Header("조건 체크")]
    public bool progressType=true;

    // 인터랙트 실행
    public bool Interact(GameObject obj)
    {
        //토튜리얼 주인공 스마트폰 보유시 에만 문열림
        if (ProgressManager.Instance.progressData.phoneDatas[0].hasPhone)
        {
            if (ProgressManager.Instance.progressData.scene.Equals("DayHouse") && obj.name.Equals("MyDoor")){
                return false;
            }
        }
        //현재 스토리 진행도
        string currentProgress = ProgressManager.Instance.progressData.storyProgress;
        
        //스토리 진행도나 아이템 획득여부 판별
        if (progressType)
        {
            //현재 진행도와 세팅한 진행도가 같은지 판별
            if (currentProgress.Equals(requiredStoryProgress))
            {
                //진행도가 갱신될때 스크립트 출력 여부
                if (onScript)
                {
                    CSVRoad_Story.instance.OnSelectChapter(successDialogue);
                }
                else
                {
                    //Debug.Log("대사 없음");
                }
                return false;
            }
            else
            {
                //반복대사
                CSVRoad_Story.instance.OnSelectChapter(failDialogue);
                return true;
            }
        }
        else
        {
            //아이템 획득 여부로 진행 진행 가능 판별
            // 여기에 아이템 획득 여부 확인 로직을 추가하고,
            // 그 결과에 따라 return true; 또는 return false; 를 작성해야 합니다.
            return false; // 임시로 false 반환
        }

        // 이 위치에는 더 이상 도달하지 않지만, 만약을 위해 기본 반환 값을 추가할 수도 있습니다.
        // return false;
    }
}
