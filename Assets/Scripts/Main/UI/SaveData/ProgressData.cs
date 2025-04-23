using UnityEngine;

[CreateAssetMenu(fileName = "ProgressData", menuName = "Game/ProgressData")]
public class ProgressData : ScriptableObject
{
    [Header("진행도 예: 챕터_서브_노드 형식")]
    public string storyProgress = "0_0_0";

    [Header("아이템 소지 여부")]
    public bool getSmartPhone = false;

    [Header("플레이어 위치")]
    public Vector3 playerPosition = Vector3.zero;

    [Header("플레이어 산치")]
    public int sanchi = 0;
    // 초기화 기능
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        getSmartPhone = false;
        playerPosition = Vector3.zero;
    }
}
