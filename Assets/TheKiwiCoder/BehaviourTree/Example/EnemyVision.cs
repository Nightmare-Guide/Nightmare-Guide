using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyVision : MonoBehaviour
{
    [Header("탐지 설정")]
    public float detectionRadius = 10f; // 감지 거리
    [Range(0, 360)] public float detectionAngle = 120f; // 감지 시야각 (120도)

    [Header("레이어 설정")]
    public LayerMask playerLayer; // 플레이어 레이어
    public LayerMask obstacleLayer; // 장애물 레이어

    [Header("플레이어 정보")]
    [SerializeField] private Transform player;
    private bool isDetected = false; // 플레이어 탐지 여부

    void Start()
    {
        player = GameManager.instance?.player; // GameManager에서 플레이어 정보 가져오기
    }

    void Update()
    {
        if (player != null)
        {
            isDetected = CheckPlayerInView(); // 플레이어 탐지 여부 확인
        }

        if (isDetected)
        {
            Debug.Log("플레이어 발견! 추격 시작!");
            // 추격 상태로 전환하는 코드 추가 가능 (예: 상태 머신 적용)
        }
    }

    private bool CheckPlayerInView()
    {
        if (player == null) return false;

        // 1. 플레이어와의 거리 확인
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return false;

        // 2. 플레이어가 시야각 내에 있는지 확인
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer > detectionAngle / 2) return false; // 시야각 밖이면 탐지 X

        // 3. 장애물 체크 (레이캐스트로 시야를 방해하는 오브젝트가 있는지 확인)
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            return false; // 장애물에 가려져 있으면 탐지 X
        }

        return true; // 모든 조건을 만족하면 탐지 성공!
    }

    // 플레이어가 감지된 상태를 반환
    public bool IsPlayerDetected()
    {
        return isDetected;
    }

    // 적의 시야를 시각적으로 표시하기 위한 기즈모
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // 감지 범위 표시

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        if (isDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position); // 탐지된 플레이어와의 연결선
        }
    }
}
