using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyVision : MonoBehaviour
{
    [Header("탐지 설정")]
    public float detectionRadius = 20f; // 시야 감지 거리
    [Range(0, 360)] public float detectionAngle = 120f; // 시야각
    public float closeRangeRadius = 12f; // 근거리 전방위 감지
    public float longRangeThreshold = 30f; // 추가: 너무 멀리 도망간 경우 감지

    [Header("레이어 설정")]
    public LayerMask obstacleLayer; // 장애물 레이어
//public LayerMask lockerLayer;   // 락커 레이어 추가

    [Header("플레이어 및 오브젝트 정보")]
    [SerializeField] private GameObject player;
    //[SerializeField] private GameObject detectedLocker; // 감지된 락커 저장

    [Header("탐지 상태")]
    private bool isDetected = false;
   // private bool lockerDetected = false;
    public BehaviourTree behaviourTree;
    private Blackboard blackboard;

    private float detectionCooldown = 1f;
    private bool canDetect = true;

    AudioSource sfxAudio;

    private void Awake()
    {
        sfxAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        blackboard = behaviourTree.blackboard;
    }

    void Update()
    {
        if (canDetect)
        {
            isDetected = CheckPlayerInView();
           // lockerDetected = CheckLockerInView();

            blackboard.isDetected = this.isDetected;
            blackboard.Set("isDetected", isDetected);
           // blackboard.Set("lockerDetected", lockerDetected);

/*          if (lockerDetected && detectedLocker != null)
            {
                blackboard.Set("detectedLocker", detectedLocker);
            }*/
        }

        if (!canDetect)
        {
            StartCoroutine(HandleDetectionCooldown());
        }
    }


    private bool CheckPlayerInView()
    {
        if (player == null) { if (sfxAudio.isPlaying) { sfxAudio.Stop(); } return false; }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // 일정 거리 이상 벗어나면 강제로 감지
        if (distance > longRangeThreshold)
        {
            if (sfxAudio.isPlaying) { sfxAudio.Stop(); }
            return ForceGetPlayerPos();
        }

        // 기존 감지 로직
        bool inView = CheckObjectInView(player);
        bool inCloseRange = CheckObjectInCloseRange(player);

        return inView || inCloseRange;
    }

    // 제일 큰 범위 벗어날 시, 플레이어 위치 값 획득
    private bool ForceGetPlayerPos()
    {
        if (player == null) { if (sfxAudio.isPlaying) { sfxAudio.Stop(); } return false; }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < longRangeThreshold)
        { if (sfxAudio.isPlaying) { sfxAudio.Stop(); } return false; }

        blackboard.moveToPosition = player.transform.position;
        return true;
    }


    // 근거리 Ray
    private bool CheckObjectInCloseRange(GameObject obj)
    {
        if (obj == null) { if (sfxAudio.isPlaying) { sfxAudio.Stop(); } return false; }

        float distance = Vector3.Distance(transform.position, obj.transform.position);
        if (distance > closeRangeRadius) { if (sfxAudio.isPlaying) { sfxAudio.Stop(); } return false; }

        // 장애물 체크 (레이캐스트)
        Vector3 directionToObj = (obj.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToObj, distance, obstacleLayer))
        {
            if (sfxAudio.isPlaying) { sfxAudio.Stop(); }
                return false;
        }

        if (!sfxAudio.isPlaying) { sfxAudio.Play(); }
        return true;
    }


/*    private bool CheckLockerInView()
    {
        Collider[] lockers = Physics.OverlapSphere(transform.position, detectionRadius, lockerLayer);

        foreach (var locker in lockers)
        {
            Locker lockerScript = locker.GetComponent<Locker>(); // Locker 스크립트 가져오기

            if (lockerScript != null && lockerScript.stat == Locker.LockerStat.InMove) // 🔹 InMove 상태인지 확인
            {
                if (CheckObjectInView(locker.gameObject))
                {
                    detectedLocker = locker.gameObject;
                    blackboard.UpdateLockerDetectionStatus(true); // 🔹 블랙보드 값 업데이트
                    return true;
                }
            }
        }

        detectedLocker = null;
        blackboard.UpdateLockerDetectionStatus(false); // 🔹 락커가 탐지되지 않으면 false
        return false;
    }*/


    // 시야각 Ray
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
        Gizmos.color = Color.yellow; //감지색
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // 기존 시야 감지 거리

        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue; //범위
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        //  추가: 전방위 근거리 감지 반경
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // 주황색
        Gizmos.DrawWireSphere(transform.position, closeRangeRadius);


        //  장거리 감지 한계 표시 (회색 실선)
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, longRangeThreshold);

        if (isDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
/*
        if (lockerDetected && detectedLocker != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, detectedLocker.transform.position);
        }*/
    }
}
