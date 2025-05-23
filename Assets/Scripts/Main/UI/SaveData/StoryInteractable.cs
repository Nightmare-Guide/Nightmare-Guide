using UnityEngine;

public class StoryInteractable : MonoBehaviour
{
    [Header("�� ������Ʈ�� �۵� ������ ���丮 ���൵")]
    public string requiredStoryProgress; 

    [Header("���� ���")]
    public string successDialogue;

    [Header("���� ������ ��翩��")]
    public bool onScript = true;

    [Header("���� �������� ���")]
    public string failDialogue;

    [Header("���� üũ")]
    public bool progressType=true;

    // ���ͷ�Ʈ ����
    public bool Interact(GameObject obj)
    {
        //��Ʃ���� ���ΰ� ����Ʈ�� ������ ���� ������
        if (ProgressManager.Instance.progressData.phoneDatas[0].hasPhone)
        {
            if (ProgressManager.Instance.progressData.scene.Equals("DayHouse") && obj.name.Equals("MyDoor")){
                return false;
            }
        }
        //���� ���丮 ���൵
        string currentProgress = ProgressManager.Instance.progressData.storyProgress;
        
        //���丮 ���൵�� ������ ȹ�濩�� �Ǻ�
        if (progressType)
        {
            //���� ���൵�� ������ ���൵�� ������ �Ǻ�
            if (currentProgress.Equals(requiredStoryProgress))
            {
                //���൵�� ���ŵɶ� ��ũ��Ʈ ��� ����
                if (onScript)
                {
                    CSVRoad_Story.instance.OnSelectChapter(successDialogue);
                }
                else
                {
                    //Debug.Log("��� ����");
                }
                return false;
            }
            else
            {
                //�ݺ����
                CSVRoad_Story.instance.OnSelectChapter(failDialogue);
                return true;
            }
        }
        else
        {
            //������ ȹ�� ���η� ���� ���� ���� �Ǻ�
            // ���⿡ ������ ȹ�� ���� Ȯ�� ������ �߰��ϰ�,
            // �� ����� ���� return true; �Ǵ� return false; �� �ۼ��ؾ� �մϴ�.
            return false; // �ӽ÷� false ��ȯ
        }

        // �� ��ġ���� �� �̻� �������� ������, ������ ���� �⺻ ��ȯ ���� �߰��� ���� �ֽ��ϴ�.
        // return false;
    }
}
