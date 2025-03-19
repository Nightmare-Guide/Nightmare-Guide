using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("탐지 설정")]
    public float detectionRadius = 10f; // 감지 거리
    [Range(0, 360)] public float detectionAngle = 120f; // 감지 시야각 (120도)

    [Header("레이어 설정")]
    public LayerMask obstacleLayer; // 장애물 레이어

    [Header("플레이어 정보")]
    [SerializeField] private GameObject player;

    [Header("탐지 상태")]
    private bool isDetected = false; // 플레이어 탐지 여부
    public BehaviourTree behaviourTree; // 비헤이비어 트리 연결
    private Blackboard blackboard; // 블랙보드 변수

    private float detectionCooldown = 1f; // 감지 후 다시 체크하기 위한 대기 시간
    private bool canDetect = true; // 감지 가능한지 여부

    void Start()
    {
        blackboard = behaviourTree.blackboard; // 블랙보드 가져오기
    }

    void Update()
    {
        // 감지할 수 있을 때만 탐지 진행
        if (canDetect)
        {
            // 플레이어가 탐지된 상태인지 확인
            isDetected = CheckPlayerInView();
            blackboard.Set("isDetected", isDetected); // 블랙보드에 탐지 상태를 설정

            // 디버그 로그로 설정된 값 확인
            Debug.Log("[EnemyVision] isDetected value set to: " + isDetected);
        }

        // 탐지 후 일정 시간 동안 감지를 중지하는 코루틴 실행
        if (!canDetect)
        {
            StartCoroutine(HandleDetectionCooldown());
        }
    }

    private bool CheckPlayerInView()
    {
        if (player == null) return false;

        // 1. 거리 체크
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > detectionRadius) return false;

        // 2. 시야각 체크
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > detectionAngle / 2) return false;

        // 3. 장애물 체크 (레이캐스트)
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
        {
            Debug.Log("[EnemyVision] Player is blocked by an obstacle."); // 장애물 체크
            return false;
        }

        return true; // 위 조건을 모두 통과하면 플레이어를 감지
    }

    // 탐지 후 일정 시간 동안 다시 탐지하지 않도록 대기하는 코루틴
    private IEnumerator HandleDetectionCooldown()
    {
        canDetect = false; // 일정 시간 동안 탐지 중지
        yield return new WaitForSeconds(detectionCooldown); // 대기 시간 (예: 1초)
        canDetect = true; // 다시 탐지 가능
    }

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
            Gizmos.DrawLine(transform.position, player.transform.position); // 탐지된 플레이어와의 연결선
        }
    }
}
