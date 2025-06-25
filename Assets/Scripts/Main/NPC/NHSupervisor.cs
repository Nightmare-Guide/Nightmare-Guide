using UnityEngine;
using System.Collections;
// 제공받은 AnimHelper 스크립트를 사용하기 위해 추가합니다.
// AnimHelper 스크립트가 NPC 네임스페이스 안에 있다면 using NPC; 를 추가해야 합니다.
// UnityEditor.Experimental.GraphView.GraphView는 일반적으로 런타임 코드에 필요하지 않으므로 제거합니다.
// static NPC; 와 static UnityEditor.Experimental.GraphView.GraphView; 는 제거합니다.
// AnimHelper 스크립트가 별도의 네임스페이스 없이 프로젝트 루트에 있다면 using 문이 필요 없을 수도 있습니다.
// 여기서는 별도의 네임스페이스 없이 직접 호출한다고 가정합니다.

public class NHSupervisor : NPC
{
    // 기본 세팅 변수
    [SerializeField]
    [Tooltip("캐릭터가 순차적으로 이동할 목표 지점들입니다.")]
    private Transform[] moveTr; // 이동할 목표 Transform 배열

    // 이동 상태 및 제어 변수
    private bool isMoving = false; // 현재 이동 중인지 여부
    private Coroutine currentMoveCoroutine; // 현재 실행 중인 이동 코루틴 참조

    // 캐릭터 이동 및 회전 속도
    public float speed = 5f; // 이동 속도
    public float rotationSpeed = 100f; // 회전 속도

    

    
    void Start()
    {
        // 테스트를 위해 게임 시작 시 바로 NPC 흐름 시작
        StartNPCFlow();
    }
    // ---
    // 1. NPC가 오른쪽과 왼쪽으로 각각 회전하는 코루틴
    // ---

    /// <summary>
    /// NPC를 지정된 각도만큼 왼쪽으로 회전시키는 코루틴.
    /// </summary>
    /// <param name="angle">회전할 각도 (예: 90f)</param>
    public IEnumerator RotateLeft(float angle)
    {
        float currentRotatedAngle = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - angle, transform.eulerAngles.z);

        while (currentRotatedAngle < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, -step, 0, Space.Self); // Y축 기준으로 왼쪽으로 회전
            currentRotatedAngle += step;
            yield return null;
        }
        // 정확한 목표 각도에 도달하도록 마지막 보정
        transform.rotation = targetRotation;
    }

    /// <summary>
    /// NPC를 지정된 각도만큼 오른쪽으로 회전시키는 코루틴.
    /// </summary>
    /// <param name="angle">회전할 각도 (예: 90f)</param>
    public IEnumerator RotateRight(float angle)
    {
        float currentRotatedAngle = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);

        while (currentRotatedAngle < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, step, 0, Space.Self); // Y축 기준으로 오른쪽으로 회전
            currentRotatedAngle += step;
            yield return null;
        }
        // 정확한 목표 각도에 도달하도록 마지막 보정
        transform.rotation = targetRotation;
    }

    // ---
    // 특정 목표 지점까지 이동하는 헬퍼 코루틴
    // ---
    private IEnumerator MoveToTargetWaypoint(Transform targetWaypoint)
    {
        if (targetWaypoint == null)
        {
            Debug.LogWarning("NHSupervisor: 이동할 목표 지점이 null입니다.");
            yield break;
        }

        // 목표 지점까지 이동
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f) // 충분히 가까워질 때까지
        {
            // 목표 방향 계산
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            // Y축 회전만 고려
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 100); // 회전 속도 조절
            }

            // 이동
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

            yield return null; // 다음 프레임까지 대기
        }
        Debug.Log($"NHSupervisor: {targetWaypoint.name} 지점에 도착했습니다.");
    }

    // ---
    // 요청하신 NPC 행동 흐름을 제어하는 메인 코루틴
    // ---
    public IEnumerator NPCFlowSequence()
    {
        if (moveTr == null || moveTr.Length < 4) // 최소 4개의 포인트가 필요
        {
            Debug.LogError("NHSupervisor: NPC 흐름을 실행하려면 최소 4개의 moveTr 목표 지점이 필요합니다.");
            yield break;
        }

        isMoving = true; // 흐름 시작


        // 1. 10초후 -> 오른쪽으로 회전 -> 워크애니메이션 실행-> 포인트 1까지 이동
        Debug.Log("NHSupervisor: 10초 대기 중...");
        yield return new WaitForSeconds(10f);

        Debug.Log("NHSupervisor: 오른쪽으로 90도 회전 시작...");
        yield return StartCoroutine(RotateRight(90f)); // 오른쪽으로 90도 회전
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        Debug.Log("NHSupervisor: 포인트 1로 이동 시작...");
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[0])); // moveTr[0]이 포인트 1

        // 2. 포인트1에서 회전애니메이션->워크애니메이션-> 포인트2까지 이동
        Debug.Log("NHSupervisor: 포인트 1에서 회전 및 포인트 2로 이동 시작...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // 애니메이션 전환 대기
        yield return StartCoroutine(RotateRight(90f)); // 예시: 오른쪽으로 90도 회전 (필요한 방향으로 조절)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[1])); // moveTr[1]이 포인트 2

        // 3. 포인트 2에서 회전애니메이션->워크애니메이션-> 포인트3까지 이동
        Debug.Log("NHSupervisor: 포인트 2에서 회전 및 포인트 3으로 이동 시작...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // 애니메이션 전환 대기
        yield return StartCoroutine(RotateLeft(90f)); // 예시: 왼쪽으로 90도 회전 (필요한 방향으로 조절)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[2])); // moveTr[2]이 포인트 3

        // 4. 포인트 3에서 회전애니메이션->워크애니메이션-> 포인트4까지 이동
        Debug.Log("NHSupervisor: 포인트 3에서 회전 및 포인트 4로 이동 시작...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // 애니메이션 전환 대기
        yield return StartCoroutine(RotateRight(180f)); // 예시: 180도 회전 (필요한 방향으로 조절)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[3])); // moveTr[3]이 포인트 4

        // 5. 포인트 4에서 회전 애니메이션 -> 토크애니메이션
        Debug.Log("NHSupervisor: 포인트 4에서 회전 및 토크 애니메이션 실행...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // 애니메이션 전환 대기
        yield return StartCoroutine(RotateLeft(90f)); // 예시: 왼쪽으로 90도 회전 (필요한 방향으로 조절)
        AnimHelper.TryPlay(myAnim, "talk1", 0.2f);

        isMoving = false; // 흐름 종료
        Debug.Log("NHSupervisor: NPC 행동 흐름 완료.");
    }

    /// <summary>
    /// NPC 행동 흐름을 시작하는 공개 메서드.
    /// </summary>
    public void StartNPCFlow()
    {
        if (isMoving)
        {
            Debug.LogWarning("NHSupervisor: 이미 NPC 흐름이 실행 중입니다.");
            return;
        }

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
        currentMoveCoroutine = StartCoroutine(NPCFlowSequence());
    }

    /// <summary>
    /// 이동 시퀀스를 강제로 멈추는 공개 메서드.
    /// </summary>
    public void StopMoveSequence()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
        isMoving = false;
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        Debug.Log("NHSupervisor: 이동 시퀀스를 강제로 중지했습니다.");
    }
}