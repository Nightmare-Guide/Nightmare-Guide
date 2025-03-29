using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("탐지 설정")]
    public float detectionRadius = 10f; // 감지 거리
    [Range(0, 360)] public float detectionAngle = 120f; // 감지 시야각

    [Header("레이어 설정")]
    public LayerMask obstacleLayer; // 장애물 레이어
    public LayerMask lockerLayer;   // 락커 레이어 추가

    [Header("플레이어 및 오브젝트 정보")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject detectedLocker; // 감지된 락커 저장

    [Header("탐지 상태")]
    private bool isDetected = false;
    private bool lockerDetected = false;
    public BehaviourTree behaviourTree;
    private Blackboard blackboard;

    private float detectionCooldown = 1f;
    private bool canDetect = true;

    void Start()
    {
        blackboard = behaviourTree.blackboard;
    }

    void Update()
    {
        if (canDetect)
        {
            isDetected = CheckPlayerInView();
            lockerDetected = CheckLockerInView();

            blackboard.Set("isDetected", isDetected);
            blackboard.Set("lockerDetected", lockerDetected);

            if (lockerDetected && detectedLocker != null)
            {
                blackboard.Set("detectedLocker", detectedLocker);
            }
        }

        if (!canDetect)
        {
            StartCoroutine(HandleDetectionCooldown());
        }
    }

    private bool CheckPlayerInView()
    {
        return CheckObjectInView(player);
    }

    private bool CheckLockerInView()
    {
        Collider[] lockers = Physics.OverlapSphere(transform.position, detectionRadius, lockerLayer);

        foreach (var locker in lockers)
        {
            if (CheckObjectInView(locker.gameObject))
            {
                detectedLocker = locker.gameObject;
                return true;
            }
        }

        detectedLocker = null;
        return false;
    }

    private bool CheckObjectInView(GameObject obj)
    {
        if (obj == null) return false;

        // 거리 체크
        float distanceToObj = Vector3.Distance(transform.position, obj.transform.position);
        if (distanceToObj > detectionRadius) return false;

        // 시야각 체크
        Vector3 directionToObj = (obj.transform.position - transform.position).normalized;
        float angleToObj = Vector3.Angle(transform.forward, directionToObj);
        if (angleToObj > detectionAngle / 2) return false;

        // 장애물 체크 (레이캐스트)
        if (Physics.Raycast(transform.position, directionToObj, distanceToObj, obstacleLayer))
        {
            return false;
        }

        return true;
    }

    private IEnumerator HandleDetectionCooldown()
    {
        canDetect = false;
        yield return new WaitForSeconds(detectionCooldown);
        canDetect = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        if (isDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

        if (lockerDetected && detectedLocker != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, detectedLocker.transform.position);
        }
    }
}
