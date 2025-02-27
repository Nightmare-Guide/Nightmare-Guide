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
    [SerializeField] private bool isDetected = false; // 플레이어 탐지 여부 (인스펙터에서 확인 가능)
    public BehaviourTree behaviourTree;

    void Start()
    {

    }

    void Update()
    {
        if (Chapter1_Mgr.instance.player != null&& player == null)
        {
            player = Chapter1_Mgr.instance.player;
            Debug.Log("실행됨");
        }

        if (player != null)
        {
            isDetected = CheckPlayerInView(); // 플레이어 탐지 여부 확인
        }

        if (isDetected)
        {
            Debug.Log("플레이어 발견! 추격 시작!");
            // 추격 상태로 전환하는 코드 추가 가능
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
            return false; // 장애물에 가려져 있으면 탐지 X
        }

        return true; // 모든 조건을 만족하면 탐지 성공
    }

    public bool IsPlayerDetected()
    {
        return isDetected;
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
