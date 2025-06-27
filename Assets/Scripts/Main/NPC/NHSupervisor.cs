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
        Invoke("StartNPCFlow", 10f); // 시작 지연
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
        Debug.Log("NHSupervisor: talk1 실행");
        anim.SetTrigger("isTalk");
        yield return new WaitForSeconds(2.0f); // talk1 애니메이션 길이

        Debug.Log("NHSupervisor: Turn_Left 실행");
        anim.SetTrigger("LeftTurn");
        yield return new WaitForSeconds(1.0f); // Turn_Left 애니메이션 길이

        Debug.Log("NHSupervisor: Turn_Right 실행");
        anim.SetTrigger("RightTurn");
        yield return new WaitForSeconds(1f); // Turn_Right 애니메이션 길이

        Debug.Log("NHSupervisor: Action1 실행");
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(4f); // Action1 애니메이션 길이

        Debug.Log("NHSupervisor: walk 시작");
        anim.SetBool("isWalk", true);
        // 이동 재시작 필요 시, 다음 지점으로 MoveToTargetWaypoint 호출 가능
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
