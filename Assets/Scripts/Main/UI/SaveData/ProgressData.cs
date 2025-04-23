using UnityEngine;

[CreateAssetMenu(fileName = "ProgressData", menuName = "Game/ProgressData")]
public class ProgressData : ScriptableObject
{
    [Header("���൵ ��: é��_����_��� ����")]
    public string storyProgress = "0_0_0";

    [Header("������ ���� ����")]
    public bool getSmartPhone = false;

    [Header("�÷��̾� ��ġ")]
    public Vector3 playerPosition = Vector3.zero;

    [Header("�÷��̾� ��ġ")]
    public int sanchi = 0;
    // �ʱ�ȭ ���
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        getSmartPhone = false;
        playerPosition = Vector3.zero;
    }
}
