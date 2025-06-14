using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class NHSupervisor : MonoBehaviour
{
    // 이동할 목표 Transform 배열 (인스펙터에서 설정)
    [SerializeField]
    [Tooltip("캐릭터가 순차적으로 이동할 목표 지점들입니다.")]
    private Transform[] moveTr;

    // 현재 이동할 목표 지점의 인덱스
    private int currentTargetIndex = 0;
    // 캐릭터가 현재 이동 중인지 여부를 나타내는 플래그
    private bool isMoving = false;
    // 현재 진행 중인 이동 코루틴 참조
    private Coroutine currentMoveCoroutine;
    public float speed = 5f;
    private void Start()
    {
        Invoke("StartMovingSequence", 10f);
      
    }
    /// <summary>
    /// 캐릭터가 저장된 포인트들을 따라 이동 시퀀스를 시작하는 공개 메서드.
    /// 외부(예: 트리거 스크립트)에서 이 메서드를 호출하여 이동을 시작합니다.
    /// </summary>
    /// <param name="speed">캐릭터의 이동 속도입니다.</param>
    public void StartMovingSequence()
    {
        if (isMoving)
        {
            Debug.LogWarning($"{gameObject.name} (NHSupervisor): 이미 이동 중입니다. 새로운 이동 시퀀스를 시작할 수 없습니다.");
            return;
        }

        if (moveTr == null || moveTr.Length == 0)
        {
            Debug.LogError($"{gameObject.name} (NHSupervisor): 이동할 목표 Transform이 설정되지 않았습니다! 'Move Tr' 배열에 지점을 할당해주세요.");
            return;
        }

        if (speed <= 0f)
        {
            Debug.LogWarning($"{gameObject.name} (NHSupervisor): 이동 속도({speed})가 0 이하여서 이동을 시작할 수 없습니다. 양수 값을 입력해주세요.");
            return;
        }

        // 배열의 첫 번째 지점(index 0)부터 이동을 시작합니다.
        currentTargetIndex = 0;

        // 기존 코루틴이 있다면 안전하게 중지하고 새로운 코루틴 시작
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
        currentMoveCoroutine = StartCoroutine(MoveThroughPoints(speed));
        Debug.Log($"{gameObject.name} (NHSupervisor): 이동 시퀀스 시작! (속도: {speed})");
    }

    /// <summary>
    /// 지정된 모든 목표 지점을 순서대로 이동하는 코루틴입니다.
    /// </summary>
    private IEnumerator MoveThroughPoints(float currentMoveSpeed)
    {
        isMoving = true;

        while (currentTargetIndex < moveTr.Length)
        {
            Transform targetPoint = moveTr[currentTargetIndex];

            if (targetPoint == null)
            {
                Debug.LogWarning($"{gameObject.name} (NHSupervisor): {currentTargetIndex}번 인덱스의 목표 지점이 null입니다. 다음 지점으로 건너뜁니다.");
                currentTargetIndex++;
                continue;
            }

            Debug.Log($"{gameObject.name} (NHSupervisor): {currentTargetIndex + 1}번째 목표 지점({targetPoint.position})으로 이동 중.");

            // 목표 지점까지 순수하게 이동만 합니다. 회전은 애니메이션으로 처리됩니다.
            yield return StartCoroutine(MoveToSingleTarget(targetPoint.position, currentMoveSpeed));

            // 다음 목표 지점 인덱스 증가
            currentTargetIndex++;
        }

        Debug.Log($"{gameObject.name} (NHSupervisor): 모든 목표 지점 이동 완료!");
        isMoving = false;
        currentMoveCoroutine = null; // 코루틴 참조 해제
    }

    /// <summary>
    /// 단일 목표 지점까지 이동하는 코루틴입니다.
    /// </summary>
    /// <param name="targetPosition">도착할 목표 위치</param>
    /// <param name="currentMoveSpeed">적용할 이동 속도</param>
    private IEnumerator MoveToSingleTarget(Vector3 targetPosition, float currentMoveSpeed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.05f) // 0.05f는 도착 오차 범위
        {
            // 이동 속도가 0에 가까우면 무한 루프 방지 및 경고
            if (currentMoveSpeed <= 0.001f)
            {
                Debug.LogWarning($"{gameObject.name} (NHSupervisor): 이동 속도가 너무 낮아({currentMoveSpeed}) 이동이 중단됩니다. 다음 목표 지점으로 이동하려면 속도를 높여주세요.");
                break; // 루프 중단
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }

        transform.position = targetPosition; // 정확한 목표 위치 설정 (오차 보정)
        // yield return new WaitForSeconds(0.2f); // 각 지점 도착 후 잠시 대기 (선택 사항, 필요시 주석 해제)
    }

    // 개발 편의를 위한 시각화 (기즈모) - Unity 에디터에서만 보입니다.
    private void OnDrawGizmos()
    {
        if (moveTr == null || moveTr.Length == 0) return;

        // 이동 경로 시각화
        Gizmos.color = Color.cyan;
        for (int i = 0; i < moveTr.Length; i++)
        {
            if (moveTr[i] != null)
            {
                // 각 목표 지점 표시
                Gizmos.DrawSphere(moveTr[i].position, 0.2f);
                // 지점 번호 표시 (에디터 전용)
#if UNITY_EDITOR
                UnityEditor.Handles.Label(moveTr[i].position + Vector3.up * 0.5f, $"Point {i + 1}"); // 1부터 시작하는 번호
#endif

                // 다음 지점으로의 선 표시
                if (i < moveTr.Length - 1 && moveTr[i + 1] != null)
                {
                    Gizmos.DrawLine(moveTr[i].position, moveTr[i + 1].position);
                }
            }
        }

        // 현재 오브젝트에서 첫 번째 목표 지점까지의 선 (아직 이동 시작 전일 때)
        if (!isMoving && moveTr.Length > 0 && moveTr[0] != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, moveTr[0].position);
        }
    }
}