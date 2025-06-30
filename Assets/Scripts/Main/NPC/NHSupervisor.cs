using UnityEngine;
using System.Collections;

public class NHSupervisor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("NPC가 이동할 단 하나의 목표 지점입니다.")]
    private Transform moveTr;

    public float speed = 5f;
    public float rotationSpeed = 100f;

    private bool isMoving = false;
    private Coroutine currentMoveCoroutine;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("StartNPCFlow", 5f); // 시작 지연
    }

    private IEnumerator MoveToTargetWaypoint(Transform targetWaypoint)
    {
        if (targetWaypoint == null)
        {
            Debug.LogWarning("NHSupervisor: 이동할 목표 지점이 null입니다.");
            yield break;
        }

        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
        {
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 100f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("NHSupervisor: 포인트 1 도착");

        // 도착 후 시퀀스 실행
        StartCoroutine(PlayPostArrivalSequence());
    }

    private IEnumerator PlayPostArrivalSequence()
    {
      

        Debug.Log("NHSupervisor: -90도 회전 실행");
        yield return StartCoroutine(RotateByRelativeAngle(-90f, 1f));
        Debug.Log("NHSupervisor: talk1 실행");
        anim.SetTrigger("isTalk");
        yield return new WaitForSeconds(2.0f); // talk1 애니메이션 길이
        CSVRoad_Story.instance.OnSelectChapter("2_0_0");
        yield return new WaitForSeconds(1.0f);
        CSVRoad_Story.instance.OnSelectChapter("2_0_1");

        Debug.Log("NHSupervisor: +160도 회전 실행");
        yield return StartCoroutine(RotateByRelativeAngle(160f, 1f));
        Debug.Log("NHSupervisor: Action1 실행");
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(4f); // Action1 애니메이션 길이

        Debug.Log("NHSupervisor: walk 시작");
        anim.SetBool("isWalk", true);
    }

    private IEnumerator RotateByRelativeAngle(float angle, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0); // 상대 회전

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot; // 정확한 최종 값 보정
    }

    private IEnumerator RotateByAngle(float angle, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
    }

    public IEnumerator NPCFlowSequence()
    {
        if (moveTr == null)
        {
            Debug.LogError("NHSupervisor: moveTr가 설정되어 있지 않습니다.");
            yield break;
        }

        isMoving = true;

        Debug.Log("NHSupervisor: walk 애니메이션 시작");
        anim.SetBool("isWalk", true);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr));

        Debug.Log("NHSupervisor: 이동 완료 - walk 종료");
        anim.SetBool("isWalk", false);

        isMoving = false;
    }

    public void StartNPCFlow()
    {
        if (isMoving)
        {
            Debug.LogWarning("NHSupervisor: 이미 실행 중입니다.");
            return;
        }

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(NPCFlowSequence());
    }

    public void StopMoveSequence()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }

        isMoving = false;
        anim.SetTrigger("isIdle");
        Debug.Log("NHSupervisor: 시퀀스 강제 중지됨");
    }
}
