using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class NPC : MonoBehaviour
{
    public string story;
    public Collider col;
    public Transform npcTransform;
    public Transform playerTransform;
    public Animator myAnim;
    public bool inAction = false;
    public NavMeshAgent agent;
    private bool isWalkingToPlayer = false;

    public IEnumerator EnableCollider(Collider col, float time)
    {
        yield return new WaitForSeconds(time);
        col.enabled = true;
    }

    public void LookAtPlayer()
    {
        // 플레이어, NPC 서로 마주보게 하기
        StartCoroutine(SmoothLookAt(npcTransform, playerTransform, 0.35f));
        StartCoroutine(SmoothLookAt(playerTransform, npcTransform, 0.35f));

        // 플레이어 카메라 각도 변경
        StartCoroutine(SmoothRotateTo(PlayerController.instance.GetPlayerCamera().transform, Vector3.zero, 0.35f));

        // 플레이어 컨트롤러 비활성화
        CommonUIManager.instance.uiManager.StopPlayerController();

        CommonUIManager.instance.isTalkingWithNPC = true;
    }

    public IEnumerator SmoothLookAt(Transform me,Transform target, float duration)
    {
        Vector3 direction = target.position - me.position;
        direction.y = 0f; // 수직 방향 제외 (고개 꺾이는 것 방지)

        Quaternion startRotation = me.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        Vector3 r = targetRotation.eulerAngles;

        string meName = me.gameObject.name;
        string targetName = target.gameObject.name;

        if (meName.Contains("Michael") || targetName.Contains("Michael"))
            r += Vector3.down * 5f;
        else if (meName.Contains("Alex") || targetName.Contains("Alex"))
            r += Vector3.up * 2.5f;

        targetRotation = Quaternion.Euler(r);

        float time = 0f;
        while (time < duration)
        {
            me.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        me.rotation = targetRotation; // 마지막 각도 보정
    }

    public IEnumerator SmoothRotateTo(Transform targetTransform, Vector3 targetEulerAngles, float duration)
    {
        Quaternion startRotation = targetTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localRotation = targetRotation; // 마지막 각도 보정
    }

    public void StartWalkToPlayer(Transform player)
    {
        playerTransform = player;
        agent.stoppingDistance = 7f; // 플레이어 근처에서 멈추도록 설정
        agent.autoBraking = false;
        StartCoroutine(WalkToPlayerRoutine());
    }

    private IEnumerator WalkToPlayerRoutine()
    {
        PlayerController.instance.Close_PlayerController();
        isWalkingToPlayer = true;

        agent.SetDestination(playerTransform.position);
        myAnim.SetBool("isWalk", true);

        // 도착할 때까지 기다림
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f)
        {
            yield return null;
        }

        // 도착
        agent.ResetPath();
        myAnim.SetBool("isWalk", false);
        myAnim.SetTrigger("isTalk");
        isWalkingToPlayer = false;
    }
}
