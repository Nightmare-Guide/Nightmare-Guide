using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Steven : MonoBehaviour
{
    public Transform[] targetTransform; // 걷고 싶은 목표 지점
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    private int currentTargetIndex = -1;


    private void Start()
    {
        WalkToGoal(0);
    }



    public void WalkToGoal(int tfIndex)
    {
        if (tfIndex < 0 || tfIndex >= targetTransform.Length) return;

        currentTargetIndex = tfIndex;

        agent.isStopped = false;
        agent.SetDestination(targetTransform[tfIndex].position);
        AnimHelper.TryPlay(anim, "walk", 0.1f);

        StartCoroutine(WaitAndRotate());
    }

    private IEnumerator WaitAndRotate()
    {
        // 도착할 때까지 대기
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.isStopped = true;
        AnimHelper.TryPlay(anim, "idle1", 0.1f); // 도착 후 애니메이션 전환 (선택)

        // 회전 시작
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = targetTransform[currentTargetIndex].rotation;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
