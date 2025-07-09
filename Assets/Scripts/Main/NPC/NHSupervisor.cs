using UnityEngine;
using System.Collections;

public class NHSupervisor : MonoBehaviour
{
    public static NHSupervisor instance;
    [SerializeField] Transform moveTr; // 목표 위치를 지정할 Transform
    [SerializeField] Transform endPoint; // 목표 위치를 지정할 Transform
    [SerializeField] float speed = 5f; // 이동 속도
    [SerializeField] float stopDistance = 0.1f; // 목표 지점 근처에 도달했는지 판단할 거리

    private Animator anim;
    private bool isMoving = false; // 이동 중인지 여부




    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator 컴포넌트가 NHSupervisor와 같은 오브젝트에 없습니다!");
        }
    }

    void Update()
    {
        // isMoving이 true일 때만 이동 로직 실행
        if (isMoving && moveTr != null)
        {
            // 목표 위치까지의 방향 벡터 계산
            Vector3 direction = (moveTr.position - transform.position).normalized;

            // 현재 위치에서 목표 위치까지의 거리 계산
            float distanceToTarget = Vector3.Distance(transform.position, moveTr.position);

            // 목표 지점에 거의 도달했다면 멈춤
            if (distanceToTarget < stopDistance)
            {
                transform.position = moveTr.position; // 정확한 목표 위치로 설정
                StopMovement();
            }
            else
            {
                // 현재 위치를 목표 방향으로 speed만큼 이동
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    // 타임라인 종료 시 Signal Receiver에 의해 호출될 함수
    public void StartMovement()
    {
        Debug.Log("Timeline Ended! Starting Movement...");

        // 애니메이션의 "isWalk" 파라미터를 true로 설정
        if (anim != null)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot set 'isWalk' parameter.");
        }

        // 이동 시작 플래그 설정
        isMoving = true;
    }

    // 이동을 멈추고 애니메이션을 끄는 함수
    private void StopMovement()
    {
        Debug.Log("Movement Finished!");
        isMoving = false; // 이동 중지

        if (anim != null)
        {
            anim.SetBool("isWalk", false); // 걷기 애니메이션 끄기
            anim.SetBool("isIdle1", true);
        }
    }
}